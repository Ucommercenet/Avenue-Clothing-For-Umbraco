using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Api.Model;
using UCommerce.Runtime;
using UCommerce.Search;
using Basket = UCommerce.RazorStore.Api.Model.Basket;
using CatalogContext = Ucommerce.Api.CatalogContext;
using ICatalogContext = Ucommerce.Api.ICatalogContext;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
    public class AvenueClothingApiBasketController : ApiController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        [Route("razorstore/basket/addToBasket")]
        [HttpPost]
        public IHttpActionResult AddToBasket([FromBody] AddToBasketRequet request)
        {
            TransactionLibrary.AddToBasket(request.Quantity, request.Sku, request.VariantSku, addToExistingLine: true,
                executeBasketPipeline: true);
            return Ok();
        }

        [Route("razorstore/basket/updateLineitem")]
        public IHttpActionResult UpdateLineItem([FromBody] UpdateLineItemRequest request)
        {
            TransactionLibrary.UpdateLineItem(request.OrderLineId, request.NewQuantity);
            TransactionLibrary.ExecuteBasketPipeline();

            var orderLine = TransactionLibrary.GetBasket().OrderLines.First(l => l.OrderLineId == request.OrderLineId);

            Currency currency = Currency.Get(CatalogContext.CurrentPriceGroup.CurrencyISOCode);
            var lineTotal = new Money(orderLine.Total.GetValueOrDefault(), currency);

            var updatedLine = new LineItem()
            {
                OrderLineId = orderLine.OrderLineId,
                Quantity = orderLine.Quantity,
                Sku = orderLine.Sku,
                VariantSku = orderLine.VariantSku,
                Price = orderLine.Price,
                ProductName = orderLine.ProductName,
                Total = orderLine.Total,
                FormattedTotal = lineTotal.ToString(),
                UnitDiscount = orderLine.UnitDiscount,
                VAT = orderLine.VAT,
                VATRate = orderLine.VATRate
            };

            return Json(updatedLine);
        }

        [Route("razorstore/basket/getbasket")]
        [HttpGet]
        public IHttpActionResult GetBasket()
        {
            var currency = Currency.Get(CatalogContext.CurrentPriceGroup.CurrencyISOCode);
            var purchaseOrder = TransactionLibrary.GetBasket(false);

            var subTotal = new Money(purchaseOrder.SubTotal.Value, currency);
            var taxTotal = new Money(purchaseOrder.TaxTotal.Value, currency);
            var discountTotal = new Money(purchaseOrder.DiscountTotal.Value, currency);
            var orderTotal = new Money(purchaseOrder.OrderTotal.Value, currency);

            var basket = new Basket
            {
                SubTotal = purchaseOrder.SubTotal,
                TaxTotal = purchaseOrder.TaxTotal,
                DiscountTotal = purchaseOrder.DiscountTotal,
                OrderTotal = purchaseOrder.OrderTotal,
                TotalItems = purchaseOrder.OrderLines.Sum(l => l.Quantity),

                FormattedSubTotal = subTotal.ToString(),
                FormattedTaxTotal = taxTotal.ToString(),
                FormattedDiscountTotal = discountTotal.ToString(),
                FormattedOrderTotal = orderTotal.ToString(),
                FormattedTotalItems = purchaseOrder.OrderLines.Sum(l => l.Quantity).ToString("#,##"),

                LineItems = new List<LineItem>()
            };

            foreach (var line in purchaseOrder.OrderLines)
            {
                var product = CatalogLibrary.GetProduct(line.Sku);
                var url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product});
                var imageUrl = product.PrimaryImageUrl;
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
                basket.LineItems.Add(lineItem);
            }

            return Json(new {Basket = basket});
        }

        private string GetImageUrlForProduct(Product product)
        {
            var thumbnail = GetImageUrlFromMediaItem(product.ThumbnailImageMediaId);

            // If we have a thumbnail image then return that otherwise return the product's main image
            return String.IsNullOrWhiteSpace(thumbnail)
                ? GetImageUrlFromMediaItem(product.PrimaryImageMediaId)
                : thumbnail;
        }

        private string GetImageUrlFromMediaItem(string mediaId)
        {
            if (String.IsNullOrWhiteSpace(mediaId))
                return String.Empty;

            var umbracoHelper = Umbraco.Web.Composing.Current.UmbracoHelper;

            var image = umbracoHelper.Media(Guid.Parse(mediaId));
            return image.Url;
        }
    }
}