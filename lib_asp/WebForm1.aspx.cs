﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using OriginalUserControls;
using System.Text;

namespace lib_asp
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        public static TreeView TemporaryTree = new TreeView();
        public static TreeView StoredTree = new TreeView();
        public static bool isBeg = true;
        public static string message = "";
        public static string filter = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (isBeg)
            {
                nodenod.Nodes.Add(new TreeNode("Home1", "Home1"));
                nodenod.Nodes.Add(new TreeNode("Home2", "Home2"));
                nodenod.Nodes.Add(new TreeNode("Home3", "Home3"));
                nodenod.Nodes.Add(new TreeNode("Mango", "Fruit1"));
                nodenod.Nodes.Add(new TreeNode("Apple", "Fruit2"));
                nodenod.Nodes.Add(new TreeNode("Pineapple", "Fruit3"));
                nodenod.Nodes.Add(new TreeNode("Orange", "Fruit4"));
                nodenod.Nodes.Add(new TreeNode("Grapes", "Fruit5"));
                foreach (TreeNode node in nodenod.Nodes)
                {
                    if (node.Text == "Mango")
                    {
                        node.ChildNodes.Add(new TreeNode("WOW", "SubNode"));
                    }
                }
                foreach (TreeNode node in nodenod.Nodes)
                {
                    if (node.Text == "Home3")
                    {
                        node.ChildNodes.Add(new TreeNode("Child1", "SubNode1"));
                        node.ChildNodes.Add(new TreeNode("Child2", "SubNode1"));
                    }
                }
                Copy(nodenod, StoredTree);
            }
            isBeg = false;
        }

        public void nodenod_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            message = "";
            foreach (TreeNode t in nodenod.Nodes)
            {
                if (t.Checked == true)
                {
                    message += t.Text.ToString() + "; ";
                }
                if (t.ChildNodes.Count > 0)
                {
                    foreach (TreeNode tt in t.ChildNodes)
                        if (tt.Checked == true)
                        {
                            message += tt.Text.ToString() + "; ";
                        }     
                }
            }
        }

        protected void Submit(object sender, EventArgs e)
        {
            filter = txtData.Text;
            if (filter != "")
            {
                TreeNode temp = null;
                    foreach (TreeNode node in nodenod.Nodes)
                    {
                        if (node.Text == filter || node.Text.Contains(filter))
                        {
                            TemporaryTree.Nodes.Add(new TreeNode(node.Text, node.Text));
                        }
                    }
                nodenod.Nodes.Clear();
                lock(TemporaryTree.Nodes)
                {
                    foreach (TreeNode node in TemporaryTree.Nodes)
                    {
                        nodenod.Nodes.Add(new TreeNode(node.Text, node.Text));
                    }
                }
            }else
            {
                nodenod.Nodes.Clear();
                Copy(StoredTree, nodenod);      
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


        public void Copy(TreeView treeview1, TreeView treeview2)
        {
            TreeNode newTn;
            foreach (TreeNode tn in treeview1.Nodes)
            {
                newTn = new TreeNode(tn.Text, tn.Value);
                CopyChilds(newTn, tn);
                treeview2.Nodes.Add(newTn);
            }
        }

        public void CopyChilds(TreeNode parent, TreeNode willCopied)
        {
            TreeNode newTn;
            foreach (TreeNode tn in willCopied.ChildNodes)
            {
                newTn = new TreeNode(tn.Text, tn.Value);
                parent.ChildNodes.Add(newTn);
            }
        }

    }
}
    
