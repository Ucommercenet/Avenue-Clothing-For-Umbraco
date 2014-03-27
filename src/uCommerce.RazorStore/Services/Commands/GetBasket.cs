namespace UCommerce.RazorStore.Services.Commands
{
    using System.Collections.Generic;

    using ServiceStack.ServiceInterface;
    using ServiceStack.ServiceInterface.ServiceModel;

    using UCommerce.Api;

    using System;
    using System.Linq;

    using UCommerce;
    using UCommerce.EntitiesV2;
    using UCommerce.RazorStore.Services.Model;
    using UCommerce.Runtime;

    using umbraco.MacroEngines;

    using Basket = UCommerce.RazorStore.Services.Model.Basket;

    public class GetBasket
    {
    }

    public class GetBasketResponse : IHasResponseStatus
    {
        public GetBasketResponse()
        {
        }

        public GetBasketResponse(UCommerce.EntitiesV2.Basket basket)
        {
            var currency = SiteContext.Current.CatalogContext.CurrentCatalog.PriceGroup.Currency;

            var po = basket.PurchaseOrder;

            var subTotal = new Money(po.SubTotal.Value, currency);
            var taxTotal = new Money(po.TaxTotal.Value, currency);
            var discountTotal = new Money(po.DiscountTotal.Value, currency);
            var orderTotal = new Money(po.OrderTotal.Value, currency);

            Basket = new Basket
                {
                    SubTotal = po.SubTotal,
                    TaxTotal = po.TaxTotal,
                    DiscountTotal = po.DiscountTotal,
                    OrderTotal = po.OrderTotal,
                    TotalItems = po.OrderLines.Sum(l => l.Quantity),

                    FormattedSubTotal = subTotal.ToString(),
                    FormattedTaxTotal = taxTotal.ToString(),
                    FormattedDiscountTotal = discountTotal.ToString(),
                    FormattedOrderTotal = orderTotal.ToString(),
                    FormattedTotalItems = po.OrderLines.Sum(l => l.Quantity).ToString("#,##"),

                    LineItems = new List<LineItem>()
                };

            foreach (var line in po.OrderLines)
            {
                var product = CatalogLibrary.GetProduct(line.Sku);
                var url = CatalogLibrary.GetNiceUrlForProduct(product);
                var imageUrl = getImageUrlForProduct(product);
                var lineTotal = new Money(line.Total.Value, currency);

                var lineItem = new LineItem
                    {
                        OrderLineId = line.OrderLineId,
                        Quantity = line.Quantity,
                        Sku = line.Sku,
                        VariantSku = line.VariantSku,
                        Url = url,
                        ImageUrl = imageUrl,
                        Price = line.Price,
                        ProductName = line.ProductName,
                        Total = line.Total,
                        FormattedTotal = lineTotal.ToString(),
                        UnitDiscount = line.UnitDiscount,
                        VAT = line.VAT,
                        VATRate = line.VATRate
                    };
                Basket.LineItems.Add(lineItem);
            }
        }

        private string getImageUrlForProduct(Product product)
        {
            var thumbnail = getImageUrlFromMediaItem(product.ThumbnailImageMediaId);
            
            // If we have a thumbnail image then return that otherwise return the product's main image
            return String.IsNullOrWhiteSpace(thumbnail)
                ? getImageUrlFromMediaItem(product.PrimaryImageMediaId)
                : thumbnail;
        }

        private string getImageUrlFromMediaItem(string mediaId)
        {
            if (String.IsNullOrWhiteSpace(mediaId))
                return String.Empty;

            dynamic mediaItem = new DynamicMedia(mediaId);
            return mediaItem.umbracoFile;
        }

        public ResponseStatus ResponseStatus { get; set; }
        public Basket Basket { get; set; }
    }
    public class GetBasketService : ServiceBase<GetBasket>
    {
        protected override object Run(GetBasket request)
        {
            var basket = TransactionLibrary.GetBasket(false);
            return new GetBasketResponse(basket);
        }

    }
}