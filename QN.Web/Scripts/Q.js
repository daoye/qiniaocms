(function (window, $, undefined) {

    var Q = window.Q = window.q = function () { };

    //显示调试日志
    Q.log = function (msg) {
        if (typeof (console) != undefined) {
            console.log(msg);
        }
    }

    /**************************************************
    *  name            :   Format
    *  description     :   格式化字符串。
    *  @_str           :   即将被格式化的字符串，以“{数字}”的形式作为通配符。
    *  @return         :   被格式化后的字符串。
    /**************************************************/
    Q.format = function (_str) {
        var pattern = '';

        _str = _str || '';

        for (var i = 1, len = arguments.length; i < len; i++) {
            pattern = new RegExp('\\{' + (i - 1) + '\\}', 'g');

            _str = _str.replace(pattern, arguments[i]);
        }

        return _str;
    }

    Q.guid = function (isSpliter) {
        if (typeof isSpliter === 'undefined') {
            isSpliter = true;
        }

        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                guid += isSpliter ? "-" : "";
        }
        return guid;
    }

    //页面加载完毕后会被调用的方法列表
    Q.runMethod = [];
    Q.run = function () {
        for (var i = 0; i < Q.runMethod.length; i++) {
            if ($.isFunction(Q.runMethod[i])) {
                try {
                    Q.runMethod[i]();
                }
                catch (e) {
                    Q.log(e.message);
                }
            }
        }
    };

    //ui命名空间
    Q.ui = {};


    //自动绑定验证码
    Q.ui.captcha = function () {
        var codeurl = basepath + 'Common/Code.aspx?time=';
        var imgcaptcha=   $('img[class*="captcha"]');
        
        imgcaptcha.click(function () {
            this.src = codeurl + Date.parse(new Date());
        });

        imgcaptcha.attr('src', codeurl + Date.parse(new Date()));
    }
    //加入启动执行列表
    Q.runMethod.push(Q.ui.captcha);

    //编辑器
    Q.editor = function (el, opt) {
        opt = $.extend(true, {}, opt);
        var ue = UE.getEditor(el, opt);

        return ue;
    }

    $(function () {
        Q.run();
    });
}(window, $));