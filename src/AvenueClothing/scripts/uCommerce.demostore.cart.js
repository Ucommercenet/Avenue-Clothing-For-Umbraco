$(function () {
    wireUpCart($('#basket'));
});
function wireUpCart(basket) {
    wireupQuantityChange(basket);
    wireUpClickHandlers(basket);
};
function wireupQuantityChange(basket) {
    $('tr.order-line', basket).each(function (i, row) {
        addUpdateButton(basket, row);
    });
};
function addUpdateButton(basket, row) {
    var qty = $('.qty', row);
    var lineId = qty.data("orderlineid");
    qty.data('original', qty.val());

    var div = $('<div />', {
        "class": "input-append"
    });
    qty.wrap(div);

    var btn = $('<button />', {
        name: "update-basket-line",
        "class": "btn btn-success update-basket",
        "type": "submit",
        value: lineId
    }).hide();

    $("<i />", {
        "class": "icon-refresh icon-white"
    }).appendTo(btn);

    btn.insertAfter(qty);
    watchForQuantityChange(qty, basket);
};
function watchForQuantityChange(qty, basket) {
    qty.change(function () {
        var t = $(this);
        if (t.val() == t.data('original')) {
            return;
        }
        $('.update-basket', t.parent()).fadeIn();
        enableUpdateButton(basket);
    });
};
function wireUpClickHandlers(basket) {
    disableUpdateButton(basket);
    $('.update-basket', basket).click(function (e) {
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
            if ($('.pending-change', basket).length > 0) {
                enableUpdateButton(basket);
            } else {
                disableUpdateButton(basket);
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
function enableUpdateButton(basket) {
    $('.update-basket', basket).addClass('btn-success').removeClass('disabled').removeAttr('disabled');
    $('.update-basket i', basket).addClass('icon-white');
};
function disableUpdateButton(basket) {
    $('.update-basket', basket).removeClass('btn-success').addClass('disabled').attr('disabled', 'disabled');
    $('.update-basket i', basket).removeClass('icon-white');
};