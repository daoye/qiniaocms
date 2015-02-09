<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Step4.aspx.cs" Inherits="QN.Web.Install.Step4" %>

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
    <title>恭喜，安装完毕！ - 奇鸟CMS</title>
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
        <div class="text-center">
          <h1>安装结束</h1>
        </div>
        <hr />
        <div class="row text-center msg">
            :) 您已经完成安装，现在开始体验吧！
        </div>
        <div class="row" style="margin-top:150px;">
            <div class="alert alert-error" runat="server" id="lblError" visible="false"></div>
        </div>
        <div class="row text-center">
            <a href="<%=homepage %>" class="btn">进入首页</a>
            <a href="<%=accountpage %>" class="btn btn-success" >进入后台管理</a>
        </div>
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
