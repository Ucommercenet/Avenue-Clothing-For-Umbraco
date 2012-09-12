$(function () {
    wireupVariationOptions($("#product-sku"), $("#selected-size"), $("#selected-colour"), $("#add-to-basket"));
});
function wireupVariationOptions(productId, size, colour, addToCartButton) {
    addToCartButton.removeClass('btn-success').addClass('disabled');
    updateVariationOptions(productId, size, colour);
    size.change(function () {
        // Reset the colour options
        $('option:first', colour).attr('selected', true);
        updateVariationOptions(productId, size, colour, function () {
            updateAddToCartButton(size, colour, addToCartButton);
        });
    });
    colour.change(function () {
        updateVariationOptions(productId, size, colour, function () {
            updateAddToCartButton(size, colour, addToCartButton);
        });
    });
};
function updateAddToCartButton(size, colour, addToCartButton) {
    console.log(size.val());
    console.log(colour.val());
    // If the user has made a valid selection enable the add to cart button
    if (colour.val() != '' && size.val() != '') {
        addToCartButton.removeClass('disabled').addClass('btn-success').removeAttr('disabled');
    } else {
        addToCartButton.removeClass('btn-success').addClass('disabled').attr('disabled', 'disabled');
    }
};
function updateVariationOptions(productId, size, colour, success, failure) {
    var selectedSize = size.val();
    var colourOptions = $('option', colour).not('option[value=""]');

    // Start by disabling all the options
    colourOptions.attr('disabled', 'disabled');

    $.uCommerce.getProductVariations(
            { productSku: productId.val() },
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
                    console.log("Updated Select");
                }

                // Now call any functions that need to run after the updates
                if(success)
                    success();
            }
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