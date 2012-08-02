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
    $.uCommerce.defaults = {
        servicepath: '/ucommerceapi',
        protocol: location.protocol,
        host: location.host
    };
    function callServiceStack(request, onSuccess, onError) {
        var gateway = new servicestack.ClientGateway($.uCommerce.defaults.protocol + "//" + $.uCommerce.defaults.host + $.uCommerce.defaults.servicepath + '/');
        
        gateway.postToService(request, onSuccess, onError);
    }
})(jQuery);


