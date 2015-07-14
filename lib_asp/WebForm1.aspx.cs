using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Text;
using lib_asp.Lib;

namespace lib_asp
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        public static TreeViewExt TemporaryTree = new TreeViewExt();
        public static TreeViewExt StoredTree = new TreeViewExt();
        public static bool isBeg = true;
        public static string message = "";
        public static string filter = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            //Tree init
            if (!IsPostBack)
            {
                PopulateTree();
            }
            
            if (isBeg)
            {
                Copy(sampleTree, StoredTree);
            }
            
            isBeg = false;
            ClientScript.RegisterClientScriptBlock(this.GetType(), "asd", "$('#Button1').click();", true);
        }

        private void PopulateTree()
        {
            sampleTree.Nodes.Clear();
            TreeNodeExt root = new TreeNodeExt();
            root.Value = "root node";

            sampleTree.Nodes.Add(root);

            // Creating some fake nodes for testing purposes
            for (int i = 0; i < 10; i++)
            {
                TreeNodeExt child = new TreeNodeExt();
                TreeNodeExt doublechild = new TreeNodeExt();
                child.NodeId = i;               // Saved in ViewState
                child.NodeType = "Type " + i;   // Saved in ViewState
                child.Value = child.NodeType;
                doublechild.NodeId = i;   
                doublechild.NodeType = "Child " + i;   
                doublechild.Value = doublechild.NodeType;
                root.ChildNodes.Add(child);
                child.ChildNodes.Add(doublechild);
            }
        }

        protected void sampleTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeViewExt cTreeView = (TreeViewExt)sender;
            lblSelectedNode.Text = ((TreeNodeExt)cTreeView.SelectedNode).NodeType;
        }


        public void sampleTree_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            message = "";
            foreach (TreeNodeExt t in sampleTree.Nodes)
            {
                if (t.Checked == true)
                {
                    message += t.Text.ToString() + "; ";
                }
                CheckChild(t);
            }
        }

        public void CheckChild(TreeNodeExt t)
        {
            if (t.ChildNodes.Count > 0)
            {
                foreach (TreeNodeExt tt in t.ChildNodes)
                {
                    if (tt.Checked == true)
                    {
                        message += tt.Text.ToString() + "; ";
                    }
                    CheckChild(tt);
                }
            }
            

        }

        protected void Submit(object sender, EventArgs e)
        {
            filter = txtData.Text;
            if(StoredTree.Nodes.Count == sampleTree.Nodes.Count)
            {
                StoredTree.Nodes.Clear();
                Copy(sampleTree, StoredTree);
            }
            if (filter != "")
            { 
                foreach (TreeNodeExt node in sampleTree.Nodes)
                {
                    if (node.NodeType == filter || node.NodeType.Contains(filter))
                    {
                        TemporaryTree.Nodes.Add(new TreeNodeExt
                        {
                            NodeId = node.NodeId,
                            NodeType = node.NodeType,
                            Value = node.Value
                        });
                    }
                    else
                    {
                        foreach(TreeNodeExt Child in node.ChildNodes)
                        {
                            if(Child.NodeType == filter || Child.NodeType.Contains(filter))
                            {
                                TemporaryTree.Nodes.Add(new TreeNodeExt
                                {
                                    NodeId = Child.NodeId,
                                    NodeType = Child.NodeType,
                                    Value = Child.Value
                                });
                            }
                        }
                    }
                }
                sampleTree.Nodes.Clear();
                Copy(TemporaryTree, sampleTree);
                TemporaryTree.Nodes.Clear();
            }
            else
            {
                sampleTree.Nodes.Clear();
                Copy(StoredTree, sampleTree);      
            }
            txtData.Text = "";
            
        }


        [WebMethod]
        public static string GetServerData()
        {
            return message;
        }


        [WebMethod]
        public static string GetCurrentFilter()
        {
            return filter;
        }


        public void Copy(TreeViewExt treeview1, TreeViewExt treeview2)
        {
            TreeNodeExt newTn;
            foreach (TreeNodeExt tn in treeview1.Nodes)
            {
                newTn = new TreeNodeExt
                {
                    NodeId = tn.NodeId,
                    NodeType = tn.NodeType,
                    Value = tn.Value
                };
                if(tn.Checked)
                {
                    newTn.Checked = true;
                }
                CopyChilds(newTn, tn);
                treeview2.Nodes.Add(newTn);
            }
        }

        public void CopyChilds(TreeNodeExt parent, TreeNodeExt willCopied)
        {
            TreeNodeExt newTn;
            foreach (TreeNodeExt tn in willCopied.ChildNodes)
            {
                newTn = new TreeNodeExt
                {
                    NodeId = tn.NodeId,
                    NodeType = tn.NodeType,
                    Value = tn.Value
                };
                if (tn.Checked)
                {
                    newTn.Checked = true;
                }
                parent.ChildNodes.Add(newTn);
                CopyChilds(newTn, tn);
            }
        }

    }
}
    
