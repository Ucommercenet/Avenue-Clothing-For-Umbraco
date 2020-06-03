(function ($) {
    var $billingViewCheckbox = $('#toggleBillingView');
    var $addressForm = $('#addressForm');
    var $billingAddress = $('#billingAddress');


    $billingViewCheckbox.click(function () {
        $billingAddress.toggleClass('collapse');
    });

    $addressForm.on('submit', function(){
        if (!$billingViewCheckbox.is(':checked')) {
            var $fieldsToMirror = $addressForm.find('[data-mirror]');

            $fieldsToMirror.each(function(){
                var fieldName = $(this).data('mirror');
                $billingAddress.find('#BillingAddress_' + fieldName).val($(this).val());
            });
        }
    });
})(jQuery);