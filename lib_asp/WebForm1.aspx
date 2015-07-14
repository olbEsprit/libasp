<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="lib_asp.WebForm1" %>
<%@ Register TagPrefix="customControl" Assembly="lib_asp" Namespace="lib_asp.Lib"  %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TreeView</title>
</head>
<body>

    <div id="showmenu">
        <h4>Submitted:</h4>
        <output id="result"></output>
    </div>
    <form id="form1" runat="server">

        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
        <link href="http://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/css/bootstrap.min.css"
            rel="stylesheet" type="text/css" />
        <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js"></script>
        <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css"
            rel="stylesheet" type="text/css" />
        <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js"
            type="text/javascript"></script>
        <div class="menu" style="display: none;">
            <asp:TreeView ID="nodenod" runat="server" ShowLines="true" ShowCheckBoxes="All" OnTreeNodeCheckChanged="nodenod_TreeNodeCheckChanged">
            </asp:TreeView>


            <br />

            <h4>Filtered by:</h4>
            <output id="filter"></output>
            <asp:Button ID="Button1" Text="Submit" runat="server" OnClick="Submit" />

            <p></p>
            <h4>Filter:</h4>
            <asp:TextBox ID="txtData" runat="server"></asp:TextBox>






        </div>

        <customControl:TreeViewExt ID="sampleTree" runat="server" onselectednodechanged="sampleTree_SelectedNodeChanged">

        </customControl:TreeViewExt>
        <asp:Label ID="lblSelectedNode" runat="server" ></asp:Label>
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

        $(document).ready(function () {
            $('#showmenu').click(function () {
                $('.menu').slideToggle("fast");
            });
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




</body>
</html>
