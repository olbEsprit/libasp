using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.ComponentModel;

namespace OriginalUserControls
{
    /// <summary>
    /// Базовий клас для ComboTreeBox'a, від нього наслідкуються ComboTreeBox_Simple та ComboTreeBox_Advanced
    /// </summary>
    public abstract class ComboTreeBox_Base : ComboBox
    {
        protected ToolStripControlHost treeViewHost;
        protected ToolStripDropDown dropDown;
        protected TreeViewExt tv_Visible;

        #region Обмежений доступ до дерева (лише до деяких його властивостей.)
        /// <summary>
        /// Клас для того, щоб надати обмежений доступ до дерева (лише до деяких його властивостей.)
        /// </summary>
        public class TreeViewOpenPropertys
        {
            private ComboTreeBox_Base comboTreeBox;
            public TreeViewOpenPropertys(ComboTreeBox_Base comboTreeBox)
            {
                this.comboTreeBox = comboTreeBox;
            }

            private Color backColor;
            /// <summary>
            /// Колір фону дерева
            /// </summary>
            public Color BackColor
            {
                set
                {
                    backColor = value;
                    comboTreeBox.treeView_BackColor = value;
                }
                get { return backColor; }
            }

            /// <summary>
            /// Колекція нодів
            /// </summary>
            public TreeNodeCollection Nodes
            {
                get { return comboTreeBox.tv_Visible.Nodes; }
            }

        }
        private TreeViewOpenPropertys treeView; //Відкрито для доступу до деяких властивостей дерева
        /// <summary>
        /// Доступ до дерева (обмежений)
        /// </summary>
        public TreeViewOpenPropertys TreeView
        {
            get { return treeView; }
        }

        private Color treeView_BackColor; //колір фону під деревом
        #endregion

        ToolTip toolTip;
        
        #region Заблоковані поля
        
        /// <summary>
        /// Не використовуйте це поле. Для КомбоТріБоксу воно немає смислу. 
        /// Використовуйте SelectedId
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new String ValueMember
        {
            get { MessageBox.Show("Код звертається до поля ComboTreeBox.ValueMember. Не варто використовувати це поле! Для цього контрола воно не має значення і завжди вертає NULL"); return null; }
        }
        
        /// <summary>
        /// Не використовуйте це поле. Для КомбоТріБоксу воно немає смислу. 
        /// Використовуйте SelectedId
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new String DisplayMember
        {
            get { MessageBox.Show("Код звертається до поля ComboTreeBox.DisplayMember. Не варто використовувати це поле! Для цього контрола воно не має значення і завжди вертає NULL"); return null; }
        }
        /// <summary>
        /// не використовуйте це поле. Для КомбоТріБоксу воно немає смислу, 
        /// тому що елемент реалізований так, що в Комбобоксі елементу завжди тільки один елемент.
        /// використовуйте SelectedId
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Int32 SelectedIndex
        {
            get { MessageBox.Show("Код звертається до поля ComboTreeBox.SelectedIndex. Не варто використовувати це поле! Для цього контрола воно не має значення і завжди вертає -1"); return -1; }
        }
        /// <summary>
        /// Не використовуйте це поле. Для КомбоТріБоксу воно немає смислу. 
        /// Використовуйте SelectedId
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Object SelectedValue
        {
            get { MessageBox.Show("Код звертається до поля ComboTreeBox.SelectedValue. Не варто використовувати це поле! Для цього контрола воно не має значення і завжди вертає NULL"); return null; }
        }
        #endregion

        public ComboTreeBox_Base()
        {
            // ----------------------
            //візуальні налаштування
            // ----------------------
            treeView = new TreeViewOpenPropertys(this); //клас для доступу до цих змінних

            this.Size = new Size(200, 21); //розмір по замовченню

            this.Size = new System.Drawing.Size(200, 21); //розмір по замовченню

            this.DropDownStyle = ComboBoxStyle.DropDownList; //Щоб не можна було вводити текст

            TreeViewExt tv = new TreeViewExt();
            tv.BorderStyle = BorderStyle.None;
            treeViewHost = new ToolStripControlHost(tv);
            treeViewHost.Margin = new Padding(1); //Виправити! зробити 0
            treeViewHost.Padding = new Padding(0);
            treeViewHost.AutoSize = false;

            dropDown = new ToolStripDropDown();
            dropDown.Padding = new Padding(0);
            dropDown.Margin = new Padding(0);
            dropDown.AutoSize = false;
            dropDown.Items.Add(treeViewHost);

            this.tv_Visible = (TreeViewExt)treeViewHost.Control;
            this.tv_Visible.ShowNodeToolTips = true;
            tv_Visible.Name = "VisibleTree";

            this.KeyPress += new KeyPressEventHandler(ComboTreeBox_KeyPress);
            dropDown.Closed += new ToolStripDropDownClosedEventHandler(dropDown_Closed);

            //Створення контекстного меню, для очистки комбобокса
            ToolStripMenuItem tsmi_ClearCombo = new ToolStripMenuItem("Очистити");
            tsmi_ClearCombo.Click += new EventHandler(ClearCombo_Click);
            ContextMenuStrip cms_ResetCombo = new ContextMenuStrip();
            cms_ResetCombo.Items.Add(tsmi_ClearCombo);
            this.ContextMenuStrip = cms_ResetCombo;
            
            treeView_BackColor = SystemColors.Window;
            DropDownHeight = 150;

            //підсказка, якщо текст не поміщається в комбобоксі
            toolTip = new ToolTip();
        }

        /// <summary>
        /// Вивиодимо підсказку про вибрані підрозділи 
        /// (якщо текст не поміщається в комбобоксі)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseHover(EventArgs e)
        {
            //Виводим вспливаючу підсказку, якщо текст не поміщаєтсья в комбобоксі

            base.OnMouseHover(e);

            Size textSize = TextRenderer.MeasureText(this.Text, this.Font, this.Size, TextFormatFlags.TextBoxControl);
            String text = this.Text;

            //розбиваєм підсказку на багато рядків (на рядок по сутності) - для читабельності
            if (textSize.Width > this.Width)
            {
                Int32 i = 0;
                while (i < text.Length)
                {
                    string t = text[i].ToString();
                    if (text[i] == ';')
                    {
                        text = text.Insert(i + 1, "\n");
                        i ++;
                    }
                    i++;
                }
            }
            toolTip.SetToolTip(this, text);

        }
        
        #region Базові властивості, не чіпать!
        /// <summary>
        /// Коли розкриваєтсья випадаючий список.
        /// Встановлюютсья розміри для нього і він відображається
        /// </summary>
        protected virtual void ShowDropDown()
        {
            if (dropDown != null)
            {
                dropDown.LayoutStyle = ToolStripLayoutStyle.Table;
                dropDown.Size = new Size(DropDownWidth-1, DropDownHeight);
                dropDown.BackColor = Color.Gray;

                treeViewHost.Size = new System.Drawing.Size(DropDownWidth - 3, DropDownHeight - 2);
                treeViewHost.BackColor = treeView_BackColor;

                dropDown.Show(this, 0, this.Height);
                if (tv_Visible.SelectedNode != null)
                    treeViewHost.Focus(); // Ця команда працює, перевірено. Не видалять!!!
            }
        }


        private const int WM_USER = 0x0400,
                        WM_REFLECT = WM_USER + 0x1C00,
                        WM_COMMAND = 0x0111,
                        CBN_DROPDOWN = 7,
                        WM_LBUTTONDBLCLK = 0x203;

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (WM_REFLECT + WM_COMMAND))
            {
                if (HIWORD((int)m.WParam) == CBN_DROPDOWN)
                {
                    ShowDropDown();
                    return;
                }
            }

            base.WndProc(ref m);
        }

        // Edit: 10:37, remember to dispose the dropdown as it's not in the control collection. 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dropDown != null)
                {
                    dropDown.Dispose();
                    dropDown = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        private void ComboTreeBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //якщо натиснута <Enter>
            if (e.KeyChar.Equals((char)13))
            {
                ShowDropDown();
            }
        }
        protected virtual void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            //після вдалого вибору елемента викликаєме пеервірку елементу.
            //Завдяки цьому на CTB_SubdivType.Validated можна підписувати інші контроли
            this.OnValidated(new EventArgs());
        }

        /// <summary>
        /// Функція для контекстного меню, очищає комбобокс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ClearCombo_Click(object sender, EventArgs e)
        {
            this.Text = "";
            this.Items.Clear();
            ((ComboBox)this).SelectedIndex = -1;
        }

        /// <summary>
        /// Заповнення тексту комбобоксу.        
        /// Щоб відобразити текст в комбобоксі "DropDownList" функція створює у випадаючому списку фіктивний елемент і обирає його
        /// </summary>
        /// <param name="text">текст який потрібно відобразити в комбобоксі</param>
        protected void FillText(String text)
        {
            if (String.IsNullOrEmpty(text) == false)
            {
                this.Items.Clear();
                this.Items.Add(text);
                ((ComboBox)this).SelectedIndex = 0;
            }
            else
            {
                this.Text = "";
                this.Items.Clear();
                ((ComboBox)this).SelectedIndex = -1;
            }
        }

    }
}
