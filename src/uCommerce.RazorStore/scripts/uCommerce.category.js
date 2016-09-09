(function() {
    var categoryId = $('#category-id').val();
    var catalogId = $('#catalog-id').val();

    var prices = document.querySelectorAll('[data-productsku]');
    for (var i = 0; i < prices.length; i++) {
        var currentPriceElement = prices[i];
        var currentSku = currentPriceElement.dataset.productsku;

        $.uCommerce.getProductInformation({
                CatalogId: parseInt(catalogId),
                CategoryId: parseInt(categoryId),
                Sku: currentSku
            },
            function(data) {
                document.querySelector("p[data-productsku='" + data.Sku + "']").innerHTML = data.PriceCalculation.YourPrice.Amount.DisplayValue;
            });
    }
})()