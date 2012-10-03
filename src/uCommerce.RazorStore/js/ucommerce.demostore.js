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
    $('#newsletter-form').submit(function (e) {
        e.preventDefault();
        var form = $(this);
        if (!form.validate()) {
            return false;
        }
        form.css('opacity', 0.5);
        $.getJSON(this.action + "?callback=&", $(this).serialize(), function (data) {
            var thanks = $('<p />', { text: "Thanks! We\'ll be in touch soon!" });
            if (data.Status === 400) {
                alert("Error: " + data.Message);
                form.css('opacity', 1);
                $('input:first', form).focus();
            } else { // 200
                form.fadeOut(300, function () { $(this).css('opacity', 1).html(thanks).fadeIn(300); });
            }
        });
        return false;
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