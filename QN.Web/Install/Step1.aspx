<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="step1.aspx.cs" Inherits="QN.Web.install.step1" %>

<!DOCTYPE html>
<!--[if IE 8]>         <html class="ie8"> <![endif]-->
<!--[if IE 9]>         <html class="ie9 gt-ie8"> <![endif]-->
<!--[if (gte IE 9)|!(IE)]><!-->
<html class="gt-ie8 gt-ie9 not-ie">
<!--<![endif]-->
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0">
    <title>环境检测 - 奇鸟CMS </title>
    <link href="../content/admin/css/bootstrap.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/admin.min.css"" rel="stylesheet" />
    <link href="../content/admin/css/widgets.min.css"" rel="stylesheet" />
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
        <div class="text-center">
          <h1>安装 <small>环境检测</small></h1>
        </div>
        <hr />
        <div class="row">
            <div class="span12">
                <table class="table">
                    <thead>
                        <tr>
                            <th style="width:200px;">目录</th>
                            <th>描述</th>
                            <th style="width:60px;">读写</th>
                        </tr>
                    </thead>
                    <asp:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                          <tr>
                              <td><%#Eval("Path") %></td>
                              <td><%#Eval("Description") %></td>
                              <td><img src='<%#Eval("Icon") %>' /></td>
                          </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
        <div class="row text-center">
            <div class="span12">
                <a href="Index.aspx" class="btn">上一步</a>
                <a href="javascript:location.reload();" class="btn btn-success" runat="server" id="btnNext">刷新</a>
            </div>
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
