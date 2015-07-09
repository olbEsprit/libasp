using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OriginalUserControls
{
    /// <summary>
    /// Розширений варіант TreeNode
    /// Ноди з додатковими функціями і полями.
    /// </summary>
    public class TreeNodeExt : TreeNode
    {
        public TreeNodeExt() : base() 
        {
            CompareResult = CompareResult.Null;
            id = -1;
            FullName = null;
            ShortName = null;
            Abbreviation = null;
        }
        public TreeNodeExt(String text) : base(text) 
        {
            CompareResult = CompareResult.Null;
            id = -1;
            FullName = null;
            ShortName = null;
            Abbreviation = null;
        }

        private Int32 id;
        /// <summary>
        /// Поле що зберігає Id сутності
        /// Інколи сутність описуєтсья ієрархією нод, Id присвоюєтсья не всім нодам, 
        /// а тільки тим, що відповідають сутності в деякі таблиці RT
        /// Тому деякі проміжні ноди можуть взагалі немати Id і, віднопвідно, не можуть бути вибрані!
        /// </summary>
        public Int32 Id
        {
            set 
            {
                if (id >= -1)
                    id = value;
                else
                    id = -1;
            }
            get { return id; }
        }

        private String fullName;
        /// <summary>
        /// Повна назва сутності (назва з "RT таблиці")
        /// </summary>
        public String FullName
        {
            set { fullName = value; }
            get { return fullName; }
        }

        private String shortName;
        /// <summary>
        /// Скорочена назва сутності (назва з "RT таблиці")
        /// </summary>
        public String ShortName
        {
            set { shortName = value; }
            get { return shortName; }
        }

        private String abbreviation;
        /// <summary>
        /// Аббревіатура сутності (назва з "RT таблиці")
        /// </summary>
        public String Abbreviation
        {
            set { abbreviation = value; }
            get { return abbreviation; }
        }


        private TreeNodeExt parallelNode;
        /// <summary>
        /// Посилання на Нод
        /// Поле зроблене для того щоб посилатися на такий самий нод в іншому дереві
        /// </summary>
        public TreeNodeExt ParallelNode
        {
            set { parallelNode = value; }
            get { return parallelNode; }
        }
        
        private Color savedForeColor;
        /// <summary>
        /// Збережений колір тексту нода.
        /// Якщо дерево втрачало фокус - то нод втрачав колір текста.
        /// Цим полем колір можна відновити.
        /// </summary>
        public Color SavedForeColor
        {
            get { return savedForeColor; }
        }

        private Color savedBackColor;
        /// <summary>
        /// Збережений колір фону нода.
        /// Якщо дерево втрачало фокус - то нод втрачав колір фону.
        /// Цим полем колір можна відновити.
        /// </summary>
        public Color SavedBackColor
        {
            get { return savedBackColor; }
        }

        /// <summary>
        /// Копіює вузол дерева і все піддерево.
        /// Функція переписана для клонування розширеного нода TreeNodeExt
        /// а не тільки базових полів звичайного нода
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            TreeNodeExt tne = (TreeNodeExt)base.Clone();

            tne.Text = this.Text;
            tne.Id = this.Id;
            tne.FullName = this.FullName;
            tne.ShortName = this.ShortName;
            tne.Abbreviation = this.Abbreviation;

            tne.savedForeColor = this.savedForeColor;
            tne.savedBackColor = this.savedBackColor;
            tne.ImageIndex = this.ImageIndex;

            
            return tne;
        }

        /// <summary>
        /// Колір текста ноди.
        /// Одразу запам"ятовуєтсья колір в SavedForeColor
        /// </summary>
        public new Color ForeColor
        {
            set 
            {
                base.ForeColor = value;
                savedForeColor = value;
            }
            get { return base.ForeColor; }
        }
        /// <summary>
        /// Фоновий колір ноди.
        /// Одразу запам"ятовуєтсья колір в SavedBackColor
        /// </summary>
        public new Color BackColor
        {
            set 
            {
                base.BackColor = value;
                savedBackColor = value;
            }
        }

        public CompareResult CompareResult;

        /// <summary>
        /// Повертає кількість дочірніх нодів, в піднодах теж
        /// </summary>
        /// <returns>кількість дочірніх нодів</returns>
        public Int32 GetChildCount()
        {
            Int32 count = 0;
            count = GetChildCount(this);
            return count;
        }
        private Int32 GetChildCount(TreeNodeExt parentNode)
        {
            Int32 count = 0;

            if (parentNode.Nodes.Count > 0)
            {
                foreach (TreeNodeExt node in parentNode.Nodes)
                {
                    count++;
                    count += GetChildCount(node);
                }
            }

            return count;
        }
    }

    public enum CompareResult
    { 
        Null,
        NoContains,
        DoPaler,
        Remove
    }
}
