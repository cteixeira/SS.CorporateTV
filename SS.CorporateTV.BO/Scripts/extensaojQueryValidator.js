(function ($) {
    var formatoData = "DD-MM-YYYY HH:MM:SS";
    var debug = false;

    $.validator.unobtrusive.adapters.addSingleVal("timegreaterthan", "enddate");
    $.validator.addMethod("timegreaterthan", function (value, element) {
        var otherDateSelector = $(element).attr('data-val-timegreaterthan-enddate');
        var otherDate = $("#" + otherDateSelector + " option:selected");

        if (!value || !otherDate.text()) return true;

        value = value.replace(/-/g, '/');

        var dataFim = Date.parse('01/01/2011 ' + value + ':00');
        var dataInicio = Date.parse('01/01/2011 ' + otherDate.text() + ':00');

        var isValid = !(dataFim <= dataInicio);

        if (debug) {
            console.log("data inicio " + Date.parse('01/01/2011 ' + otherDate.text() + ':00') + " data fim " + Date.parse('01/01/2011 ' + value + ':00'));
            console.log(!(dataFim < dataInicio));

            if (!isValid)
                Notifica($(element).attr('data-val-timegreaterthan'), "alert");
        }
        return isValid;
    });
})(jQuery);