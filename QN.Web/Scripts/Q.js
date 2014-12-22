(function (window, $, undefined) {

    var Q = window.Q = window.q = function () { };

    //显示调试日志
    Q.log = function (msg) {
        if (typeof (console) != undefined) {
            console.log(msg);
        }
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
        var codeurl = basePath + 'Common/Code.aspx?time=';
        var imgcaptcha=   $('img[class*="captcha"]');
        
        imgcaptcha.click(function () {
            this.src = codeurl + Date.parse(new Date());
        });

        imgcaptcha.attr('src', codeurl + Date.parse(new Date()));
    }
    //加入启动执行列表
    Q.runMethod.push(Q.ui.captcha);



    $(function () {
        Q.run();
    });
}(window, $));