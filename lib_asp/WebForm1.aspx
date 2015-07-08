<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="lib_asp.WebForm1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
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
            <script type="text/javascript">
                function pageLoad() {
                    var chngPosition = $find('CTB1_DropDownExtender')._dropPopupPopupBehavior;
                    chngPosition.set_positioningMode(2);

                }

            </script>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div style="height: 200px; width: 300px; overflow: auto;">

                <asp:TextBox ID="CTB1" runat="server" ReadOnly="true"></asp:TextBox>
                <asp:Panel ID="ComboTreeBox1" runat="server">
                    <asp:TreeView ID="nodenod" runat="server" ShowLines="true" ShowCheckBoxes="All" OnTreeNodeCheckChanged="nodenod_TreeNodeCheckChanged">
                    </asp:TreeView>
                    <asp:Button ID="Button1" Text="Submit" runat="server" OnClick="Submit" /><br />
                    <asp:TextBox ID="txtData" runat="server"></asp:TextBox>
                </asp:Panel>
            </div>
            <br />
            <cc1:DropDownExtender ID="DropDownExtender1" runat="server" TargetControlID="CTB1" DropDownControlID="ComboTreeBox1">
            </cc1:DropDownExtender>
        </div>


        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <p></p>
        <h4>Filter:</h4>

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
    <h4>Submitted:</h4>
    <output id="result"></output>
    <h4>Filtered by:</h4>
    <output id="filter"></output>
</body>
</html>
