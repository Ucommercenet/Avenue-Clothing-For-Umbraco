var _itemAddedAlert = null;

$(function () {
	relateVariations($('#product-sku'), $('#variation-collarsize'), $('#variation-colour'), $('#add-to-basket'));
	enableAddToCartWhenSelected($('#add-to-basket'), $('.variant'));
	wireupAddToCartButton($('#add-to-basket'), $('#catalog-id'), $('#product-sku'), $('.variant'), $('#quantity-to-add'));
	wireupRatings($('.rating'));

});

function relateVariations(sku, size, colour) {
	updateVariationOptions(sku, size, colour, true);
	size.change(function () {
		// Reset the colour options
		$('option:first', colour).attr('selected', true);
		updateVariationOptions(sku, size, colour, true);
	});
	colour.change(function () {
		updateVariationOptions(sku, size, colour, true);
	});
};
function enableAddToCartWhenSelected(addToCartButton, variantInputs) {
	if (variantInputs.length == 0)
		return;

	addToCartButton.removeClass('btn-success').addClass('disabled');

	variantInputs.change(function () {
		updateAddToCartButton(addToCartButton, variantInputs);
	});
};

function wireupRatings(radios) {
    $('#review-form').addClass("display-none");
    $('label', radios).each(function () {
        var t = $(this);
        t.addClass('off');
        $('input:radio', t).addClass("display-none");
        setStarHoverOutState($('i', t));
        t.hover(function () {
            var parent = $(this);
            var labels = parent.prevAll('label');
            setStarHoverState($('i', labels));
            setStarHoverState($('i', parent));
        }, function () {
            var parent = $(this);
            var labels = parent.prevAll('label');
            if (!parent.hasClass('selected')) {
                setStarHoverOutState($('i', labels));
                setStarHoverOutState($('i', parent));
            }
        });
        t.click(function () {
            var parent = $(this);
            parent.addClass('selected');
            $('#review-form').slideDown();
        });
    });
};


function updateAddToCartButton(addToCartButton, variantInputs) {
	if (variantInputs.length == 0)
		return;

	var empty = variantInputs.filter(function () { return this.value === ""; });

	// If the user has made a valid selection enable the add to cart button
	if (empty.length == 0) {
		addToCartButton.removeClass('disabled').addClass('btn-success').removeAttr('disabled');
	} else {
		addToCartButton.removeClass('btn-success').addClass('disabled').attr('disabled', 'disabled');
	}
};

function setStarHoverState(label) {
	label.addClass('fa-star').removeClass('fa-star-o');
}
function setStarHoverOutState(label) {
	label.addClass('fa-star-o').removeClass('fa-star');
}
function wireupAddToCartButton(addToCartButton, catalogIdInput, skuInput, variantInputs, quantityInput) {
	addToCartButton.click(function (e) {
		e.preventDefault();

		var sku = skuInput.val();
		var myarray = {};
		variantInputs.each(function (i, v) {
			var t = $(v);
			myarray[t.attr("id").replace("variation-", "")] = t.val();
		});
		var qty = quantityInput.val();

		$.uCommerce.getVariantSkuFromSelection(
            {
            	productSku: sku,
            	variantProperties: myarray
            },
            function (data) {
            	var variant = data.Variant;
                if(variant != null){
            	$.uCommerce.addToBasket(
                    {
                        catalogId: catalogIdInput.val(),
                    	sku: variant.Sku,
                    	variantSku: variant.VariantSku,
                    	quantity: qty
                    },
                    function () {
                    	updateCartTotals(addToCartButton);

                    	var parent = addToCartButton.parent();
                    	var alert = parent.find(".item-added");
                    	if (alert.length == 0) {
                    		// Add an alert box so the customer knows they've added an item to the cart
                    		alert = $('<div />', {
                    			"class": "alert alert-success item-added",
                    			html: '<button type="button" class="close" data-dismiss="alert">×</button><p>Thanks, this item has been added to your cart. <a href="/basket">Click here to view your cart</a>.</p>'
                    		}).hide();
                    		parent.append(alert);
                    		alert.slideDown();
                    	} else {
                    		//alert.effect("highlight", { color: '#FCF8E3' }, 500);
                    	}

                    	// Incase there's already a timeout in place, clear it
                    	clearTimeout(_itemAddedAlert);

                    	// Remove the alert after 5 seconds
                    	_itemAddedAlert = setTimeout(function () {
                    		alert.slideUp(500, function () {
                    			alert.remove();
                    		});
                    	}, 5000);
                    }
                );
                }
            });
		return false;
	});
};
function updateVariationOptions(sku, size, colour, userAction, success, failure) {
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

            	if (selectedSize != '' && availableOptions.length == 1 && userAction) {
            		colour.val(availableOptions.val());
            		//Fire these events manually to prevent a loop
            		updateVariationOptions(sku, size, colour, false);
            		updateAddToCartButton($('#add-to-basket'), $('.variant'));
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