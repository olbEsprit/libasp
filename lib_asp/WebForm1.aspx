﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="lib_asp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>


    <form id="form1" runat="server">
        <div>
            <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
            <link href="http://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/css/bootstrap.min.css"
                rel="stylesheet" type="text/css" />
            <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js"></script>
            <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css"
                rel="stylesheet" type="text/css" />
            <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js"
                type="text/javascript"></script>

            <asp:TreeView ID="nodenod" runat="server" ShowLines="true" ShowCheckBoxes="All" OnTreeNodeCheckChanged="nodenod_TreeNodeCheckChanged">
            </asp:TreeView>
            <br />

        </div>

        <asp:Button ID="Button1" Text="Submit" runat="server" OnClick="Submit" />

        <p> </p>
        <asp:TextBox ID="txtData" runat="server"></asp:TextBox>
    </form>

    <script type="text/javascript">
        $.ajax({
            url: 'WebForm1.aspx/GetServerData',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                $('#result').text(response.d);
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });

        $.ajax({
            url: 'WebForm1.aspx/GetCurrentFilter',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                $('#filter').text(response.d);
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    </script>



    <br />
    <p><output id="result"></output></p>
    <p><output id="filter"></output></p>
</body>
</html>
