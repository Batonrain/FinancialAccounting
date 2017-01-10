$(document).ready(function () {
    $.datepicker.setDefaults({
        dateFormat: 'dd.mm.yy',
        changeMonth: true,
        changeYear: true,
        yearRange: '1900:2100',
        defaultDate: '',
        minDate: 0,
    });

    $('#DateOfPrepaymentPicker').datepicker();
    $('#DateOfFinalPaymentPicker').datepicker();
    $('#DateOfEndingPicker').datepicker();

    $('#DateOfPrepaymentPicker').datepicker("setDate", null);
    $('#DateOfFinalPaymentPicker').datepicker("setDate", null);
    $('#DateOfEndingPicker').datepicker("setDate", null);

    $('#DateOfPrepaymentPicker').value = '';
    $('#DateOfFinalPaymentPicker').value = '';
    $('#DateOfEndingPicker').value = '';
});