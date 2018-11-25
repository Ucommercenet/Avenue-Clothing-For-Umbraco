var uCommerce = uCommerce || {};
(function ($) {
    $.extend({
        uCommerce : {
            getBasket: function(options, onSuccess, onError) {
                var defaults = {
//                    create: false
                };
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ GetBasket: extendedOptions }, onSuccess, onError);
            },
            getProductVariations: function(options, onSuccess, onError) {
                var defaults = {};
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ GetProductVariations: extendedOptions }, onSuccess, onError);
            },
            getSkuFromSelection: function(options, onSuccess, onError) {
                var defaults = {};
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ GetVariantSkuFromSelection: extendedOptions }, onSuccess, onError);
            },
            getVariantSkuFromSelection: function (options, onSuccess, onError) {
                var defaults = {};
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ GetVariantSkuFromSelection: extendedOptions }, onSuccess, onError);
            },
            search: function(options, onSuccess, onError) {
                var defaults = { };
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ Search: extendedOptions }, onSuccess, onError);
            },
            addToBasket: function(options, onSuccess, onError) {
                 var defaults = {
                    quantity: 1, 
                    sku: '',
                    variantSku: '',
                    addToExistingLine: true
                };
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ AddToBasket: extendedOptions }, onSuccess, onError);
            },
            getProductInformation: function(options, onSuccess, onError) {
                var defaults = {
                    Sku: '',
                    CatalogId: -1,
                    CategoryId: -1
                };
                var extendedOptions = $.extend(defaults, options);

                $.post('razorstore/products/getproductinformation', extendedOptions)
                .done(function () {
                        console.log('success');
                    }).fail(function() {
                        console.log('fail');
                });
            },
            updateLineItem: function(options, onSuccess, onError) {
                 var defaults = {
                    orderLineId: 0, 
                    newQuantity: 1
                };
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({ UpdateLineItem: extendedOptions }, onSuccess, onError);
            }
        }
    });
})(jQuery);


