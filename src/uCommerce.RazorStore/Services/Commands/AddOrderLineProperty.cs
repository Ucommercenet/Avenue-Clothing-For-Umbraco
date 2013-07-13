using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Services.Model;
using UCommerce.Runtime;

namespace UCommerce.RazorStore.Services.Commands
{
    // http://stackoverflow.com/questions/17478654/ucommerce-add-dynamic-property-to-order-line/17541940#17541940
    public class AddOrderLineProperty
    {
        public int? OrderLineId { get; set; }

        public string Sku { get; set; }

        public string VariantSku { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
    public class AddOrderLinePropertyResponse : IHasResponseStatus
    {
        public AddOrderLinePropertyResponse() { }

        public AddOrderLinePropertyResponse(UCommerce.EntitiesV2.OrderLine line)
        {
            if (line == null)
            {
                UpdatedLine = new LineItem();
                return;
            }

            var currency = SiteContext.Current.CatalogContext.CurrentCatalog.PriceGroup.Currency;
            var lineTotal = new Money(line.Total.Value, currency);

            UpdatedLine = new LineItem()
            {
                OrderLineId = line.OrderLineId,
                Quantity = line.Quantity,
                Sku = line.Sku,
                VariantSku = line.VariantSku,
                Price = line.Price,
                ProductName = line.ProductName,
                Total = line.Total,
                FormattedTotal = lineTotal.ToString(),
                UnitDiscount = line.UnitDiscount,
                VAT = line.VAT,
                VATRate = line.VATRate
            };
        }

        public ResponseStatus ResponseStatus { get; set; }

        public LineItem UpdatedLine { get; set; }
    }
    public class AddOrderLinePropertyService : ServiceBase<AddOrderLineProperty>, IUCommerceApiService
    {
        protected override object Run(AddOrderLineProperty request)
        {
            var orderLineId = request.OrderLineId;
            var sku = request.Sku;
            var variantSku = request.VariantSku;

            var orderLine = findOrderLine(orderLineId, sku, variantSku);
            addPropertyToOrderLine(orderLine, request.Key, request.Value);

            TransactionLibrary.ExecuteBasketPipeline();
            var newLine = findOrderLine(orderLineId, sku, variantSku);
            return new AddOrderLinePropertyResponse(newLine);
        }

        private void addPropertyToOrderLine(OrderLine orderLine, string key, string value)
        {
            if (orderLine == null)
                return;

            orderLine[key] = value;

            orderLine.Save();
        }

        private static OrderLine findOrderLine(int? orderLineId, string sku, string variantSku)
        {
            return orderLineId.HasValue
                                ? getOrderLineByOrderLineId(orderLineId)
                                : getOrderLineBySku(sku, variantSku);
        }

        private static OrderLine getOrderLineBySku(string sku, string variantSku)
        {
            return String.IsNullOrWhiteSpace(variantSku)
                                ? getOrderLines().FirstOrDefault(l => (l.Sku == sku))
                                : getOrderLines().FirstOrDefault(l => (l.Sku == sku && l.VariantSku == variantSku));
        }

        private static OrderLine getOrderLineByOrderLineId(int? orderLineId)
        {
            return getOrderLines().FirstOrDefault(l => l.OrderLineId == orderLineId);
        }

        private static ICollection<OrderLine> getOrderLines()
        {
            return TransactionLibrary.GetBasket().PurchaseOrder.OrderLines;
        }
    }
}