$(function () {
    wireUpCart($('#cart'));
});
function wireUpCart(cart) {
    wireupQuantityChange(cart);
    wireUpClickHandlers(cart);
};
function wireupQuantityChange(cart) {
    $('tr.order-line', cart).each(function (i, row) {
        addUpdateButton(cart, row);
    });
};
function addUpdateButton(cart, row) {
    var qty = $('.qty', row);
    var lineId = qty.data("orderlineid");
    qty.data('original', qty.val());

    var div = $('<div />', {
        "class": "input-append"
    });
    qty.wrap(div);
   
    var btn = $('<button />', {
        name: "update-basket-line",
        "class": "btn btn-success update-cart",
        "type": "submit",
        value: lineId
    }).hide();

    $("<i />", {
        "class": "icon-refresh icon-white"
    }).appendTo(btn);

    btn.insertAfter(qty);
    watchForQuantityChange(qty, cart);
};
function watchForQuantityChange(qty, cart) {
    qty.change(function () {
        var t = $(this);
        if (t.val() == t.data('original')) {
            return;
        }
        $('.update-cart', t.parent()).fadeIn();
        enableUpdateButton(cart);
    });
};
function wireUpClickHandlers(cart) {
    disableUpdateButton(cart);
    $('.update-cart', cart).click(function (e) {
        e.preventDefault();
        var t = $(this);
        var qty = $('.qty', t.parent());
        qty.addClass('pending-change');
        $.uCommerce.updateLineItem({
            orderLineId: t.val(),
            newQuantity: qty.val()
        },
        function (updatedLine) {
            updateCartTotals();
            t.fadeOut();
            qty.removeClass('pending-change');
            var row = t.parents('tr');
            updateLineTotals(row, updatedLine.UpdatedLine);
            updateQuantity(row, updatedLine.UpdatedLine);
            if ($('.pending-change', cart).length > 0) {
                enableUpdateButton(cart);
            } else {
                disableUpdateButton(cart);
            }
        });
        return false;
    });
};
function updateLineTotals(row, line) {
    $('.line-total', row).text(line.FormattedTotal);
};
function updateQuantity(row, line) {
    $('.qty', row).data('original', line.Quantity);
};
function enableUpdateButton(cart) {
    $('.update-basket', cart).addClass('btn-success').removeClass('disabled').removeAttr('disabled');
    $('.update-basket i', cart).addClass('icon-white');
};
function disableUpdateButton(cart) {
    $('.update-basket', cart).removeClass('btn-success').addClass('disabled').attr('disabled', 'disabled');
    $('.update-basket i', cart).removeClass('icon-white');
};