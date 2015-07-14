using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace lib_asp.Lib
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TreeViewExt runat=server></{0}:TreeViewExt>")]
    public class TreeViewExt : TreeView
    {
        protected override TreeNode CreateNode()
        {
            // Tree node will get its members populated with the data from VIEWSTATE
            return new TreeNodeExt();
        }
    }
}
