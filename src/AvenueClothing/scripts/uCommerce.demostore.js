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

    if ($('.js-confidences').length) {
        var confidencesSettings = {
            mobileFirst: true,
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 3000,
            speed: 2000,
            arrows: false,
            dots: false,
            infinite: true,
            responsive: [
                {
                    breakpoint: 600,
                    settings: {
                        slidesToShow: 2
                    }
                },
                {
                    breakpoint: 760,
                    settings: {
                        slidesToShow: 3
                    }
                },
                {
                    breakpoint: 1024,
                    settings: "unslick"
                }
            ]
        };

        var $slick_slider = $('.js-confidences');
        $slick_slider.slick(confidencesSettings);
    }



    // if ($('.js-multi-item-carousel').length && $('.js-multi-item-carousel').children().length > 3) {
    //     $('.js-multi-item-carousel').slick({
    //         mobileFirst: true,
    //         slidesToShow: 2,
    //         slidesToScroll: 2,
    //         autoplay: true,
    //         arrows: false,
    //         dots: true,
    //         infinite: true,
    //         responsive: [
    //             {
    //                 breakpoint: 760,
    //                 settings: {
    //                     slidesToShow: 3,
    //                     slidesToScroll: 3,
    //                     centerMode: true
    //                 }
    //             },
    //             {
    //                 breakpoint: 1024,
    //                 settings: {
    //                     slidesToShow: 4,
    //                     slidesToScroll: 4,
    //                     centerMode: false
    //                 }
    //             }
    //         ]
    //     });
    // }
});
function updateCartTotals() {
    if ($('#empty-cart').length != 0) {
        prepCart();
    }

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
        //var highlight = [qty, sub, tax, disc, tot];
        //$(highlight).effect("highlight", {}, 500);
    });
};
function prepCart() {
    var icn = $("<i>", { "class": "fa fa-shopping-cart icon-white margin-right-small" });
    var qty = $("<span>", { "class": "item-qty" });
    var tot = $("<span>", { "class": "order-total" });
    var cart = $("<a>", { 'href': '/basket', "id": "mini-cart" });

    cart.append(icn).append(qty).append(" items in cart, total: ").append(tot);
    $('#empty-cart').replaceWith(cart);
};
