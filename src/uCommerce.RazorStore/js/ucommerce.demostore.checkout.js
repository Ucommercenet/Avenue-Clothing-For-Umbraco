$(function () {
    $('form.validate').validate({
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