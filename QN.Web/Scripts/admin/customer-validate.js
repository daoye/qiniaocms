
(function ($) {
    var $jQval = $.validator;

    $.validator.setDefaults({
        highlight: function (element) {
            return $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            return $(element).closest('.form-group').removeClass('has-error').find('help-block-hidden').removeClass('help-block-hidden').addClass('help-block').show();
        },
        errorElement: 'span',
        errorClass: 'jquery-validate-error',
        errorPlacement: function (error, element) {
            alert(1);
            var container = $(this).find("[data-valmsg-for='" + escapeAttributeValue(inputElement[0].name) + "']"),
                replaceAttrValue = container.attr("data-valmsg-replace"),
                replace = replaceAttrValue ? $.parseJSON(replaceAttrValue) !== false : null;

            container.removeClass("field-validation-valid").addClass("field-validation-error help-block");
            error.data("unobtrusiveContainer", container);

            if (replace) {
                container.empty();
                error.parents('div.form-group').addClass('has-error');
                error.removeClass("input-validation-error").appendTo(container);
            }
            else {
                error.hide();
            }
        }
    });


    $jQval.addMethod('qrequired', function (value, element, params) {
        return $jQval.methods.required(value, element, params);
    });

    $jQval.unobtrusive.adapters.add('qrequired', function (options) {
        if (options.element.tagName.toUpperCase() !== "INPUT" || options.element.type.toUpperCase() !== "CHECKBOX") {
            var ruleName = "required";

            options.rules[ruleName] = true;
            if (options.message) {
                options.messages[ruleName] = options.message;
            }
        }
    });
}(jQuery));