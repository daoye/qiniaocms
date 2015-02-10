(function (window) {

    var _format = function (_str,paras) {
        var pattern = '';

        _str = _str || '';

        for (var i = 1, len = paras.length; i < len; i++) {
            pattern = new RegExp('\\{' + (i - 1) + '\\}', 'g');

            _str = _str.replace(pattern, paras[i]);
        }

        return _str;
    }


    var la = window.la = function (key, paras) {
        if (typeof (lang) == 'undefined') {
            return key;
        }

        for (var i = 0; i < lang.length; i++) {
            if (lang[i]["id"] === key) {
                if (typeof (paras) != 'undefined') {
                    return _format(lang[i]["value"], paras);
                }
                else {
                    return lang[i]["value"];
                }
            }
        }

        return key;
    };
}(window));