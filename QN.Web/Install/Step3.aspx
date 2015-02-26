<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="step3.aspx.cs" Inherits="QN.Web.install.step3" %>

<!DOCTYPE html>
<html>
<!--[if IE 8]>         <html class="ie8"> <![endif]-->
<!--[if IE 9]>         <html class="ie9 gt-ie8"> <![endif]-->
<!--[if gt IE 9]>
<html class="gt-ie8 gt-ie9 not-ie">
<![endif]-->
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0">
    <title>创建管理员账号 - 奇鸟CMS</title>
    <link href="../content/admin/css/bootstrap.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/admin.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/pages.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/rtl.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/themes.min.css"" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <!--[if lt IE 9]>
        <script src="../scripts/fuckIE/respond.min.js"></script>
        <script src="../scripts/fuckIE/html5shiv.min.js"></script>
        <script src="../scripts/fuckIE/selectivizr-min.js"></script>
    <![endif]-->
    <script src="../scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../content/admin/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../content/admin/js/admin.min.js" type="text/javascript"></script>
</head>
<body>
    <div class="container">
        <div class="install-content">
            <form class="form-horizontal" id="form1" method="post">
            <div class="text-center">
                <h1>安装 <small>创建管理员账号</small></h1>
            </div>
            <hr />
            <div class="row">
                <div style="width: 600px; margin: 20px auto;">
                    <div class="control-group">
                        <label class="control-label" for="LoginName">
                            账号</label>
                        <div class="controls">
                            <input type="text" name="LoginName" class="form-control" id="LoginName" value="<%=Request.Form["LoginName"] %>" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="Email">
                            昵称</label>
                        <div class="controls">
                            <input type="text" name="NiceName" class="form-control" id="NiceName" value="<%=Request.Form["NiceName"] %>" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="Email">
                            邮箱</label>
                        <div class="controls">
                            <input type="text" name="Email" class="form-control" id="Email" value="<%=Request.Form["Email"] %>" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="Password">
                            密码</label>
                        <div class="controls">
                            <input type="password" name="Password" class="form-control" id="Password" value="" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="LoginName">
                            网站地址</label>
                        <div class="controls">
                            <input type="text" name="SiteDomain" class="form-control" id="SiteDomain" value="<%=Request.Form["SiteDomain"] ?? RealUrl %>" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="SiteName">
                            网站名称</label>
                        <div class="controls">
                            <input type="text" name="SiteName" class="form-control" id="SiteName" value="<%=Request.Form["SiteName"] %>" />
                        </div>
                    </div>
                    <div class="control-group">
                        <label class="control-label" for="SiteInfo">
                            网站简介</label>
                        <div class="controls">
                            <textarea name="SiteInfo" id="SiteInfo" class="form-control" cols="15" rows="2"><%=Request.Form["SiteInfo"] %></textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="alert alert-error" runat="server" id="lblError" visible="false"></div>
            </div>
            <div class="row text-center">
                <a href="Step2.aspx" class="btn">上一步</a>
                <input type="submit" class="btn btn-success" value="下一步" />
            </div>
            </form>
        </div>
        <div class="row footer">
            <div class="span12  text-center">
                <hr />
                Copyright &#169; 2012-2014 <a href="http://www.qiniaosoft.com/" target="_blank">QiniaoSoft Ltd</a>.
            </div>
        </div>
    </div>
    <script type="text/javascript">
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
