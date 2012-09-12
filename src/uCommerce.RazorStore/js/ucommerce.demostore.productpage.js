var _minicart = null;
$(function () {
    _minicart = $('#mini-cart');
    wireupVariationOptions($('#product-sku'), $('#selected-size'), $('#selected-colour'), $('#add-to-basket'));
    wireupAddToCartButton($('#add-to-basket'), $('#product-sku'), $('#selected-size'), $('#selected-colour'), $('#quantity-to-add'));
});
function wireupVariationOptions(sku, size, colour, addToCartButton) {
    addToCartButton.removeClass('btn-success').addClass('disabled');
    updateVariationOptions(sku, size, colour);
    size.change(function () {
        // Reset the colour options
        $('option:first', colour).attr('selected', true);
        updateVariationOptions(sku, size, colour, function () {
            updateAddToCartButton(size, colour, addToCartButton);
        });
    });
    colour.change(function () {
        updateVariationOptions(sku, size, colour, function () {
            updateAddToCartButton(size, colour, addToCartButton);
        });
    });
};
function wireupAddToCartButton(addToCartButton, skuInput, sizeInput, colourInput, quantityInput) {
    addToCartButton.click(function (e) {
        e.preventDefault();

        var sku = skuInput.val();
        var size = sizeInput.val();
        var colour = colourInput.val();
        var qty = quantityInput.val();

        $.uCommerce.getVariantSkuFromSelection(
            {
                productSku: sku,
                size: size,
                colour: colour
            },
            function (data) {
                var variant = data.Variant;
                $.uCommerce.addToBasket(
                    {
                        sku: variant.Sku,
                        variantSku: variant.VariantSku,
                        quantity: qty
                    },
                    function () {
                        updateCartTotals(addToCartButton);
                    }
                );
            });
        return false;
    });
};
function updateCartTotals(addToCartButton) {
    if (_minicart == null) {
        return;
    }
    $.uCommerce.getBasket({}, function (response) {
        var basket = response.Basket;
        var qty = $(".cart-qty", _minicart);
        var sub = $(".sub-total", _minicart);
        qty.text(basket.TotalItems);
        sub.text(basket.OrderTotal);

        _minicart.effect("highlight", {}, 3000);
        var alert = $('<div />', {
            "class": "alert alert-success",
            html: 'Thanks, this item has been added to your cart. <a href="/shop/checkout/cart.aspx">Click here to view your cart</a>.'
        });
        addToCartButton.parent().append(alert);
        setTimeout(function () {
            alert.remove();
        }, 3000);
    });
};
function updateAddToCartButton(size, colour, addToCartButton) {
    // If the user has made a valid selection enable the add to cart button
    if (colour.val() != '' && size.val() != '') {
        addToCartButton.removeClass('disabled').addClass('btn-success').removeAttr('disabled');
    } else {
        addToCartButton.removeClass('btn-success').addClass('disabled').attr('disabled', 'disabled');
    }
};
function updateVariationOptions(sku, size, colour, success, failure) {
    var selectedSize = size.val();
    var colourOptions = $('option', colour).not('option[value=""]');

    // Start by disabling all the options
    colourOptions.attr('disabled', 'disabled');

    $.uCommerce.getProductVariations(
            { productSku: sku.val() },
            function (data) {
                $.each(data.Variations, function (i, v) {
                    var variationSize = getObjectsByKey(v.Properties, 'Name', 'CollarSize')[0];
                    var variationColour = getObjectsByKey(v.Properties, 'Name', 'Colour')[0];

                    // The user hasn't yet selected a size so offer them all colours (so they can see the list)
                    if (selectedSize == '') {
                        colourOptions.removeAttr('disabled');
                        return true;
                    }

                    // If the size matches the selected size, this colour is available so enable it in the drop down list
                    if (variationSize.Value == selectedSize) {
                        $('option[value="' + variationColour.Value + '"]', colour).removeAttr('disabled');
                    }
                });

                // If there is only a single item in the list then select it
                var availableOptions = $('option', colour).not('option[value=""]').not(':disabled');
                if (selectedSize != '' && availableOptions.size() == 1) {
                    colour.val(availableOptions.val());
                }



                // Now call any functions that need to run after the updates
                if (success)
                    success();
            },
            failure
        );
}
function getObjectsByKey(obj, key, val) {
    var objects = [];
    for (var i in obj) {
        if (!obj.hasOwnProperty(i)) continue;
        if (typeof obj[i] == 'object') {
            objects = objects.concat(getObjectsByKey(obj[i], key, val));
        } else if (i == key && obj[key] == val) {
            objects.push(obj);
        }
    }
    return objects;
}