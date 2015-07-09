using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace OriginalUserControls
{
    /// <summary>
    /// Простий ComboTreeBox, в ньому немає мультивибору та фільтру.
    /// Є тільки поле SelectedId, для отримання вибраного елементу
    /// Пронаслідувано від абстракного базового класу ComboTreeBox_Base
    /// </summary>
    public class ComboTreeBox_Simple : ComboTreeBox_Base
    {
        protected Int32 selectedId;

        /// <summary>
        /// Id Обраного елементу в дереві комбобокса
        /// </summary>
        [Category("1. Додаткові властивості")]
        [DisplayName("Id вибраного елементу")]
        public virtual Int32 SelectedId
        {
            set
            {
                //Як тільки ми змінюємо обраний елемент (значення Id обраного елементу)
                //то автоматично змінюємо текст контрола

                TreeNode[] findNodes = tv_Visible.Nodes.Find(value.ToString(), true);
                if (findNodes.Length == 1)
                {
                    selectedId = value;
                    tv_Visible.SelectedNode = findNodes[0];
                    FillText(findNodes[0].Text);
                }
            }
            get { return selectedId; }
        }

        public ComboTreeBox_Simple()
        {
            tv_Visible.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeView_NodeMouseDoubleClick);
            tv_Visible.KeyPress += new KeyPressEventHandler(treeView_KeyPress);
        }

        protected virtual void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (String.IsNullOrEmpty(e.Node.Name) == false)
            //{
                //if (Convert.ToInt32(e.Node.Name) >= 0)
                //{
                    //SelectedId = Convert.ToInt32(e.Node.Name);
                    dropDown.Close();
                //}
            //}
        }
        protected virtual void treeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            //якщо натиснута <Enter>
            if (e.KeyChar.Equals((char)13))
            {
                if (String.IsNullOrEmpty(tv_Visible.SelectedNode.Name) == false)
                {
                    if (Convert.ToInt32(tv_Visible.SelectedNode.Name) >= 0)
                    {
                        SelectedId = Convert.ToInt32(tv_Visible.SelectedNode.Name);
                        dropDown.Close();
                    }
                }
            }
        }
    }
}
