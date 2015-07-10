using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebUserControl
{
    /// <summary>
    /// Класс для уникнення проблеми з подвійним кліком.
    /// В звичайному дереві при подвійному кліку завжди проставлялася галочка
    /// а галочки в деяких нодах треба зоборонити
    /// </summary>
    public class TreeViewExt : TreeView
    {
        /// <summary>
        /// Подія - подвійний клік по ноду.
        /// замінює стандартний подвійний клік, стандартний заблокований
        /// </summary>
        public event NodeExtDelegate NodeExtDoubleClick;
        #region Опис проблеми і вирішення
        /*
        У меня есть TreeView с включенными checkboxes. Мне нужно запретить для некоторых узлов возможность поставить галочку. 
        Самое простое решение я выбрал такое – как только ставится галочка (checked = true) - я сразу ее снимаю: 
        treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
             if(e.Node.Checked)
                e.Node.Checked = false;
        }
        Но такой код неадекватно работает, если произвести двойной клик по checkbox’у
        После двойного клика галочка ставится. Кажется что при двойном клике функция treeView1_AfterCheck срабатывает только после первого клика,
        а после второго нет (в этот момент появляется галочка) 
        А если кликнуть третий раз (даже спустя несколько секунд и по другому ноду) – то второй и третий клик срабатывают как doubleCklick
         
        В цьому зміненому дереві подавляєтсья подвійний клік взагалі.
        І штучно запускаєтсья розкриття дерева.         
        */
        #endregion
        /*
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203)
            {
                m.Result = IntPtr.Zero;

                TreeNode editNode = this.GetNodeAt(
                this.PointToClient(System.Windows.Forms.Control.MousePosition));

                System.Drawing.Point p = this.PointToClient(System.Windows.Forms.Control.MousePosition);
                if (editNode != null)
                {
                    //SelectAllChild((TreeNodeExt)editNode, editNode.Checked);
                    if (NodeExtDoubleClick != null)
                        NodeExtDoubleClick.Invoke((TreeNodeExt)editNode);

                    //Щоб в дереві без мультивибору можна було по подвійному кліку вибрати елемент.
                    //Ми тут перехопили подвійний клік і вручну його запустили. Основна проблема вирішуєтсья таким чином. І дабл клік працює.
                    MouseEventArgs mea = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, p.X, p.Y, 0);
                    this.OnMouseDoubleClick(mea);
                   
                }
            }
            else
                base.WndProc(ref m);
        }
        */

        /// <summary>
        /// Пошук Ноди по Id (Id сутності)
        /// Тобто знайти можна тільки ті ноди, яким выдповідає сутність.
        /// Якщо нода не знайдена - повертає Null
        /// </summary>
        /// <param name="nodeId">Id сутності</param>
        /// <returns></returns>
        public TreeNodeExt FindByEssenceId(Int32 nodeId)
        {
            if (nodeId >= 0)
            {
                List<TreeNodeExt> findNodes = new List<TreeNodeExt>();
                FindByEssenceId_Ch(nodeId, findNodes, Nodes);

                if (findNodes.Count == 1)
                    return findNodes[0];
                else
                    return null;
            }
            else
                throw new Exception("В дереві здійснено пошук нода по Id = -1");
        }
        //рекурсивний пошук по Ід сутності
        private void FindByEssenceId_Ch(Int32 nodeEssenceId, List<TreeNodeExt> findNodes, TreeNodeCollection nodes)
        {
            foreach (TreeNodeExt node in nodes)
            {
                if (nodeEssenceId == node.Id)
                    findNodes.Add(node);
                if (node.Nodes.Count > 0)
                    FindByEssenceId_Ch(nodeEssenceId, findNodes, node.Nodes);
            }
        }
    }

    /// <summary>
    /// делегат для реалізації події подвійного кліку. передає нод по якому клікнули.
    /// </summary>
    /// <param name="node"></param>
    public delegate void NodeExtDelegate(TreeNodeExt node);
}
