$(function () {
    $('form.validate').each(function () {
        $(this).validate({
            errorElement: "span",
            errorClass: "help-inline",
            highlight: function (label) {
                $(label).closest('.control-group').addClass('error');
            },
            success: function (label) {
                label.closest('.control-group').addClass('success');
            }
        });

    });
    $('#site-search').typeahead({
        minLength: 3,
        source: function (query, process) {
            $.uCommerce.search({ keyword: query }, function (resp) {
                $.map(resp, function (data) {
                    var matches = [];
                    for (var i = 0; i < data.length; i++) {
                        matches.push(data[i].ProductName);
                    }
                    return process(matches);
                });
            });
        }
    });
});
function updateCartTotals() {
    $.uCommerce.getBasket({}, function (response) {
        var basket = response.Basket;
        var qty = $(".item-qty");
        var sub = $(".order-subtotal");
        var tax = $(".order-tax");
        var disc = $(".order-discounts");
        var tot = $(".order-total");
        
        qty.text(basket.FormattedTotalItems);
        sub.text(basket.FormattedSubTotal);
        tax.text(basket.FormattedTaxTotal);
        disc.text(basket.FormattedDiscountTotal);
        tot.text(basket.FormattedOrderTotal);

        // Pulse the cart so it catches the user's attention
        var highlight = [qty,sub,tax,disc,tot];
        $(highlight).effect("highlight", {}, 500);
    });
};
