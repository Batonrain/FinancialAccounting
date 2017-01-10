$(document).ready(function () {
    var dialog, form;

    var allFields = $([]).add(PaymentType).add(Summ);

    function addPayment() {

        return true;
    }

    //$("#PaymentType").selectmenu();

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
    });
});