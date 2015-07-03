﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OriginalUserControls;
using System.Text;



namespace lib_asp
{
        public partial class WebForm1 : System.Web.UI.Page
        {
            public string message = "";
            protected void Page_Load(object sender, EventArgs e)
            {
                
            }

            protected void Submit(object sender, EventArgs e)
            {/*
                string message = "";
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
                }*/
                ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "alert('" + message + "');", true);
            }

            public void nodenod_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
            {
                
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

        }
    }