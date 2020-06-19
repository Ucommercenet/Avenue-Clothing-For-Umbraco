﻿$(function () {
    var $body = $('body');
    var throttleInterval = 250;

    if ($('#catalogue').length) {
        var $loadMoreBtn = $('.js-load-more');
        var $numResultsSelect = $('#resultsToShow');
        var $products = $('#products');
        var perPage = $numResultsSelect.val();
        var size = perPage;
        var page = 1;
        var totalProducts = parseInt($products.data('num-products'));
        var searchParams = new URLSearchParams(window.location.search);

        if (searchParams.get('size').length) {
            size = parseInt(searchParams.get('size'));
            page =  parseInt(searchParams.get('pg'));

            if (searchParams.get('pp')) {
                perPage = parseInt(searchParams.get('pp'));
            }

            if (perPage &&  $numResultsSelect.find('option[value='+ perPage +']').length) {
                $numResultsSelect.val(perPage);
            }

            if (size*page < totalProducts) {
                $loadMoreBtn.removeClass('d-none');
            }
        }

        $numResultsSelect.on('click', function() {
            if ($(this).val() != perPage) {
                searchParams.set('pg', '1')
                searchParams.set('size', $(this).val());
                searchParams.set('pp', $(this).val());
                window.location.search = searchParams.toString();
            }
        });

        $loadMoreBtn.on('click', function() {
            var currentlyShowing = size*page;
            searchParams.set('size', currentlyShowing + parseInt(perPage));
            window.location.search = searchParams.toString();
        });
    }

    $('#main-nav').on('hidden.bs.collapse', function () {
        hideCollapse('category-nav');
    });

    $('#main-nav').on('show.bs.collapse', function () {
        hideCollapse('navbar-form');
    });

    $('#navbar-form').on('show.bs.collapse', function () {
        hideCollapse('main-nav');
        hideCollapse('category-nav');
    });

    function hideCollapse(elementId) {
        $('#' + elementId + '').removeClass('show');
        $('.navbar-toggler[data-target="' + elementId +'"]').addClass('collapsed').attr('aria-expanded','false');
    }

    $(window).on('scroll resize', throttle( function() {
        condenseHeader();
        collapseSiteNav();
    }, throttleInterval))

    $(window).on('resize', throttle( function() {
        setupConfidences();
    }, throttleInterval))

    function condenseHeader() {
        var scrollDistance = 10;
        var currentDistance = $(window).scrollTop();
        var scrolledClass = 'scrolled';

        if (currentDistance < scrollDistance) {
            $body.removeClass(scrolledClass);
        } else {
            $body.addClass(scrolledClass);
        }
    }

    function collapseSiteNav() {
        $('#category-nav').removeClass('show');
        $('#main-nav').removeClass('show');
        $('#navbar-form').removeClass('show');
        $('.navbar-toggler').addClass('collapsed').attr('aria-expanded','false');
    }

    $('form.validate').each(function () {
        $(this).validate({
            errorElement: "span",
            errorClass: "help-inline",
            highlight: function (input) {
                var $input = $(input);
                var $inputGroup = $input.closest('.control-group').removeClass('valid');


                if ($input.val().length) {
                    $inputGroup.addClass('invalid');
                }  else {
                    $inputGroup.removeClass('invalid');
                }
                $inputGroup.closest('form.validate').removeClass('form-valid').addClass('form-invalid');
            },
            success: function (input) {
                $(input).closest('.control-group').removeClass('invalid').addClass('valid');
                $(input).closest('form.validate').removeClass('form-invalid').addClass('form-valid');
            }
        });
    });

    var $searchForm = $('#search-form');

    $('#site-search').typeahead({
        minLength: 3,
        source: function (query, process) {
            $searchForm.addClass('searching');
            $.uCommerce.search({ keyword: query }, function (resp) {
                $.map(resp, function (data) {
                    var matches = [];
                    for (var i = 0; i < data.length; i++) {
                        matches.push(data[i].ProductName);
                    }
                    return process(matches);
                });
            });
            $searchForm.removeClass('searching');
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
    setupConfidences();

    function setupConfidences()
    {
        if (!$slick_slider.hasClass('slick-initialized') && $(window).width() < 1024) {
            $slick_slider.slick(confidencesSettings);
        }
    }


    if ($('.js-multi-item-carousel').length && $('.js-multi-item-carousel').children().length > 3) {
        $('.js-multi-item-carousel').slick({
            mobileFirst: true,
            slidesToShow: 2,
            slidesToScroll: 2,
            autoplay: true,
            arrows: false,
            dots: true,
            infinite: true,
            responsive: [
                {
                    breakpoint: 760,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3,
                        centerMode: true
                    }
                },
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 4,
                        slidesToScroll: 4,
                        centerMode: false
                    }
                }
            ]
        });
    }

    if ($('.js-slider-for').length) {
        $('.js-slider-for').slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            arrows: false,
            fade: true,
            asNavFor: '.js-slider-nav'
        });
        $('.js-slider-nav').slick({
            slidesToShow: 5,
            slidesToScroll: 1,
            asNavFor: '.js-slider-for',
            dots: false,
            focusOnSelect: true
        });
    }
});

var throttle = function (fn, threshold, scope) {
    var last;
    var deferTimer;
    return function () {
        var context = scope || this;
        var now = Number(new Date());
        var args = arguments;
        if (last && now < last + threshold) {
            // hold on to it
            clearTimeout(deferTimer);
            deferTimer = setTimeout(function () {
                last = now;
                fn.apply(context, args);
            }, threshold + last - now);
        } else {
            last = now;
            fn.apply(context, args);
        }
    };
};

function updateCartTotals() {
    if ($('#empty-basket').length != 0) {
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

        // Pulse the basket so it catches the user's attention
        //var highlight = [qty, sub, tax, disc, tot];
        //$(highlight).effect("highlight", {}, 500);
    });
};
function prepCart() {
    var icn = $("<i>", { "class": "fa fa-shopping-basket icon-white margin-right-small" });
    var qty = $("<span>", { "class": "item-qty" });
    var tot = $("<span>", { "class": "order-total" });
    var basket = $("<a>", { 'href': '/basket', "id": "mini-basket" });

    basket.append(icn).append(qty).append(" items in basket, total: ").append(tot);
    $('#empty-basket').replaceWith(basket);
};
