var uCommerce = uCommerce || {};
(function ($) {
    $.extend({
        uCommerce: {
            getBasket: function (options, onSuccess, onError) {
                var defaults = {
//                    create: false
                };
                var extendedOptions = $.extend(defaults, options);
                $.get('/ucommerceapi/razorstore/basket/getbasket', extendedOptions)
                    .done(function (response) {
                        onSuccess(response);
                    }).fail(function (response) {
                    if (onError) onError(response);
                });
            },
            getProductVariations: function (options, onSuccess, onError) {
                //console.log('getProductVariations');

                var defaults = {};
                var extendedOptions = $.extend(defaults, options);

                $.post('/ucommerceapi/razorstore/products/getproductvariations', extendedOptions)
                    .done(function (response) {
                        onSuccess(response);
                    }).fail(function (response) {
                    if (onError) onError(response);
                });
            },
            getSkuFromSelection: function (options, onSuccess, onError) {
                //console.log('sku from selection');
                var defaults = {};
                var extendedOptions = $.extend(defaults, options);
                $.post('/ucommerceapi/razorstore/products/getvariantskufromselection', extendedOptions)
                    .done(function (response) {
                        onSuccess(response);
                    }).fail(function (response) {
                    if (onError) onError(response);
                });

            },
            getVariantSkuFromSelection: function (options, onSuccess, onError) {
                //console.log('getVariantSkuFromSelection');

                var defaults = {};
                var extendedOptions = $.extend(defaults, options);
                $.ajax({
                    type: 'POST',
                    url: '/ucommerceapi/razorstore/products/getvariantskufromselection',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(extendedOptions)
                }).done(function (response) {
                    onSuccess(response);
                }).fail(function (response) {
                    if (onError) onError(response);
                });
            },
            suggest: function (options, onSuccess, onError) {
                var defaults = {};
                var extendedOptions = $.extend(defaults, options);
                $.post('/ucommerceapi/razorstore/suggest', extendedOptions)
                    .done(function (response) {
                        onSuccess(response);
                    }).fail(function (response) {
                    if (onError) onError(response);
                });
            },
            addToBasket: function (options, onSuccess, onError) {
                var defaults = {
                    quantity: 1,
                    sku: '',
                    variantSku: '',
                    addToExistingLine: true
                };
                var extendedOptions = $.extend(defaults, options);

                $.post('/ucommerceapi/razorstore/basket/addToBasket', extendedOptions)
                    .done(function (response) {
                        onSuccess(response);
                    }).fail(function (response) {
                    if (onError) onError(response);
                });
            },
            getProductInformation: function (options, onSuccess, onError) {
                var defaults = {
                    Sku: '',
                    CatalogId: -1,
                    CategoryId: -1
                };
                var extendedOptions = $.extend(defaults, options);

                $.post('/ucommerceapi/razorstore/products/getproductinformation', extendedOptions)
                    .done(function (response) {
                        onSuccess(response);
                    }).fail(function (response) {
                    if (onError) onError(response);
                });
            },
            updateLineItem: function (options, onSuccess, onError) {
                var defaults = {
                    orderLineId: 0,
                    newQuantity: 1
                };
                var extendedOptions = $.extend(defaults, options);
                callServiceStack({UpdateLineItem: extendedOptions}, onSuccess, onError);
            }
        }
    });
})(jQuery);


