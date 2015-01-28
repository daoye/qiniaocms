
(function ($) {
    var $jQval = $.validator;

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