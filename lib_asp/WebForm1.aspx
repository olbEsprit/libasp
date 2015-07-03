<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="lib_asp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:TreeView ID="nodenod" runat="server" ShowLines="true" ShowCheckBoxes="All" OnTreeNodeCheckChanged="nodenod_TreeNodeCheckChanged">
                <Nodes>
                    <asp:TreeNode Value="Home"
                        Text="Home"
                        Target="Content"
                        Expanded="True">

                        <asp:TreeNode Value="Page 1"
                            Text="Page1"
                            Target="Content">

                        </asp:TreeNode>

                        <asp:TreeNode Value="Page 2"
                            Text="Page 2"
                            Target="Content"></asp:TreeNode>

                    </asp:TreeNode>
                    <asp:TreeNode Value="Home1"
                        Text="Home1"
                        Target="Content"></asp:TreeNode>
                    <asp:TreeNode Value="Home2"
                        Text="Home2"
                        Target="Content"></asp:TreeNode>
                    <asp:TreeNode Value="Home3"
                        Text="Home3"
                        Target="Content"></asp:TreeNode>
                </Nodes>
            </asp:TreeView>
            <asp:Button ID="Button1" Text="Submit" runat="server" OnClick="Submit" />
        </div>
    </form>
</body>
</html>
