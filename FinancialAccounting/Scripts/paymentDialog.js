$(function () {
    var dialog, form, stageId;

    var allFields = $([]).add(PaymentType).add(Summ);

    dialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 400,
        width: 350,
        modal: true,
        buttons: {
            "Провести платёж": addPayment,
            "Отмена": function () {
                dialog.dialog("close");
            }
        },
        close: function () {
            form[0].reset();
            allFields.removeClass("ui-state-error");
        }
    });

    form = dialog.find("form").on("submit", function (event) {
        event.preventDefault();
        addPayment();
    });

    $("#create-payment").button().on("click", function () {
        dialog.dialog("open");
        stageId = $(this).data('stageid');
    });

    function addPayment() {
        var requestData = {
            StageId: stageId,
            PaymentSum: $.trim($("#Summ").val()),
            PaymentType: $("#PaymentType").val()
        };
        $.ajax({
            url: '/Stages/AddPayment',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(requestData),
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            },
            success: function () {
                dialog.dialog("close");
                //reload page
            },
            async: true,
            processData: false
        });
        return true;
    }
});