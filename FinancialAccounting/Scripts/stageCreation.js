$(document).ready(function () {
    $.datepicker.setDefaults({
        dateFormat: 'dd.mm.yy',
        changeMonth: true,
        changeYear: true,
        yearRange: '1900:2100',
        defaultDate: ''
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

function SetDataToPicker(prepaymentDay, finalPaymentDay, dateOfEnding) {
    $('#DateOfPrepaymentPicker').datepicker("setDate", prepaymentDay);
    $('#DateOfFinalPaymentPicker').datepicker("setDate", finalPaymentDay);
    $('#DateOfEndingPicker').datepicker("setDate", dateOfEnding);
    
    $('#DateOfPrepaymentPicker').value = prepaymentDay;
    $('#DateOfFinalPaymentPicker').value = finalPaymentDay;
    $('#DateOfEndingPicker').value = dateOfEnding;
}

