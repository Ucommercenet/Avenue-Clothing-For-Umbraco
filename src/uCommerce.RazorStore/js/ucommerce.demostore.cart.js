$(function () {
    watchForQuantityChange($('#cart'));
});
function watchForQuantityChange(cart) {
    $('.update-cart', cart).hide();
    disableUpdateButton(cart);
    $('.qty', cart).change(function () {
        var t = $(this);
        if (t.val() == t.data('original')) {
            return;
        }
        $('.update-cart', t.parent()).fadeIn();
        enableUpdateButton(cart);
    });
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
            updateLineTotals(t.parents('tr'), updatedLine.UpdatedLine);
        });
        return false;
    });
    $('.qty', cart).each(function () {
        var t = $(this);
        t.data('original', t.val());
    });
};
function updateLineTotals(row, line) {
    $('.line-total', row).text(line.Total.toFixed(2));
};
function enableUpdateButton(cart) {
    $('.update-basket', cart).addClass('btn-success').removeClass('disabled').removeAttr('disabled');
    $('.update-basket i', cart).addClass('icon-white');
};
function disableUpdateButton(cart) {
    $('.update-basket', cart).removeClass('btn-success').addClass('disabled').attr('disabled', 'disabled');
    $('.update-basket i', cart).removeClass('icon-white');
};