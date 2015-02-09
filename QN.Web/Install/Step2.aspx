<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Step2.aspx.cs" Inherits="QN.Web.Install.Step2" %>

<!DOCTYPE html>
<!--[if IE 8]>         <html class="ie8"> <![endif]-->
<!--[if IE 9]>         <html class="ie9 gt-ie8"> <![endif]-->
<!--[if gt IE 9]><!-->
<html class="gt-ie8 gt-ie9 not-ie">
<!--<![endif]-->
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0">
    <title>数据库连接测试 - 奇鸟CMS</title>
    <link href="../content/admin/css/bootstrap.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/admin.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/pages.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/rtl.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/themes.min.css"" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <!--[if lt IE 9]>
        <script src="../Scripts/fuckIE/respond.min.js"></script>
        <script src="../Scripts/fuckIE/html5shiv.min.js"></script>
        <script src="../Scripts/fuckIE/selectivizr-min.js"></script>
    <![endif]-->
    <script src="../Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../Content/admin/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../Content/admin/js/admin.min.js" type="text/javascript"></script>
</head>
<body>
    <div class="container">
        <div class="install-content">
            <form class="form-horizontal" id="form1" method="post">
            <div class="text-center">
                <h1>
                    安装 <small>数据库配置</small></h1>
            </div>
            <hr />
            <div class="row">
                <div style="width: 600px; margin: 20px auto;">
                    <div class="control-group">
                        <label class="control-label" for="DbType">
                            数据库类型</label>
                        <div class="controls">
                            <select name="DbType" id="DbType" class="form-control">
                                <option value="SQLITE" selected>SQLite</option>
                            </select>
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="DbPort">
                            数据库名称</label>
                        <div class="controls">
                            <input type="text" name="DbName"  class="form-control" id="DbName" value="<%=Request.Form["DbName"] ?? "qiniaocms" %>" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="alert alert-error" runat="server" id="lblError" visible="false">
                </div>
            </div>
            <div class="row text-center">
                <a href="Step1.aspx" class="btn">上一步</a>
                <input type="submit" class="btn btn-success" value="下一步" />
            </div>
            </form>
        </div>
        <div class="row footer">
            <div class="span12  text-center">
                <hr />
                Copyright &#169; 2012-2014 <a href="http://www.qiniaosoft.com/" target="_blank">QiniaoSoft
                    Ltd</a>.
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function changeDisable(dbType) {
            if ('SQLITE' == dbType) {
                $('#DbIP').attr('disabled', 'disabled');
                //$('#DbName').attr('disabled', 'disabled');
                $('#DbPort').attr('disabled', 'disabled');
                $('#DbUser').attr('disabled', 'disabled');
                $('#DbPwd').attr('disabled', 'disabled');

            } else {
                $('#DbIP').removeAttr('disabled');
                $('#DbName').removeAttr('disabled');
                $('#DbPort').removeAttr('disabled');
                $('#DbUser').removeAttr('disabled');
                $('#DbPwd').removeAttr('disabled');
            }
        }
        $('#DbType').change(function () {
            var me = $(this),
                dbType = me.val();

            changeDisable(dbType);
        });

        $(function () {
            changeDisable($('#DbType').val());
        });

        (function () {
            var wheight = $(window).height() - 90,
                cheight = $('.install-content').height();

            if (cheight < wheight) {
                $('.install-content').css('height', wheight + 'px');
            }

        } ());
    </script>
</body>
</html>
