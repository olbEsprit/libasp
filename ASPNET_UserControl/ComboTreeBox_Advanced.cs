using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
//using System.Reflection.Emit;
using System.Threading;

namespace OriginalUserControls
{
    /// <summary>
    /// Розширений ComBoTreeBox, в ньому є мультивибір, пошук нода (фільтр), різні режими відображення вибраного нода
    /// Пронаслідувано від абстракного базового класу ComboTreeBox_Base
    /// </summary>
    public class ComboTreeBox_Advanced : ComboTreeBox_Base
    {
        private Boolean isCheckBoxes;
        /// <summary>
        /// Поле показує, чи відображаються чекБокси в контролі
        /// </summary>
        public Boolean IsCheckBoxes
        {
            get { return isCheckBoxes; }
        }

        private SelectedItemView selectedItemView;
        /// <summary>
        /// Поле показує, як відображаєтсья вибраний елемент в полі ComboTreeBox
        /// Тобто після вибору в еткстовому полі КомбоБокса відображається назва ноду, складена назва (ієрархічна) або "повна назва" з RT таблиці
        /// </summary>
        public SelectedItemView SelectedItemView
        {
            set { selectedItemView = value; }
            get { return selectedItemView; }
        }

        private SelectedTreeItem selectedItem;
        /// <summary>
        /// вертає Вибраний елемент в дереві (Якщо мультивибір виключений)
        /// Якщо не вибраний елемент - вертає NULL
        /// Якщо мультивибір включений - теж вертає NULL
        /// </summary>
        public new SelectedTreeItem SelectedItem
        {
            get
            {
                if (isCheckBoxes == false)
                    return selectedItem;
                else
                    return null;
            }
        }

        private List<SelectedTreeItem> selectedList; //Список вибраних елементів в дереві 
        /// <summary>
        /// Список вибраних елементів в дереві
        /// Якщо мультивибір включений, інакше NULL
        /// Якщо жоден елемент не вибраний - то теж вертає NULL
        /// </summary>
        public List<SelectedTreeItem> SelectedList
        {
            get
            {
                if (isCheckBoxes == true)
                    return selectedList;
                else
                    return null;
            }
            //set { selectedList = value; }
        }

        /// <summary>
        /// Піктограмки для видимого дерева, використовуємо піктограмку, що показує вибір всіх дочірніх нодів
        /// </summary>
        private ImageList imList;

        //Індекс іконки, що показує, що вся група (вся гілка) вибрана
        private Int32 ImageIndex_FullBranch;
        private Int32 ImageIndex_NoIcon;


        /// <summary>
        /// Повна копія дерева. (Потрібно саме для реалізації фільтру)
        /// Коли видиме дерево відфільтроване - це залишається повним
        /// </summary>
        protected TreeViewExt tv_Full; //Теж розширений клас, не заради кліка, а заради пошуку по Id
        
        //Блок полів для фільтру !
        /// <summary>
        /// текстове поле фільтру
        /// </summary>
        private TextBox tb_filter;
        protected ToolStripControlHost filterHost;

        //блок полів для панельки мультивибору 
        protected ToolStripControlHost manageHost;
        private CheckBox chb_selectChild;
        private Label lbl_selectedCount;
        private Button btn_ok;

        //Блок полів для панельки неактуального дерева
        private ToolStripControlHost noActualTreeHost;
        private TextBox tb_NoActualTree;

        #region Поля для доступу до фільтру ззовні
        /// <summary>
        /// Допоміжний клас для доступу до деяких властивостей фільтру
        /// </summary>
        public class FilterBoxOpenPropertys
        {
            private ComboTreeBox_Advanced ctb_advanced;
            public FilterBoxOpenPropertys(ComboTreeBox_Advanced ctb_advanced)
            {
                this.ctb_advanced = ctb_advanced;
                inFocus_BackColor = ctb_advanced.filterInFocus_BackColor;
                inFocus_ForeColor = ctb_advanced.filterInFocus_ForeColor;
                noFocus_BackColor = ctb_advanced.filterNoFocus_BackColor;
                noFocus_ForeColor = ctb_advanced.filterNoFocus_ForeColor;
            }

            private Color noFocus_BackColor;
            /// <summary>
            /// Колір фону фільтра коли він немає фокусу
            /// </summary>
            public Color NoFocus_BackColor
            {
                set
                {
                    noFocus_BackColor = value;
                    ctb_advanced.filterNoFocus_BackColor = value;
                }
                get { return noFocus_BackColor; }
            }

            private Color noFocus_ForeColor;
            /// <summary>
            /// Колір тексту фільтра коли він немає фокусу
            /// </summary>
            public Color NoFocus_ForeColor
            {
                set
                {
                    noFocus_ForeColor = value;
                    ctb_advanced.filterNoFocus_ForeColor = value;
                }
                get { return noFocus_ForeColor; }
            }

            private Color inFocus_BackColor;
            /// <summary>
            /// Колір фону фільтра коли він має фокус
            /// </summary>
            public Color InFocus_BackColor
            {
                set
                {
                    inFocus_BackColor = value;
                    ctb_advanced.filterInFocus_BackColor = value;
                }
                get { return inFocus_BackColor; }
            }

            private Color inFocus_ForeColor;
            /// <summary>
            /// Колір тексту фільтра коли він має фокус
            /// </summary>
            public Color InFocus_ForeColor
            {
                get { return inFocus_ForeColor; }
            }

        }


        private FilterBoxOpenPropertys filterBox; //Відкрито для доступу до деяких властивостей фільтру
        /// <summary>
        /// Поле доступу фільтру
        /// </summary>
        public FilterBoxOpenPropertys FilterBox
        {
            get { return filterBox; }
        }
        private Color filterInFocus_BackColor; //Колір фону фільтру, коли він має фокусу
        private Color filterInFocus_ForeColor; //Колір тексту фільтру, коли він має фокус
        private Color filterNoFocus_BackColor; //Колір фону фільтру коли він немає фокус
        private Color filterNoFocus_ForeColor; //Колір тексту фільтру, коли він має фокус
        #endregion
        #region Поля для доступу до дерева ззовні
        /// <summary>
        /// Допоміжний клас для доступу до деяких властивостей Дерева
        /// </summary>
        public new class TreeViewOpenPropertys
        {
            private ComboTreeBox_Advanced ctb_advanced;
            public TreeViewOpenPropertys(ComboTreeBox_Advanced ctb_advanced)
            {
                this.ctb_advanced = ctb_advanced;

                //коли створюється це поле - то необхідно одразу визначити значення з батьківського класу
                //самі вони визначилися б тільки коли до них звернутися ззовні.
                selectedNodeNoFocus_BackColor = ctb_advanced.selectedNodeNoFocus_BackColor;
                selectedNodeNoFocus_ForeColor = ctb_advanced.selectedNodeNoFocus_ForeColor;

                nodesCorrespondFilter_BackColor = ctb_advanced.nodesCorrespondFilter_BackColor;
                nodesNoCorrespondFilter_ForeColor = ctb_advanced.nodesNoCorrespondFilter_ForeColor;

                nodesCanSelected_ForeColor = ctb_advanced.nodesCanSelected_ForeColor;
                nodesCanNotSelected_ForeColor = ctb_advanced.nodesCanNotSelected_ForeColor;

                /* текст не змінився? ->
                selectedNodeNoFocus_BackColor = ctb_advanced.selectedNodeNoFocus_BackColor;
                selectedNodeNoFocus_ForeColor = ctb_advanced.selectedNodeNoFocus_ForeColor;

                nodesCorrespondFilter_BackColor = ctb_advanced.nodesCorrespondFilter_BackColor;
                nodesNoCorrespondFilter_ForeColor = ctb_advanced.nodesNoCorrespondFilter_ForeColor;
                */

                //nodesNew = new TreeNodeCollectionExt(ctb_advanced.tv_Visible, ctb_advanced.tv_Full);

                //Позамовченню дерево актуальне
                isActual = true;
                dateOfBuild = DateTime.MinValue;
            }


            private Color backColor;
            private Color nodesCorrespondFilter_BackColor;  // Колір фону нодів, що відповідають фільтру
            private Color nodesNoCorrespondFilter_ForeColor;// Колір тексту нодів, що не відповідають фільтру
            private Color selectedNodeNoFocus_BackColor;    // Виділена нода, колір фону
            private Color selectedNodeNoFocus_ForeColor;    // Виділена нода, колір тексту
            private Color nodesCanSelected_ForeColor;       // Колір нодів, які можуть бути обрані
            private Color nodesCanNotSelected_ForeColor;    // Колір ноди, яка не можу бути обрана (немає такої сутності)


            /// <summary>
            /// Колір фону дерева
            /// </summary>
            public Color BackColor
            {
                set
                {
                    backColor = value;
                    ctb_advanced.treeView_BackColor = value;
                }
                get { return backColor; }
            }
            /// <summary>
            /// Колір підствічування нодів, що відповідають фільтру 
            /// (колір фону тексту)
            /// </summary>
            public Color NodesCorrespondFilter_BackColor
            {
                set
                {
                    nodesCorrespondFilter_BackColor = value;
                    ctb_advanced.nodesCorrespondFilter_BackColor = value;
                }
                get { return nodesCorrespondFilter_BackColor; }
            }
            /// <summary>
            /// Колір приглушення нодів що не відповідають фільтру
            /// (колір тексту)
            /// </summary>
            public Color NodesNoCorrespondFilter_ForeColor
            {
                set
                {
                    nodesNoCorrespondFilter_ForeColor = value;
                    ctb_advanced.nodesNoCorrespondFilter_ForeColor = value;
                }
                get { return nodesNoCorrespondFilter_ForeColor; }
            }
            /// <summary>
            /// Колір фону виділеного нода, коли дерево неактивне.
            /// (Щоб виділення не зникало)
            /// </summary>
            public Color SelectedNodeNoFocus_BackColor
            {
                get { return selectedNodeNoFocus_BackColor; }
            }
            /// <summary>
            /// Колір тексту виділеного нода, коли дерево неактивне.
            /// (Щоб виділення не зникало)
            /// </summary>
            public Color SelectedNodeNoFocus_ForeColor
            {
                get { return selectedNodeNoFocus_ForeColor; }
            }
            /// <summary>
            /// Колір нодів, які можуть бути обрані
            /// </summary>
            public Color NodesCanSelected_ForeColor
            {
                set { nodesCanSelected_ForeColor = value; }
                get { return nodesCanSelected_ForeColor; }
            }
            /// <summary>
            /// Колір нодів, які не можуть бути обрані (наприклад ноді не відповідає жодна сутність)
            /// </summary>
            public Color NodesCanNotSelected_ForeColor
            {
                set { nodesCanNotSelected_ForeColor = value; }
                get { return nodesCanNotSelected_ForeColor; }
            }

            //private TreeNodeCollectionExt nodesNew;
            /// <summary>
            /// Моя перероблена колекція нодів.
            /// Вона дозволяє додавати ноди тільки типу TreeNodeExt
            /// </summary>
            //public TreeNodeCollectionExt NodesNew
            //{
            //    get { return nodesNew; }
            //}

            /// <summary>
            /// Колекція нодів (повного дерева)
            /// </summary>
            public TreeNodeCollection Nodes
            {
                get { return ctb_advanced.tv_Full.Nodes; }
            }

            private Boolean isActual;
            /// <summary>
            /// Показує чи дерево актуальне (побудоване на сьогоднішню дату)
            /// </summary>
            public Boolean IsActual
            {
                set { isActual = value; }
                get { return isActual; }
            }

            private DateTime dateOfBuild;
            /// <summary>
            /// Якщо побудоване дерево не актуальне - то це поле показує, на який момент часу відображаєтсья дерево
            /// </summary>
            public DateTime DateOfBuild
            {
                set { dateOfBuild = value; }
                get { return dateOfBuild; }
            }

            /// <summary>
            /// Пошук нода в дереві (повному) по Id нода (Id сутності)
            /// </summary>
            /// <param name="nodeId"></param>
            /// <returns></returns>
            public TreeNodeExt FindNode(Int32 nodeId)
            {
                return ctb_advanced.tv_Full.FindByEssenceId(nodeId);
            }

        }


        private TreeViewOpenPropertys treeView; //Відкрито для доступу до деяких властивостей дерева
        /// <summary>
        /// Поле доступу до дерева
        /// </summary>
        public new TreeViewOpenPropertys TreeView
        {
            get { return treeView; }
        }

        private Color treeView_BackColor; //Колір фону Дерева
        private Color nodesCorrespondFilter_BackColor; // Колір підсвічування нодів, що відповідають фільтру
        private Color nodesNoCorrespondFilter_ForeColor;// Колір тексту нодів, що не відповідають фільтру
        //не налаштовується ззовні
        private Color selectedNodeNoFocus_BackColor; //Колір вибраного нода (букви), коли дерева без фокусу.
        private Color selectedNodeNoFocus_ForeColor; //Колір вибраного нода (фон), коли дерева без фокусу.
        private Color nodesCanSelected_ForeColor; //Колір тексту нодів, що можуть бути обрані.
        private Color nodesCanNotSelected_ForeColor; //Колір тексту нодів, що не можуть бути обраними.
        #endregion

        /// <summary>
        /// Поле що показує чи відфільтроване зараз дерево treeView.
        /// Якщо поле не відфільтроване - то не потрібно другий раз його відновлювати з повного дерева
        /// Просто в деяких частинках коду (в tb_filter_TextChanged) не можливо було перевірити в якому дерево стані
        /// </summary>
        private Boolean isFiltered = false;
        /// <summary>
        /// Останній вибраний нод, для поновлення фокусу, коли дерево втрачає фокус
        /// </summary>
        private TreeNode lastSelectedNode;
        /// <summary>
        /// Нод, по якому відбувся правий клік
        /// </summary>
        private TreeNodeExt rightClickedNode;

        //Контекстне меню, для кліка правою кнопкою миші
        ContextMenuStrip cms_rightClickMenu;
        //Кнопка для відображення дочірніх нодів. Кнопка для меню, що викликаєтсья правим кліком
        private ToolStripMenuItem tsmi_AddChild;
        //кнопка для скидування списку вибраних нодів
        private ToolStripMenuItem tsmi_ClearSelectedList;

        private Int32 showFilterAfterNodeCount;
        /// <summary>
        /// Властивість показує при якій кількості нод в дереві варто відображати фільтр.
        /// Якщо властивість = 0 - то фільтр відображається весь час.
        /// По замовченню = 10
        /// </summary>
        public Int32 ShowFilterAfterNodeCount
        {

            set
            {
                if (showFilterAfterNodeCount > 0)
                    showFilterAfterNodeCount = value;
                else
                    MessageBox.Show("Ця фластивіть не може бути менше за 0");
            }
            get { return showFilterAfterNodeCount; }
        }

        private Boolean isCheckVisibleTreeOnFill;
        /// <summary>
        /// Поле показує, чи потрібно при кожному розкритті контрола перевіряти заповненість видимого дерева.
        /// Мається на увазі, перевірка чи не додалися нові ноди в повне дерево (про що контрол не дізнається)
        /// В такому випадку в новій ноді повного дерева немає посилання на відповідний нод видимого дерева і видиме дерево потрібно перестворити.
        /// Функція перевірки займає час. Тому цю функцію можна вимкнути, якщо після додавання нових нодів викликати функцію: CopyFullToVisibleTree 
        /// </summary>
        public Boolean IsCheckVisibleTreeOnFill
        {
            set { isCheckVisibleTreeOnFill = value; }
            get { return isCheckVisibleTreeOnFill; }

        }


        //ПОДІЯ
        /// <summary>
        /// Подія відбувається коли змінюєтсья значення в контролі
        /// Тобто змінюєтсья SelectedItem або SelectedItemsList
        /// (насправді не змінюєтсья а саме підтверджуєтсья вибір, тобто якщо той самий виберем знову - подія теж спрацює)
        /// Подія відправляє один параметр - об"єкт типу ComboTreeBox_Advanced - тобто двіправляє себе.
        /// ТОбто в обробнику події ясно в якому контролі змінилося значення і можна одразу отримати це значення через поля sender.SelectedItem або sender.SelectedList
        /// </summary>
        public event SelectedItemChangedDelegate SelectedItemChanged;

        //КОНСТРУКТОРИ
        /// <summary>
        /// Конструктор розширеного КомбоТріБокса.
        /// По замовченню: спосіб відображення вибраного елементу - назва ноди
        /// По замовченню: Фільтр покажеться, якщо нодів буде більше ніж 20
        /// </summary>
        /// <param name="isCheckBoxes">Мультивибір</param>
        public ComboTreeBox_Advanced(Boolean isCheckBoxes) : this(isCheckBoxes, 20, SelectedItemView.NodeName) { }
        /// <summary>
        /// Конструктор розширеного КомбоТріБокса.
        /// По замовченню: спосіб відображення вибраного елементу - назва ноди
        /// </summary>
        /// <param name="isCheckBoxes">Мультивибір</param>
        /// <param name="ShowFilterAfterNodeCount">показувати фільтр, якщо нодів більше ніж N, Якщо 0 - завжди показувати, якщо -1 - ніколи не показувати</param>
        public ComboTreeBox_Advanced(Boolean isCheckBoxes, Int32 ShowFilterAfterNodeCount) : this(isCheckBoxes, ShowFilterAfterNodeCount, SelectedItemView.NodeName) { }
        /// <summary>
        /// Конструктор розширеного КомбоТріБокса.
        /// По замовченню: Фільтр покажеться, якщо нодів буде більше ніж 20
        /// </summary>
        /// <param name="isCheckBoxes">Мультивибір</param>
        /// <param name="selectedItemView">Cпосіб відображення вибраного елементу</param>
        public ComboTreeBox_Advanced(Boolean isCheckBoxes, SelectedItemView selectedItemView) : this(isCheckBoxes, 20, selectedItemView) { }
        /// <summary>
        /// Конструктор розширеного КомбоТріБокса.
        /// </summary>
        /// <param name="isCheckBoxes">Мультивибір</param>
        /// <param name="ShowFilterAfterNodeCount">Показувати фільтр, якщо нодів більше ніж N, Якщо 0 - завжди показувати, якщо -1 - ніколи не показувати</param>
        /// <param name="selectedItemView">Cпосіб відображення вибраного елементу</param>
        public ComboTreeBox_Advanced(Boolean isCheckBoxes, Int32 ShowFilterAfterNodeCount, SelectedItemView selectedItemView)
        {
            tv_Visible.KeyPress += new KeyPressEventHandler(treeView_KeyPress);
            dropDown.Closed += new ToolStripDropDownClosedEventHandler(dropDown_Closed);
            tv_Visible.KeyDown += new KeyEventHandler(tv_Visible_KeyDown);

            this.isCheckBoxes = isCheckBoxes;

            if (isCheckBoxes)
            {
                selectedList = new List<SelectedTreeItem>();

                tv_Visible.CheckBoxes = true;
                tv_Visible.AfterCheck += new TreeViewEventHandler(tv_Visible_AfterCheck);

                //Список піктограмок для видимого дерева
                imList = new ImageList();
                imList.Images.Add(Properties.Resources.check_all);
                imList.Images.Add(Properties.Resources.select);

                ImageIndex_FullBranch = 0;
                ImageIndex_NoIcon = 99;

                tv_Visible.ImageList = imList;
                tv_Visible.ImageIndex = ImageIndex_NoIcon;
                tv_Visible.SelectedImageIndex = 1;
            }

            this.selectedItemView = selectedItemView;

            //невидиме повне дерево
            tv_Full = new TreeViewExt();
            tv_Full.Name = "FullTree";

            
            #region налаштування змінних для Кольору
            filterNoFocus_BackColor = SystemColors.Window;
            filterNoFocus_ForeColor = Color.Gray;
            filterInFocus_BackColor = Color.LightPink;
            filterInFocus_ForeColor = SystemColors.WindowText;

            treeView_BackColor = SystemColors.Window;
            nodesCorrespondFilter_BackColor = Color.LightPink;
            nodesNoCorrespondFilter_ForeColor = SystemColors.WindowText; //Зараз чорний, але може затемнюватися.
            selectedNodeNoFocus_BackColor = Color.SteelBlue;
            selectedNodeNoFocus_ForeColor = Color.White;
            nodesCanSelected_ForeColor = SystemColors.WindowText; //Зараз чорний, але може затемнюватися.
            nodesCanNotSelected_ForeColor = Color.Gray;
            #endregion
            
            filterBox = new FilterBoxOpenPropertys(this); //клас для доступу до цих змінних
            treeView = new TreeViewOpenPropertys(this); //клас для доступу до цих змінних

            this.DropDownStyle = ComboBoxStyle.DropDownList; //Щоб не можна було вводити текст

            //Блок конструктора, пов"язаний з фільтром
            //Налаштування поля фільтру
            tb_filter = new TextBox();
            tb_filter.BorderStyle = BorderStyle.None;
            tb_filter.Text = " фильтр";
            tb_filter.GotFocus += new EventHandler(tb_filter_GotFocus);
            tb_filter.LostFocus += new EventHandler(tb_filter_LostFocus);
            tb_filter.TextChanged += new EventHandler(tb_filter_TextChanged);
            tb_filter.BackColor = filterNoFocus_BackColor;
            tb_filter.ForeColor = filterNoFocus_ForeColor;

            filterHost = new ToolStripControlHost(tb_filter);
            filterHost.AutoSize = false;
            filterHost.Margin = new Padding(0);
            filterHost.Padding = new Padding(0);

            //налашутвання мультивибору
            chb_selectChild = new CheckBox();
            chb_selectChild.Text = "вибирати дочірні";
            chb_selectChild.BackColor = SystemColors.ControlLight;
            chb_selectChild.Padding = new System.Windows.Forms.Padding(3);

            btn_ok = new Button();
            btn_ok.Text = "Ок";
            btn_ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn_ok.Click += new EventHandler(btn_ok_Click);

            lbl_selectedCount = new Label();
            lbl_selectedCount.AutoSize = true;
            lbl_selectedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));

            manageHost = new ToolStripControlHost(chb_selectChild);
            manageHost.AutoSize = false;
            manageHost.Margin = new Padding(0);
            manageHost.Padding = new Padding(0);
            manageHost.Visible = true;
            manageHost.Control.Controls.Add(btn_ok);
            manageHost.Control.Controls.Add(lbl_selectedCount);

            //налаштування панелі неактуального дерева
            tb_NoActualTree = new TextBox();
            tb_NoActualTree.ReadOnly = true;
            tb_NoActualTree.BorderStyle = BorderStyle.None;
            tb_NoActualTree.Margin = new Padding(0);
            tb_NoActualTree.BackColor = Color.MistyRose;
            tb_NoActualTree.ForeColor = Color.OrangeRed;
            tb_NoActualTree.Font = new System.Drawing.Font("Arial", 14);
            tb_NoActualTree.TextAlign = HorizontalAlignment.Center;
            
            noActualTreeHost = new ToolStripControlHost(tb_NoActualTree);
            noActualTreeHost.AutoSize = false;
            noActualTreeHost.Margin = new Padding(0);
            noActualTreeHost.Padding = new Padding(0);
            manageHost.Visible = true;

            //Додавання обох блоків
            dropDown.Items.Add(filterHost);
            dropDown.Items.Add(manageHost);
            dropDown.Items.Add(noActualTreeHost);
        

            //Видиме дерево
            tv_Visible.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
            tv_Visible.GotFocus += new EventHandler(treeView_GotFocus);
            tv_Visible.LostFocus += new EventHandler(treeView_LostFocus);
            tv_Visible.MouseClick += new MouseEventHandler(treeView_MouseClick);
            tv_Visible.MouseDoubleClick += new MouseEventHandler(tv_Visible_MouseDoubleClick);
            tv_Visible.BackColor = treeView_BackColor;
            tv_Visible.NodeExtDoubleClick += new NodeExtDelegate(NodeExtDoubleClick);



            //Створення контекстного меню для дерева
            cms_rightClickMenu = new ContextMenuStrip();
            cms_rightClickMenu.Closed += new ToolStripDropDownClosedEventHandler(cms_rightClickMenu_Closed);

            tsmi_AddChild = new ToolStripMenuItem("Відобразити дітей");
            tsmi_AddChild.Click += new EventHandler(tsmi_AddChild_Click);
            tsmi_AddChild.DisplayStyle = ToolStripItemDisplayStyle.Text;
            cms_rightClickMenu.Items.Add(tsmi_AddChild);
            cms_rightClickMenu.VisibleChanged += new EventHandler(cms_rightClickMenu_VisibleChanged);
            cms_rightClickMenu.Opening += new CancelEventHandler(cms_rightClickMenu_Opening);

            tsmi_ClearSelectedList = new ToolStripMenuItem("Очистити список вибраних нод");
            tsmi_ClearSelectedList.Click += new EventHandler(tsmi_ClearSelectedList_Click);
            cms_rightClickMenu.Items.Add(tsmi_ClearSelectedList);

            tv_Visible.ContextMenuStrip = cms_rightClickMenu;
            dropDown.AutoClose = true;
            //dropDown.Closed +=new ToolStripDropDownClosedEventHandler(dropDown_Closed);

            showFilterAfterNodeCount = ShowFilterAfterNodeCount; //по замовченню фільтр відображаєтсья, коли нод більше ніж 
            isCheckVisibleTreeOnFill = true;
        }


        /// <summary>
        /// Після кожної зміни вибраного ноду запам"ятовуємо вибір.
        /// Це потрібно для того щоб підфарбовувати вибраний нод, коли дерево деактивується.
        /// Тобто щоб вибраний нод був синім, навіть коли дерево деактивувати (напр перейти на фільтр)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lastSelectedNode = tv_Visible.SelectedNode;

            //У вибраному ноді така піктограмка, як в самому елементі.
            //Весь час знінюєтсья піктограмка вибраного ноду. Для того, щоб не малювати пктограмку з буквою S
            //працює не до кінця правильно. При бажанні потім доробити.
            //Якщо нод вибраний, піктограмку якого треба змінити - вона не міняється.
            //tv_Visible.SelectedImageIndex = e.Node.ImageIndex;            
        }

        /// <summary>
        /// Вибір нода галочкою.
        /// Цейнод заноситься в тимчасовий спиок вибраних нодів.
        /// Якщо буде потім підтвердження, то цей тимчасовий список скопіюєтсья в постійний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tv_Visible_AfterCheck(object sender, TreeViewEventArgs e)
        {

            Boolean isChecked = e.Node.Checked;
            TreeNodeExt clickNode = (TreeNodeExt)e.Node;

            if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
            {
                //Якщо нод вибраний - треба вибрати його в невидимому дереві
                if (e.Node.Checked)
                {
                    {
                        //Елемент не можна вибрати якщо Id<0 (цьому елементу в дереві не відповідає сутність)
                        if (clickNode.Id < 0)
                            e.Node.Checked = false;

                        //зміна вибору в невидимому дереві
                        UpdateCheckInparallelTree(clickNode);
                    }
                }
                else //Якщо нод невибраний - видалити його з списку вибраних
                {
                    //зміна вибору в невидимому дереві
                    UpdateCheckInparallelTree(clickNode);
                }

                //перевірка чи заповнене все дерево:
                CheckOnSelectedGroup();

                //відображаєм кількість вибраних нодів
                SetText_LabelSelectedCount();
            }
        } //з малюнками

        /// <summary>
        /// Подвійний клік - вибір всієї групи
        /// </summary>
        /// <param name="clickNode"></param>
        private void NodeExtDoubleClick(TreeNodeExt clickNode)
        {
            if (chb_selectChild.Checked)
            {
                if (clickNode.Id < 0)
                {
                    if (clickNode.ImageIndex == ImageIndex_FullBranch)
                        ChangeCheck_Child(clickNode, false);
                    else
                        ChangeCheck_Child(clickNode, true);
                }
                else
                    ChangeCheck_Child(clickNode, clickNode.Checked);

                UpdateCheckInparallelTree(clickNode);

                CheckOnSelectedGroup();

                //відображаєм кількість вибраних нодів
                SetText_LabelSelectedCount();
            }
        }
        private void ChangeCheck_Child(TreeNodeExt node, Boolean isChecked)
        {
            if (isChecked) //Якщо батьківський вибраний
            {
                //Якщо Id менше нуля - то цьому елементу в дереві не відповідає сутність, і елемент не можна вибрати
                if (node.Id < 0)
                    node.Checked = false;
                else
                    node.Checked = true;
            }
            else //Якщо батьківський не вибраний
                node.Checked = false;

            //зміна вибору в невидимому дереві
            UpdateCheckInparallelTree(node);

            if (node.Nodes.Count > 0)
            {
                foreach (TreeNodeExt childNode in node.Nodes)
                {
                    ChangeCheck_Child(childNode, isChecked);
                }
            }
        }

        /// <summary>
        /// перевірка чи є повністю вибрані групи і проставляння піктограмки групи.
        /// </summary>
        /// <param name="clickNodeInVisibleTree"></param>
        private void CheckOnSelectedGroup()
        {
            foreach (TreeNodeExt node in tv_Full.Nodes)
            {
                CheckOnSelectedGroup_rec(node);
            }
        }
        /// <summary>
        /// перевірка чи вибрані всі дочірні ноди та сусідні ноди.
        /// Якщо так - то батьківському ставиться знак вибраної групи.
        /// Рекурсивна функція.
        /// </summary>
        /// <param name="parentNodeInFull"></param>
        /// <returns></returns>
        private Int32 CheckOnSelectedGroup_rec(TreeNodeExt parentNodeInFull)
        {
            Int32 NoCheckedCount = 0;
            foreach (TreeNodeExt node in parentNodeInFull.Nodes)
            {
                if (node.Nodes.Count > 0)
                    NoCheckedCount += CheckOnSelectedGroup_rec(node);

                if (node.Id < 0)
                {
                    if (node.ImageIndex == ImageIndex_NoIcon)
                        NoCheckedCount++;
                }
                else
                    if (node.Checked == false)
                        NoCheckedCount++;
            }

            if (parentNodeInFull.Nodes.Count > 0 && NoCheckedCount == 0)
            {
                parentNodeInFull.ImageIndex = ImageIndex_FullBranch;
                UpdateCheckInparallelTree(parentNodeInFull);
            }
            else
            {
                parentNodeInFull.ImageIndex = ImageIndex_NoIcon;
                UpdateCheckInparallelTree(parentNodeInFull);
            }

            return NoCheckedCount;
        }

        /// <summary>
        /// Коли змінюєм вибір в одному дереві - ця функція змінює вибір в паралельному дереві.
        /// Наприклад якщо параметр - нод з видимого дерева. То функція скопіює властивості нода у відповідний нод в невидимому дереві
        /// наприклад це для того, щоб пам"ятати наш вибір під час фільтру (коли видиме дерево змінюється)
        /// </summary>
        /// <param name="node"></param>
        private void UpdateCheckInparallelTree(TreeNodeExt node)
        {
            if (node.ParallelNode != null)
            {
                node.ParallelNode.Checked = node.Checked;
                node.ParallelNode.ImageIndex = node.ImageIndex;
            }
            else
            {
                throw new ParallelNodeIsNullException(node);
            }
        }

        //Функції підтвердження вибору
        /// <summary>
        /// Якщо робимо подвійний клік по ноді, то це підтверджуєе вибір і запускає функцію зміни значення ChangeItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tv_Visible_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isCheckBoxes == false)
                ChangeItem((TreeNodeExt)tv_Visible.SelectedNode);
        }
        /// <summary>
        /// Якщо натискаємо ENTER, то це підтверджуєе вибір і запускає функцію зміни значення ChangeItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void treeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            //якщо натиснута <Enter>
            if (e.KeyChar.Equals((char)13))
            {
                //SelectedList = selectedListTemp.GetRange(0, selectedListTemp.Count);
                ChangeItem((TreeNodeExt)tv_Visible.SelectedNode);
            }
            //if (e.KeyChar.Equals((char)16) && e.KeyChar.Equals((char)38))
            {
                MessageBox.Show(e.KeyChar.ToString());
            }


        }
        /// <summary>
        /// Якщо натискаємо ок, то вибір з тимчасового списку переносим в постійний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_ok_Click(object sender, EventArgs e)
        {
            //SelectedList = selectedListTemp.GetRange(0, selectedListTemp.Count);
            ChangeItem((TreeNodeExt)tv_Visible.SelectedNode);
        }

        /// <summary>
        /// Якщо відбулося підтвердження вибору - Треба змінити значення елементу - 
        /// - викликаєм цю функцію і передаєм їй виділений нод
        /// </summary>
        /// <param name="selectedNode"></param>
        private void ChangeItem(TreeNodeExt selectedNode)
        {
            if (isCheckBoxes == false) //Якщо ОДНЕ значення
            {
                selectedList = null;

                if (selectedNode != null && selectedNode.Id >= 0)
                {
                    //Встановлення значення для контрола
                    selectedItem = new SelectedTreeItem(selectedNode.Id, selectedNode.Text, GetCompoundName(selectedNode), selectedNode.FullName, selectedNode.ShortName, selectedNode.Abbreviation);

                    //встановлення тексту для контрола (в залежності від вибраного "способу відображення вибраного елементу")
                    switch (selectedItemView)
                    {
                        case SelectedItemView.NodeName:
                            FillText(selectedItem.NodeName);
                            break;
                        case SelectedItemView.CompoundName:
                            FillText(selectedItem.CompoundName);
                            break;
                        case SelectedItemView.FullName:
                            if (selectedItem.FullName != null)
                                FillText(selectedItem.FullName);
                            else
                            {
                                MessageBox.Show("Елемент не вибирається, тому що FullName в цьому контролі не заповнене. Мабуть при створенні контрола треба було вибрати NodeName або CompoundName");
                                selectedItem = null;
                            }
                            break;
                        default:
                            MessageBox.Show("Помилочка, в коді не прописаний новий вид відображення вибраного елементу!");
                            break;
                    }
                    //Якщо відбувся вибір елементу - викликати подію, про те що відбувся вибір
                    if (SelectedItemChanged != null) //Якщо в основній програмі до події прив"язані якісь обробники подій
                    {
                        EventArgs ea = new EventArgs();
                        SelectedItemChanged.Invoke(this);
                    }

                    dropDown.Close();
                }
            }
            else //БАГАТО значень
            {
                selectedItem = null;

                //Збереження списку вибраних
                SaveTreeToList();

                //встановлення тексту для контрола
                String manyNames = GetManyNames();
                if (manyNames != null)
                {
                    FillText(GetManyNames());

                    //Якщо відбувся вибір елементу - викликати подію, про те що відбувся вибір
                    if (SelectedItemChanged != null) //Якщо в основній програмі до події прив"язані якісь обробники подій
                    {
                        EventArgs ea = new EventArgs();
                        SelectedItemChanged.Invoke(this);
                    }
                }
                else
                    selectedList = null;

                dropDown.Close();
            }
        }

        /// <summary>
        /// Функція отримує повну назву елементу(сутності) для вибраної ноди.
        /// Йдетсья про сутності що розбиті на багато вкладених нодів
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private String GetCompoundName(TreeNode node)
        {
            String fulltext = null;
            fulltext += node.Text;
            node = node.Parent;
            while (node != null)
            {
                //fulltext += "\\" + node.Text;
                fulltext = fulltext.Insert(0, node.Text + "\\");
                node = node.Parent;
            }

            return fulltext;
        }
        /// <summary>
        /// Функція дозволяє отримати рядок, що складається з багатьох назв.
        /// Використовувати, якщо в дереві стоїть мультивибір.
        /// </summary>
        /// <returns></returns>
        private string GetManyNames()
        {
            String AllNames = null;
            if (SelectedList.Count > 0)
            {
                foreach (SelectedTreeItem tni in SelectedList)
                {
                    //встановлення тексту для контрола (в залежності від вибраного "способу відображення вибраного елементу")
                    switch (selectedItemView)
                    {
                        case SelectedItemView.NodeName:
                            AllNames += tni.NodeName + "; ";
                            break;
                        case SelectedItemView.CompoundName:
                            AllNames += tni.CompoundName + "; ";
                            break;
                        case SelectedItemView.FullName:
                            if (tni.FullName != null)
                                AllNames += tni.FullName + "; ";
                            else
                            {
                                MessageBox.Show("Елемент не вибирається, тому що FullName в цьому контролі не заповнене. Мабуть при створенні контрола треба було вибрати NodeName або CompoundName");
                                return null;
                            }
                            break;
                        default:
                            MessageBox.Show("Помилочка, в коді не прописаний новий вид відображення вибраного елементу!");
                            break;
                    }
                }

                if (AllNames[AllNames.Length - 1] == ' ')
                {
                    AllNames.Remove(AllNames.Length - 1, 1);
                }
            }
            return AllNames;
        }

        /// <summary>
        /// Зберігає всі вибрані ноди (в повному невидимому дереві) в список вибраних нодів
        /// </summary>
        private void SaveTreeToList()
        {
            if (isCheckBoxes)
            {
                if (selectedList == null)
                    selectedList = new List<SelectedTreeItem>();
                else
                    selectedList.Clear();
                SaveTreeToList_Rec(tv_Full.Nodes);
            }
        }
        private void SaveTreeToList_Rec(TreeNodeCollection nodes)
        {
            foreach (TreeNodeExt node in nodes)
            {
                if (node.Checked)
                {
                    SelectedTreeItem sti = new SelectedTreeItem(node.Id, node.Text, GetCompoundName(node), node.FullName, node.ShortName, node.Abbreviation);
                    selectedList.Add(sti);
                }

                if (node.Nodes.Count > 0)
                    SaveTreeToList_Rec(node.Nodes);
            }
        }

        #region Встановлення і скидання значення програмно
        /// <summary>
        /// Встановлює вибраний елемент в дереві по Id
        /// </summary>
        /// <param name="itemId">Id елемента, який потрібно вибрати в дереві та значенні контрола</param>
        public void SetSelectedOneItem(Int32 itemId)
        {
            //if (!isCheckBoxes)
            {
                TreeNodeExt findNode = tv_Full.FindByEssenceId(itemId);
                if (findNode != null)
                {
                    //вибір візуальний - в дереві
                    lastSelectedNode = tv_Full.FindByEssenceId(itemId);
                    RecoverySelectedNode();

                    //також змінюємо значення контрола на цей нод що вибрали в дереві
                    ChangeItem(findNode);
                }
                else
                    throw new CanNotBeSetValueException(this.GetType().ToString(), new Int32[] { itemId });
            }
            //else
                //throw new Exception("ComboTreeBox отримав ззовні значення, яке потрібно вибрати в дереві. Але в дереві увімкнений мультивибір. Скористуйтеся функцією SetSelectedItems(Int32[] itemIds)");
        }

        /// <summary>
        /// масив Id елементів, які потрібно вибрати в дереві та значенні контрола
        /// </summary>
        /// <param name="itemIds"></param>
        public void SetSelectedManyItems(Int32[] itemIds)
        {
            List<Int32> errorIdList = new List<int>();

            if (isCheckBoxes)
            {
                foreach (Int32 itemId in itemIds)
                {
                    //Заповнюється невидиме дерево
                    TreeNodeExt findNode = tv_Full.FindByEssenceId(itemId);
                    if (findNode != null)
                    {
                        //проставляєм вибрані ноди в невидимому дереві
                        findNode.Checked = true;
                        //TreeViewEventArgs e = new TreeViewEventArgs(findNode, TreeViewAction.ByKeyboard);
                    }
                    else
                        errorIdList.Add(itemId);
                }

                if (errorIdList.Count == 0)
                    //також змінюємо значення контрола на ці ноди, що вибрали в дереві
                    //Вибрані ноди з повного дерева заносяться у список вибраних значень
                    ChangeItem(null);
                else
                {
                    Int32[] errorIdArray = new Int32[errorIdList.Count];
                    Int32 i;
                    for(i = 0; i< errorIdList.Count; i++)
                    {
                        errorIdArray[i] = errorIdList[i];
                    }
                    throw new CanNotBeSetValueException(this.GetType().ToString(), errorIdArray);
                }
            }
            else
                throw new Exception("ComboTreeBox отримав ззовні масив значень, які потрібно вибрати в дереві. Але в дереві вимкнений мультивибір. Скористуйтеся функцією SetSelectedItem(Int32 itemId)");
        }

        /// <summary>
        /// Функція ощичає вибрані користувачем елементи.
        /// Тобто обнуляютясь властивості selectedItem та selectedList, скидає текст контрола та вибраний елемент в дереві!
        /// </summary>
        public void SelectedItemsClear()
        {
            //Скидування значення.
            selectedItem = null;
            selectedList = null;
            //Скидування тексту
            FillText(null);

            #region Скидування дерева
            tv_Visible.SelectedNode = null;
            ResetBothTree();

            //скидування кольору виділеного останній раз ноду
            if (lastSelectedNode != null)
            {
                TreeNode tn = TreeManager.FindNode(tv_Visible.Nodes, lastSelectedNode.Text, true);
                if (tn != null)
                {
                    tn.BackColor = SystemColors.Window; //selectedNodeNoFocus_BackColor;
                    tn.ForeColor = SystemColors.ControlText; //selectedNodeNoFocus_ForeColor;
                }
            }

            tv_Visible.CollapseAll();
            #endregion

        }
        #endregion

        //перехід по вибраним елементам стрілочками  (Обробка натиснення клавіш шифт + трілка)
        void tv_Visible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                //перехід по вибраним елементам стрілочками
                if (isCheckBoxes) //якщо включений мультивибір
                {
                    if (selectedList != null && selectedList.Count > 0) //якщо щось вибрано
                    {
                        Int32 indexOfList = -1;

                        //Чи вибраний нод в дереві є в списку значень контрола
                        SelectedTreeItem findedNode = selectedList.Find(delegate(SelectedTreeItem item)
                        {
                            return item.Id == ((TreeNodeExt)tv_Visible.SelectedNode).Id;
                        });

                        //Якщо ніякий нод зі списку значень в дереві не вибраний
                        if (findedNode == null)
                        {
                            indexOfList = 0;
                        }
                        else //якщо вже вибраний нод зі списку занчень
                        {
                            indexOfList = selectedList.IndexOf(findedNode);

                            if (e.Shift && e.KeyCode == Keys.Up)
                            {
                                if (indexOfList == 0) //останній індех
                                    indexOfList = selectedList.Count - 1;
                                else
                                    indexOfList = indexOfList - 1;
                            }
                            else
                                if (e.Shift && e.KeyCode == Keys.Down)
                                {
                                    if (indexOfList == selectedList.Count - 1) //перший елемент
                                        indexOfList = 0; //тоді вибираєм останній
                                    else
                                        indexOfList = indexOfList + 1;
                                }
                        }

                        tv_Visible.SelectedNode = tv_Visible.FindByEssenceId(selectedList[indexOfList].Id);

                        e.SuppressKeyPress = true;
                    }
                }
            }
        }
                    
        protected override void ShowDropDown()
        {
            base.ShowDropDown();
            tv_Visible.BackColor = treeView_BackColor;

            if (isCheckVisibleTreeOnFill && CheckVisibleTreeOnFill()) //Якщо програміст додав нові ноди і не перестворив видиме дерево - воно заповниться автоматично (якщо ввімкнена ця опція)
            {
                //Якщо знайдений новий нод (в ньому не буде проініціалізований паралельний нод з видимого дерева.)
                //тому потрібно скопіювати поновому повне дерева у видиме. 
                //Заодно відновити попередній вид - до додавання нових нодів можуть бути вибрані ноди - відновлюєм вибрані ноди і якщо був - фільтр
                CopyTree(tv_Full, tv_Visible);

                tb_filter_TextChanged(null, null);
            }

            #region Малювання випадаючого списку в різних режимах, тільки дерево чи дерево + допоміжні панелі
            if (treeView.IsActual)
            {
                borderWidth = 1;
                dropDown.BackColor = Color.Gray;
            }
            else
            {
                borderWidth = 2;
                dropDown.BackColor = tb_NoActualTree.ForeColor;
            }
            
            //Якщо мультивибір включений - відображаєм чекбокс і кнопку
            if (isCheckBoxes)
            {
                if (selectedList == null)
                    selectedList = new List<SelectedTreeItem>();
                selectedItem = null;

                //помічаєм на дереві вибір з основного списку
                fromListToTree();

                //перевірка чи заповнені повні групи,  промальовування піктограмок повних:
                try
                {
                    CheckOnSelectedGroup();
                }
                catch (ParallelNodeIsNullException e)
                {
                    MessageBox.Show(e.Message);
                }

                //Якщо фільтр вимкнуний або нодів мало - фільтр НЕ Відображаєм
                if (showFilterAfterNodeCount < 0)
                {
                    if (treeView.IsActual)
                        ShowMultiOnly();
                    else
                        ShowMultiOnly_NoActual();
                }
                else //Якщо фільтр ввімкнений.
                {
                    //Якщо фільтр пустий і нодів мало - фільтр НЕ Відображаєм
                    if (String.IsNullOrEmpty(tb_filter.Text) && tv_Visible.GetNodeCount(true) < showFilterAfterNodeCount)
                    {
                        if (treeView.IsActual)
                            ShowMultiOnly();
                        else
                            ShowMultiOnly_NoActual();
                    }
                    else //Якщо фільтр не пустий або нодів багато
                    {
                        if (treeView.IsActual)
                            ShowMultiAndFilter();
                        else
                            ShowMultiAndFilter_NoActual();
                    }
                }
            }
            else //Якщо мультивибір ВИключений - НЕ відображаєм чекбокс і кнопку
            {
                selectedList = null;

                //Якщо фільтр вимкнуний або нодів мало - фільтр НЕ Відображаєм
                if (showFilterAfterNodeCount < 0)
                {
                    if (treeView.IsActual)
                        ShowSimple();
                    else
                        ShowSimple_NoActual();
                }
                else //Якщо фільтр ввімкнений.
                {
                    //Якщо фільтр пустий і нодів мало - фільтр НЕ Відображаєм
                    if (String.IsNullOrEmpty(tb_filter.Text) && tv_Visible.GetNodeCount(true) < showFilterAfterNodeCount)
                    {
                        if (treeView.IsActual)
                            ShowSimple();
                        else
                            ShowSimple_NoActual();
                    }
                    else //Якщо фільтр не пустий або нодів багато
                    {
                        if (treeView.IsActual)
                            ShowFilterOnly();
                        else
                            ShowFilterOnly_NoActual();
                    }
                }
            }
            #endregion

            //Якщо в дереві мало елементів - то вони розгортаютсья для зручності
            if (tv_Visible.GetNodeCount(true) <= showFilterAfterNodeCount)
                tv_Visible.ExpandAll();
        }
        #region Функції для відображення різних комбінацій випадаючого списку
        /// <summary>
        /// Функція помічає на дереві ноди, що занесені в список.
        /// Коли дерево розкривається ще раз - треба помітити на ньому ті ноді, що ми вибрали попереднього разу.
        /// (відновити вибір з SelectedList)
        /// </summary>
        void fromListToTree()
        {
            //Скидування дерева перед відновлюванням вибору
            ResetBothTree();

            foreach (SelectedTreeItem sti in selectedList)
            {

                //зі списку відновлюються тільки реальні ноди.

                TreeNodeExt visibleFindNode = tv_Visible.FindByEssenceId(sti.Id);
                if (visibleFindNode != null)
                    visibleFindNode.Checked = true;

                TreeNodeExt unvisibleFindNode = tv_Full.FindByEssenceId(sti.Id);
                if (unvisibleFindNode != null)
                    unvisibleFindNode.Checked = true;
            }
        }

        void ShowMultiAndFilter()
        {
            noActualTreeHost.Visible = false;
            manageHost.Visible = true;
            filterHost.Visible = true;

            ConfigureFilterHost();
            ConfigureManageHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - filterHost.Size.Height - manageHost.Size.Height - 2); // 2 - дві роздільні полоски між 3-ма елементами

            //Позиціонування
            tv_Visible.Location = GetLocation(0, 0);
            tb_filter.Location = GetLocation(0, tv_Visible.Size.Height + 1);
            chb_selectChild.Location = GetLocation(0, tv_Visible.Size.Height + tb_filter.Size.Height + 2);
        }
        void ShowMultiOnly()
        {
            noActualTreeHost.Visible = false;
            manageHost.Visible = true;
            filterHost.Visible = false;

            ConfigureManageHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - manageHost.Size.Height - 1);

            //Позиціонування
            tv_Visible.Location = GetLocation(0, 0);
            chb_selectChild.Location = GetLocation(0, tv_Visible.Size.Height + 1);


        }
        void ShowFilterOnly()
        {
            noActualTreeHost.Visible = false;
            manageHost.Visible = false;
            filterHost.Visible = true;

            ConfigureFilterHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - filterHost.Size.Height - 1);
            
            //Позиціонування
            tv_Visible.Location = GetLocation(0, 0); 
            tb_filter.Location = GetLocation(0, tv_Visible.Size.Height + 1);


        }
        void ShowSimple()
        {
            noActualTreeHost.Visible = false;
            manageHost.Visible = false;
            filterHost.Visible = false;

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight());

            //Встановлення позиції:
            tv_Visible.Location = GetLocation(0, 0);
        }

        void ShowMultiAndFilter_NoActual()
        {
            manageHost.Visible = true;
            filterHost.Visible = true;
            noActualTreeHost.Visible = true;

            ConfigureNoActualTreeHost();
            ConfigureFilterHost();
            ConfigureManageHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - noActualTreeHost.Size.Height - filterHost.Size.Height - manageHost.Size.Height - 3); // 3 - три роздільні полоски між 4-ма елементами

            //Позиціонування
            tb_NoActualTree.Location = GetLocation(0, 0);
            tv_Visible.Location = GetLocation(0, tb_NoActualTree.Size.Height + 1);
            tb_filter.Location = GetLocation(0, tb_NoActualTree.Size.Height + tv_Visible.Size.Height + 2);
            chb_selectChild.Location = GetLocation(0, tb_NoActualTree.Size.Height + tv_Visible.Size.Height + tb_filter.Size.Height + 3);
        }
        void ShowMultiOnly_NoActual()
        {
            manageHost.Visible = true;
            filterHost.Visible = false;
            noActualTreeHost.Visible = true;

            ConfigureNoActualTreeHost();
            ConfigureManageHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - noActualTreeHost.Size.Height - manageHost.Size.Height - 2);

            //Позиціонування
            tb_NoActualTree.Location = GetLocation(0, 0);
            tv_Visible.Location = GetLocation(0, tb_NoActualTree.Size.Height + 1);
            chb_selectChild.Location = GetLocation(0, tb_NoActualTree.Size.Height + tv_Visible.Size.Height + 2);
        }
        void ShowFilterOnly_NoActual()
        {
            manageHost.Visible = false;
            filterHost.Visible = true;
            noActualTreeHost.Visible = true;

            ConfigureNoActualTreeHost();
            ConfigureFilterHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - noActualTreeHost.Size.Height - filterHost.Size.Height - 2);

            //Позиціонування
            tb_NoActualTree.Location = GetLocation(0, 0);
            tv_Visible.Location = GetLocation(0, tb_NoActualTree.Size.Height + 1);
            tb_filter.Location = GetLocation(0, tb_NoActualTree.Size.Height + tv_Visible.Size.Height + 2);
        }
        void ShowSimple_NoActual()
        {
            manageHost.Visible = false;
            filterHost.Visible = false;
            noActualTreeHost.Visible = true;

            ConfigureNoActualTreeHost();

            //Встановлення розмірів
            treeViewHost.Size = new System.Drawing.Size(GetWorkArea_Width(), GetWorkArea_Hight() - noActualTreeHost.Size.Height - 1);

            //Позиціонування
            tb_NoActualTree.Location = GetLocation(0, 0);
            tv_Visible.Location = GetLocation(0, tb_NoActualTree.Size.Height + 1);
        }

        /// <summary>
        /// налаштування для панельки фільтра
        /// </summary>
        void ConfigureFilterHost()
        {
            filterHost.Size = new System.Drawing.Size(GetWorkArea_Width(), tb_filter.Size.Height);
            tb_filter.BackColor = filterNoFocus_BackColor;
            tb_filter.ForeColor = filterNoFocus_ForeColor;
        }
        /// <summary>
        /// налашутвання для панельки мультивибору
        /// </summary>
        void ConfigureManageHost()
        {
            manageHost.Size = new Size(GetWorkArea_Width(), 26);

            btn_ok.Size = new System.Drawing.Size(100, manageHost.Size.Height - 2);
            btn_ok.Location = new Point(GetWorkArea_Width() - btn_ok.Size.Width - 1, (manageHost.Size.Height - btn_ok.Size.Height) / 2);
            chb_selectChild.BackColor = SystemColors.ControlLight;
            btn_ok.BackColor = SystemColors.ControlLightLight;

            SetText_LabelSelectedCount();
        }
        /// <summary>
        /// Функція встановлює текст і візуальний вигляд для лейбла, в залежності від кількості вибраних нодів
        /// </summary>
        void SetText_LabelSelectedCount()
        {
            lbl_selectedCount.Visible = true;
            Int32 checkNodeCount = GetChecedkNodeCount();

            lbl_selectedCount.Text = "вибраних = " + checkNodeCount.ToString();
            if (checkNodeCount > 0)
            {
                lbl_selectedCount.Font = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                lbl_selectedCount.ForeColor = Color.Red;
            }
            else
            {
                lbl_selectedCount.Font = new System.Drawing.Font("Arial", 10, FontStyle.Regular);
                lbl_selectedCount.ForeColor = Color.DarkRed;
            }

            lbl_selectedCount.Location = new Point(150, (manageHost.Size.Height - lbl_selectedCount.Size.Height) / 2);

        }
        /// <summary>
        /// налаштування панельки "неактивності деерва"
        /// </summary>
        void ConfigureNoActualTreeHost()
        {
            noActualTreeHost.Size = new System.Drawing.Size(GetWorkArea_Width(), tb_NoActualTree.Size.Height);
            tb_NoActualTree.Text = "Дерево побудоване на момент: " + treeView.DateOfBuild.ToShortDateString();
        }

        #region Функції перетворюють відносні координати в абсолютні
        Int32 borderWidth = 1; //по завомвченню рамочка шириною в піксель (сіренька)
        Point GetLocation(Int32 x, Int32 y)
        {
            return new Point(borderWidth + x, borderWidth + y);
        }
        Int32 GetWorkArea_Width()
        {
            return DropDownWidth - borderWidth*2 - 1; //чому тут потрібно ще одиничку віднімати - я не розумію
        }
        Int32 GetWorkArea_Hight()
        {
            return DropDownHeight - borderWidth*2;
        }
        #endregion



        #endregion

        /// <summary>
        /// Скидування вибору та малюночків групи в ОБОХ деревах
        /// наприклад при відкритті КомбоТріБокса треба скинути дерева і відновити вибір в дереві зі списку
        /// </summary>
        void ResetBothTree()
        {
            ResetTree_rec(tv_Full.Nodes);
            ResetTree_rec(tv_Visible.Nodes);
        }
        /// <summary>
        /// Рекурсивна функція скидування вибору в дереві
        /// </summary>
        /// <param name="nodes"></param>
        void ResetTree_rec(TreeNodeCollection nodes)
        {
            foreach (TreeNodeExt node in nodes)
            {
                node.Checked = false;
                node.ImageIndex = ImageIndex_NoIcon;

                if (node.Nodes.Count > 0)
                    ResetTree_rec(node.Nodes);
            }
        }

        /// <summary>
        /// Функція, що очищає вибрані елементи в комбоТріБоксі.
        /// в базовій функції очищаєтсья лише текст, а тут і всі новостворені властивості.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void ClearCombo_Click(object sender, EventArgs e)
        {
            //тут очищаєтсья текст комбобокса
            base.ClearCombo_Click(null, null);

            //а тут очищаються властивості комбобокса
            tsmi_ClearSelectedList_Click(null, null);
        }

        /// <summary>
        /// Якщо програміст при створенні контрола заповнив повне дерево, але не скопіював його в видиме дерево
        /// </summary>
        private Boolean CheckVisibleTreeOnFill()
        {
            //if (tv_Full.Nodes.Count > 0)
            //{
            //    if(tv_Visible.Nodes.Count == 0)
            //        CopyTree(tv_Full, tv_Visible);
            //}

            return CheckVisibleTreeOnFill_rec(tv_Full.Nodes);
        }

        private Boolean CheckVisibleTreeOnFill_rec(TreeNodeCollection nodes)
        {
            Boolean isError = false;
            foreach (TreeNodeExt node in nodes)
            {
                if (node.ParallelNode == null)
                {
                    isError = true;
                    break;
                }

                if (node.Nodes.Count > 0)
                {
                    if (CheckVisibleTreeOnFill_rec(node.Nodes))
                    {
                        isError = true;
                        break;
                    }
                }
            }

            return isError;
        }

        #region Функції для малювання виділиних елементів
        void treeView_LostFocus(object sender, EventArgs e)
        {
            if (tv_Visible.SelectedNode != null)
            {
                tv_Visible.SelectedNode.BackColor = selectedNodeNoFocus_BackColor;
                tv_Visible.SelectedNode.ForeColor = selectedNodeNoFocus_ForeColor;
            }
        }
        void treeView_GotFocus(object sender, EventArgs e)
        {
            if (tv_Visible.SelectedNode != null)
            {
                tv_Visible.SelectedNode.BackColor = ((TreeNodeExt)tv_Visible.SelectedNode).SavedBackColor;
                tv_Visible.SelectedNode.ForeColor = ((TreeNodeExt)tv_Visible.SelectedNode).SavedForeColor;
            }

            dropDown.AutoClose = true; //на всякий випадок ще й тут включаєм автоприховування
        }
        #endregion

        #region Функції, для створення контекстного меню в дереві

        //При правому кліку по дереву відображається контекстне меню.
        //Ця функція перевіряє чи треба включати кнопку "відобразити дітей" в цьому меню, чи вона має бути виключена (бо немає дітей, що невідображені)
        void treeView_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //перевірка кнопки "додати всіх дочірніх"
                dropDown.AutoClose = false; //Щоб при виклику констекстного меню дропдаун не закрився.
                rightClickedNode = (TreeNodeExt)tv_Visible.GetNodeAt(e.X, e.Y);
                tv_Visible.SelectedNode = rightClickedNode;

                Int32 childCountInVisible = rightClickedNode.GetChildCount();
                Int32 childCountInFull = rightClickedNode.ParallelNode.GetChildCount();

                //якщо дерево відфільтроване + якщо e вузлі відображено менше дочірніх ніж взагалі в нього є
                if (isFiltered && childCountInVisible < childCountInFull)
                    tsmi_AddChild.Enabled = true;
                else
                    tsmi_AddChild.Enabled = false;

                //перевірка кнопки "очистити список вибраних"
                if (selectedList != null && selectedList.Count > 0)
                    tsmi_ClearSelectedList.Enabled = true;
                else
                    tsmi_ClearSelectedList.Enabled = false;
            }
        }
        void cms_rightClickMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            //Щоб дропдаун сам знову закривався
            dropDown.AutoClose = true;

            //Коли включаю AutoClose - це поле чомусь не працює і dropDown сам не закривається.
            //Допомагає його примусове перевідкриття. Властивість AutoClose  починає працювати
            dropDown.Hide();
            dropDown.Show();
            tv_Visible.Focus();
        }

        //додаткова функція, коли клацаєм по дереву але збоку - то чомусь контекстне меню відображається.
        //дві наступні функції намагаються його приховати в цьому випадку
        void cms_rightClickMenu_Opening(object sender, CancelEventArgs e)
        {
            if (rightClickedNode == null)
            {
                tsmi_AddChild.Enabled = false;
                cms_rightClickMenu.Close();
            }
        }
        void cms_rightClickMenu_VisibleChanged(object sender, EventArgs e)
        {
            if (cms_rightClickMenu.Visible)
            {
                if (rightClickedNode == null)
                    tsmi_AddChild.Enabled = false;
            }
        }

        /// <summary>
        /// Функція додає всі дочірні ноди у відфільтрованому дереві. Викликається з контекстного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tsmi_AddChild_Click(object sender, EventArgs e)
        {
            TreeNode[] findNodes = tv_Full.Nodes.Find(rightClickedNode.Name.ToString(), true);


            //if (findNodes.Length == 1)
            {
                rightClickedNode.Nodes.Clear();
                CopyTree_rec(rightClickedNode.ParallelNode.Nodes, rightClickedNode.Nodes);

                //Після того як додали всіх дітей потбірно знову помітити ноди, що відповідають фільтру
                MarkedForContains(rightClickedNode.Nodes);
                DrawForFilter(rightClickedNode.Nodes);
                rightClickedNode.Expand();

                //відновлюємо вибрані(чекнуті) елементи
                RecoveryCheckedNodes();
            }
        }

        /// <summary>
        /// Функція скидує всі вибрані ноди в дереві. Для кнопки в контекстному меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tsmi_ClearSelectedList_Click(object sender, EventArgs e)
        {
            SelectedItemsClear();
        }
        #endregion

        #region Функції для фільтрування видимого дерева
        /// <summary>
        /// Поле фільтру отримує фокус
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_filter_GotFocus(object sender, EventArgs e)
        {
            //Коли поле фільтру отримує фокус і в ньому тільки його назва - очищаєм для введеня тексту
            if (tb_filter.Text == " фильтр")
            {
                tb_filter.Text = null;
                tb_filter.ForeColor = filterInFocus_ForeColor;
            }
            tb_filter.BackColor = filterInFocus_BackColor;


        }
        /// <summary>
        /// Поле фільтру втрачає фокус
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_filter_LostFocus(object sender, EventArgs e)
        {
            //Коли поле фільтру втрачає фокус і воно є пустим - значить фіьлтр не потрібен і треба повернуть назву
            if (String.IsNullOrEmpty(tb_filter.Text))
            {
                tb_filter.Text = " фильтр";
                tb_filter.ForeColor = filterNoFocus_ForeColor;
            }
            tb_filter.BackColor = filterNoFocus_BackColor;
        }

        Thread demoThread;
        /// <summary>
        /// зміна тексту в полі фільтру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_filter_TextChanged(object sender, EventArgs e)
        {
            //якщо поле ще пусте (готове для вводу) - ще не фільтруєм
            //після очищення в поле повертається назва ' фильтр' - теж не фільтруєм
            //в інших випадках фільтруєм
            if (String.IsNullOrEmpty(tb_filter.Text) || tb_filter.Text == " фильтр")
            {
                if (isFiltered) //якщо поле вже відфільтроване
                {
                    if (demoThread != null && demoThread.IsAlive)
                        demoThread.Abort();
                    CopyTree(tv_Full, tv_Visible);
                    isFiltered = false; //тепер не відфільтроване

                    RecoverySelectedNode();
                    RecoveryCheckedNodes();
                }
            }
            else
            {
                // Виключає перевідрку і дозволяє звертатися до контролів з різних потоків. мабуть ризиковано, але швидко.
                Control.CheckForIllegalCrossThreadCalls = false;

                if (demoThread != null && demoThread.IsAlive)
                    demoThread.Abort();

                demoThread = new Thread(new ThreadStart(this.FilterTree));
                demoThread.IsBackground = true;
                demoThread.Start();

                isFiltered = true;
            }


        }

        // Пам"ятка по ФІЛЬТРУ!
        // 1. filter
        //      Якщо нод помічений як "noContains", значить він не відповідає фільтру і з ним треба щось зробить. 
        // 2. delete
        //      Якщо в нода з позначкою filter всі діти можуть бути видалені - то цей теж може бути видалений.        //
        // 3. color
        //      Якщо в нода з позначкою filter є діти що мають залишитися після фільтрації, то нод ми не можем видалити, але помічаєм його кольором
        // </summary>

        delegate void dCopyTree(TreeViewExt t1, TreeViewExt t2);
        object locker = new object();


        /// <summary>
        /// Функція фільтрує дерево на основі введеного тексту в поле фільтру.
        /// Функція запускається окремим потоком. Щоб не блокувалися клафіши і можна було миттєво доповнювати фільтр.
        /// </summary>
        private void FilterTree()
        {
            // Два підходи: 
            // Правильно операцію з контролом передавати в основний потік (де створений контрол)
            // але якщо операція ця тривала - форма перестає миттєво реагувати (напр. на натискання клавіш)
            // реагує тільки по завершенні операції.
            //Деякого прискорення додаэ асинхронний метод запуску BeginInvoke
            //Якщо тривалу операцію робити в будь якому потоці - основний потік форми не зайнятий і форма миттєво реагує на натискання.
            //В даному коді тривала операція - розкриття дерева.

            #region Цей код виконує фільтрацію дерева в основному потоці. так правильно, надійно, але довго.
            //Можна включить цей код, закоментувати "локер" і виключить CheckForIllegalCrossThreadCalls
            /*
            if (this.treeView.InvokeRequired)
            {
            //    treeView.Invoke(new myD(FilterTree));
            //    або
                treeView.BeginInvoke(new myD(FilterTree)); //асинхронно і швидше
            }
            else
            {
            */
            #endregion

            //копіювання дерева здійснюєтсья швидко, тому його можна виконати правильно - передавши в основний потік форми.
            if (this.tv_Visible.InvokeRequired)
                tv_Visible.Invoke(new dCopyTree(CopyTree), new Object[] { tv_Full, tv_Visible }); //перенести потім в кінець!!!!!!!
            else
                CopyTree(tv_Full, tv_Visible);

            lock (locker)
            {
                //фільтруємо основне дерево, що відображається в DropDown
                //спочатку маркуємо ноди для видалення і окремо видаляємо помічені

                try
                {
                    System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();


                    //Маркуємо всі ноди що не відповідають фільтру
                    MarkedForContains(tv_Visible.Nodes); //Швидкість 30-40мс

                    //Аналіз помічених нодів. Деякі помічаються на видалення, а деяки на затінення сіреньким кольором.
                    MarkingForDeletes(tv_Visible.Nodes); // 0,1-0,2 мс

                    //перемальовування. деякі робим яскравими а деякі навпаки.
                    DrawForFilter(tv_Visible.Nodes);  // 1-3мс

                    List<TreeNodeExt> deleteList = new List<TreeNodeExt>();
                    GetDeleteList(deleteList, tv_Visible.Nodes); // 0,1 - 0,15 мс

                    //Видаляємо ноди, що в списку на видалення // 200-500мс !!!
                    foreach (TreeNode node in deleteList)
                    {
                        node.Remove();
                    }

                    tv_Visible.ExpandAll();   // 500-1000 мс !!!!

                    //після нового проходу фільтру відновлюєм ноду що була виділена перед цим
                    RecoverySelectedNode();

                    //Відновлюєм ноди, що були вибрані
                    RecoveryCheckedNodes();
                }
                catch (ThreadAbortException)
                {
                    //Це приривання виникає, коли ми зупиняємо потік (Abort()), тому нічого тут не робимо, хай зупиняється
                }
                catch (Exception e) //потім це прибрати мабуть. коли з блокуваннями визначусь
                {
                    MessageBox.Show("Якась помилка під час фільтрування: \r\n" + e.Message);
                }

            }
        }

        /// <summary>
        /// Функція шукає ноди, що не відповідають фільтру і помічає їх.
        /// Рекурсивна. не враховує регістр.
        /// </summary>
        /// <param name="nodes"></param>
        private void MarkedForContains(TreeNodeCollection nodes)
        {
            foreach (TreeNodeExt tne in nodes)
            {
                if (tne != null)
                {
                    if (tne.Text.ToLower().Contains(tb_filter.Text.ToLower()) == false)
                        tne.CompareResult = CompareResult.NoContains;
                    if (tne.Nodes.Count > 0)
                        MarkedForContains(tne.Nodes);
                }
            }
        }

        /// <summary>
        /// Функція аналізує чи можна видаляти знайдені(помічені) ноди чи їх можна тільки притінити.
        /// Ноду можна видалить тоді, якщо вона не відповідає фільтру і в неї немає дітей яких не можна видаляти.
        /// А якщо нода відпов. фільтру і всіх дітей можна видалить - то і цю можна видалить.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Boolean MarkingForDeletes(TreeNodeCollection nodes)
        {
            Boolean isNoDelete = false;

            foreach (TreeNodeExt tne in nodes)
            {
                if (tne.Nodes.Count > 0)
                {
                    Boolean isNoDeleteChildren = MarkingForDeletes(tne.Nodes);
                    if (isNoDeleteChildren == true) //якщо є дочірні, що не видаляються
                    {
                        isNoDelete = true;
                        if (tne.CompareResult == CompareResult.NoContains)
                            tne.CompareResult = CompareResult.DoPaler;
                    }
                    else
                    {
                        if (tne.CompareResult == CompareResult.NoContains)
                            tne.CompareResult = CompareResult.Remove;
                        else
                            isNoDelete = true;
                    }

                }
                else
                {
                    if (tne.CompareResult == CompareResult.NoContains)
                        tne.CompareResult = CompareResult.Remove;
                    else
                        isNoDelete = true;
                }
            }
            return isNoDelete;
        }

        /// <summary>
        /// Функція зафарбовує ноди, що відповідають фільтру
        /// Повинна завжди викликатися після функцій маркування (MarkedContains, MarkingForDeletes)
        /// </summary>
        /// <param name="nodes"></param>
        private void DrawForFilter(TreeNodeCollection nodes)
        {
            //ноди що не відповідають фільтру, але їх не можна видалити - замальовуєм сірим.
            //а ті що відповідають - навпаки яскравим

            foreach (TreeNodeExt tne in nodes)
            {
                if (tne.Nodes.Count > 0)
                    DrawForFilter(tne.Nodes);

                switch (tne.CompareResult)
                {
                    case CompareResult.DoPaler: //ті що не відповідають фільтру, але не видаляютсья.
                        //tne.ForeColor = nodesNoCorrespondFilter_ForeColor;
                        if (tne.Id < 0)
                            tne.ForeColor = nodesCanNotSelected_ForeColor;
                        else
                            tne.ForeColor = nodesCanSelected_ForeColor;

                        break;
                    case CompareResult.Null: //ті що відповідають
                        tne.BackColor = nodesCorrespondFilter_BackColor;
                        break;
                    default:
                        //tne.ForeColor = Color.Black;
                        break;
                }
            }
        }

        /// <summary>
        /// Функція повертає список нодів, що помічені на видалення
        /// </summary>
        /// <param name="deleteList"></param>
        /// <param name="nodes"></param>
        private void GetDeleteList(List<TreeNodeExt> deleteList, TreeNodeCollection nodes)
        {
            foreach (TreeNodeExt tne in nodes)
            {
                if (tne.CompareResult == CompareResult.Remove)
                    deleteList.Add(tne);
                if (tne.Nodes.Count > 0)
                    GetDeleteList(deleteList, tne.Nodes);
            }
        }

        /// <summary>
        /// Функція відновлює фоукс для нода після втрати деревом фокусу.
        /// Відновлюється останній вибраний нод мишкою або клавіатурою
        /// </summary>
        private void RecoverySelectedNode()
        {
            if (lastSelectedNode != null)
            {
                // пошук по обєкту нода tv_Full.FindByEssenceId() не підходить
                //у видимому ми ушкати не можем, деерво може бути відфільтроване.
                //тому що якщо шукати по ноді - то шукається в невидимому дереві, а виділяєтсья потім у видимому.
                //Тобто в lastSelectedNode потрапляє нод з невидимого дерева і ми не можем його в інше дерево присвоювати, 
                //тому шукаємо по lastSelectedNode.Text

                TreeNode tn = TreeManager.FindNode(tv_Visible.Nodes, lastSelectedNode.Text, true);
                if (tn != null)
                {
                    tv_Visible.SelectedNode = tn;
                    tv_Visible.SelectedNode.BackColor = selectedNodeNoFocus_BackColor;
                    tv_Visible.SelectedNode.ForeColor = selectedNodeNoFocus_ForeColor;
                }
            }
        }

        /// <summary>
        /// При зміні фільтра відлюємо ноди, що були вибрані.
        /// Тобто, якщо вибрана нода попала в відфільтроване дерево - то позначаємо, що вона вибрана (чеком)
        /// Відновлюється останні вибрані ноди мишкою або клавіатурою
        /// </summary>
        private void RecoveryCheckedNodes()
        {
            RecoveryCheckedNodes_rec(tv_Visible.Nodes);
        }

        private void RecoveryCheckedNodes_rec(TreeNodeCollection nodes)
        {
            foreach (TreeNodeExt visibleNode in nodes)
            {
                visibleNode.Checked = visibleNode.ParallelNode.Checked;
                visibleNode.ImageIndex = visibleNode.ParallelNode.ImageIndex;
                
                if (visibleNode.Nodes.Count > 0)
                    RecoveryCheckedNodes_rec(visibleNode.Nodes);
            }
        }
        #endregion

        /// <summary>
        /// Функція рахує кількість вибраних нодів в невидимому (повному) дереві
        /// </summary>
        /// <returns></returns>
        private Int32 GetChecedkNodeCount()
        {
            Int32 checkNodeCount = 0;

            checkNodeCount = GetChecedkNodeCount_Rec(tv_Full.Nodes, checkNodeCount);

            return checkNodeCount;
        }
        private Int32 GetChecedkNodeCount_Rec(TreeNodeCollection nodes, Int32 checkCount)
        {
            foreach (TreeNodeExt node in nodes)
            {
                if (node.Nodes.Count > 0)
                    checkCount = GetChecedkNodeCount_Rec(node.Nodes, checkCount);

                if (node.Checked)
                    checkCount++;
            }

            return checkCount;
        }

        #region Копіювання дерев
        /// <summary>
        /// Копіювання дерева із створенням перехрестних посилань для кожного нода
        /// </summary>
        /// <param name="tv_source"></param>
        /// <param name="tv_copy"></param>
        private void CopyTree(TreeViewExt tv_source, TreeViewExt tv_copy)
        {
            tv_copy.Nodes.Clear();

            CopyTree_rec(tv_source.Nodes, tv_copy.Nodes);
        }
        private void CopyTree_rec(TreeNodeCollection source_nodes, TreeNodeCollection copy_nodes)
        {
            foreach (TreeNodeExt tne in source_nodes)
            {
                TreeNodeExt newNode = new TreeNodeExt();
                newNode.Text = tne.Text;
                newNode.Id = tne.Id;
                newNode.FullName = tne.FullName;
                newNode.ShortName = tne.ShortName;
                newNode.Abbreviation = tne.Abbreviation;

                newNode.ImageIndex = tne.ImageIndex;
                newNode.ForeColor = tne.ForeColor;

                //Ця функція не підходить, тому що вона копіює піддерево, дочірні ноди. А нам треба тільки сам нод
                //TreeNodeExt newNode = (TreeNodeExt)tne.Clone();

                copy_nodes.Add(newNode);

                //перехресні посилання
                newNode.ParallelNode = tne;
                tne.ParallelNode = newNode;

                if (source_nodes.Count > 0)
                    CopyTree_rec(tne.Nodes, newNode.Nodes);
            }
        }
        /// <summary>
        /// Функція копіює повне дерево у видиме
        /// Функцію потрібно запустити один раз після створення контрола і його наповнення (наповнення видимого дерева);
        /// Якщо цього не зробити, контрол сам скопіює дерево, але витрачатиме час на перевірку потреби копіювання при розкритті
        /// </summary>
        public void CopyFullToVisibleTree()
        {
            tv_Visible.Nodes.Clear();
            CopyTree_rec(tv_Full.Nodes, tv_Visible.Nodes);
            
        }
        #endregion

        #region Фільтрування повного (невидимого) дерева.
        /// <summary>
        /// Якщо потрібно відфільтрувати повне дерево. І залишити тільки якесь піддерево.
        /// Застосовуєтсья в політиці безпеки. наприклад, якщо за рівнем доступу людина має право бачити тільки деяку частину дерева.
        /// Задаєм Id найстаршого нода, яке потрібно побачити, все інше видаляється і в повному і у видимому дереві.
        /// Якщо дерево не змого відфільтруватися - воно стане пустим!
        /// </summary>
        /// <param name="FilteredNodeId"></param>
        protected void FilterFullTree_old(Int32 filterNodeId)
        {
            if (filterNodeId >= 0)
            {
                TreeNodeExt filterNode = tv_Full.FindByEssenceId(filterNodeId);

                tv_Full.Nodes.Clear();

                if (filterNode != null)
                    tv_Full.Nodes.Add(filterNode);

                CopyFullToVisibleTree();
            }
        }

        /// <summary>
        /// Якщо потрібно відфільтрувати повне дерево. І залишити тільки якесь піддерево.
        /// Застосовуєтсья в політиці безпеки. наприклад, якщо за рівнем доступу людина має право бачити тільки деяку частину дерева.
        /// Задаєм Id найстарших нодів піддерев, які потрібно бачити, все інше видаляється і в повному і у видимому дереві.
        /// Якщо дерево не змого відфільтруватися - воно стане пустим!
        /// Відобразяться тільки ті ноди з дочірніми - які є в масиві. Якщо в масиві є зайві Id, яких немає в дереві - нічого не трапиться, помилки не буде, зайві Id проігноруються
        /// 
        /// </summary>
        /// <param name="filterNodeIds"></param>
        protected void FilterFullTree(Int32[] filterNodeIds)
        {
            //FilteredFullTree_Rec_simple(tv_Full.Nodes, filterNodeIds);
            //FilteredFullTree_Rec_3(tv_Full.Nodes, filterNodeIds, null);
            FilteredFullTree_Rec_4(tv_Full.Nodes, filterNodeIds);
            DeleteEmptyNode(tv_Full.Nodes);

            CopyFullToVisibleTree();
        }

        private void FilteredFullTree_Rec_simple(TreeNodeCollection nodes, Int32[] filterNodeIds)
        {
            List<TreeNodeExt> childNodeList = new List<TreeNodeExt>();
            List<TreeNode> nodeForDelete = new List<TreeNode>();

            Int32 k = 0;
            for (k = 0; k < nodes.Count; k++)
            {
                TreeNode node = nodes[k];
                if (IsInArray(filterNodeIds, ((TreeNodeExt)nodes[k]).Id) == false) //Якщо нода немає в списку відфільтрованих нодів
                {
                    foreach (TreeNodeExt childNode in nodes[k].Nodes)
                    {
                        childNodeList.Add(childNode);
                    }

                    nodeForDelete.Add(nodes[k]);
                }
            }

            foreach (TreeNode node in nodeForDelete)
            {
                node.Remove();
            }

            foreach (TreeNodeExt childNode in childNodeList)
            {
                tv_Full.Nodes.Add(childNode);
            }

            //Якщо ще є такі ноди, яких неповинно бути
            if (nodeForDelete.Count > 0)
                FilteredFullTree_Rec_simple(tv_Full.Nodes, filterNodeIds);
        }

        private void FilteredFullTree_Rec_2(TreeNodeCollection nodes, Int32[] filterNodeIds, TreeNode parentAliveNode)
        {
            List<TreeNodeExt> childNodeList = new List<TreeNodeExt>();
            List<TreeNode> nodeForDelete = new List<TreeNode>();

            Int32 k = 0;
            Int32 count = nodes.Count;

            for (k = 0; k < count; k++)
            {
                if (((TreeNodeExt)nodes[k]).Id >= 0)
                {
                    TreeNode node = nodes[k];
                    if (IsInArray(filterNodeIds, ((TreeNodeExt)nodes[k]).Id) == false) //Якщо нода немає в списку відфільтрованих нодів
                    {
                        
                        foreach (TreeNodeExt childNode in nodes[k].Nodes)
                        {
                            //childNodeList.Add(childNode);

                            if (nodes[k].Parent == null)
                                tv_Full.Nodes.Add(childNode);
                            else
                                //node.Parent.Nodes.Add(childNode);
                                nodes[k].Parent.Nodes.Add(childNode);

                            FilteredFullTree_Rec_2(nodes[k].Nodes, filterNodeIds, nodes[k].Parent);
                        }


                        
                        nodes[k].Remove(); //видаляємо елемент
                        k--;               //Якщо видалили елемент 2, то той що був на 3 місці тепер став на 2 місце, тому повертаємся на крок назад.
                        count--;
                    }
                }
                else
                    FilteredFullTree_Rec_2(nodes[k].Nodes, filterNodeIds, nodes[k]);
            }

            /*
            foreach (TreeNode node in nodeForDelete)
            {
                    node.Remove();
            }

            foreach (TreeNodeExt childNode in childNodeList)
            {
                tv_Full.Nodes.Add(childNode);
            }

            //Якщо ще є такі ноди, яких неповинно бути
            if (nodeForDelete.Count > 0)
                FilteredFullTree_Rec(tv_Full.Nodes, filterNodeIds);
            */
        }

        private void FilteredFullTree_Rec_3(TreeNodeCollection nodes, Int32[] filterNodeIds, TreeNode parentAliveNode)
        {
            List<TreeNodeExt> childNodeList = new List<TreeNodeExt>();
            List<TreeNode> nodeForDelete = new List<TreeNode>();

            Int32 k = 0;
            Int32 count = nodes.Count;

            for (k = 0; k < count; k++)
            {
                TreeNode node = nodes[k]; //темп

                if (((TreeNodeExt)nodes[k]).Id >= 0)
                {
                    if (IsInArray(filterNodeIds, ((TreeNodeExt)nodes[k]).Id) == false) //Якщо нода немає в списку відфільтрованих нодів
                    {
                        FilteredFullTree_Rec_3(nodes[k].Nodes, filterNodeIds, parentAliveNode);

                        nodes[k].Remove(); //видаляємо елемент
                        k--;               //Якщо видалили елемент 2, то той що був на 3 місці тепер став на 2 місце, тому повертаємся на крок назад.
                        count--;
                    }
                    else
                        parentAliveNode.Nodes.Add(nodes[k]);
                    
                }
                else
                    FilteredFullTree_Rec_3(nodes[k].Nodes, filterNodeIds, nodes[k]);
            }
        }

        private void FilteredFullTree_Rec_4(TreeNodeCollection nodes, Int32[] filterNodeIds)
        {
            foreach (TreeNodeExt node in nodes)
            {
                if (node.Id >= 0)
                {
                    if (IsInArray(filterNodeIds, node.Id) == false) //Якщо ноди немає в списку відфільтрованих нодів
                    {
                        node.Id = -1;
                        node.ForeColor = nodesCanNotSelected_ForeColor;
                        FilteredFullTree_Rec_4(node.Nodes, filterNodeIds);
                    }
                }
                else
                    FilteredFullTree_Rec_4(node.Nodes, filterNodeIds);
            }
        }

        /// <summary>
        /// Видалення пустих нодів, тобто нодів, що не являються сутностями (id = -1), і в їх дочірніх елементах немає жодної сутності.
        /// Тобто в цьому піддереві енмає чого вибирати.
        /// </summary>
        /// <param name="nodes"></param>
        private void DeleteEmptyNode(TreeNodeCollection nodes)
        {
            Int32 k = 0;
            Int32 count = nodes.Count;
            for (k = 0; k < count; k++)
            {
                if (nodes[k].Nodes.Count > 0)
                {
                    DeleteEmptyNode(nodes[k].Nodes);
                }
                
                TreeNode node = nodes[k]; //темп
                if (nodes[k].Nodes.Count == 0)
                {
                    if (((TreeNodeExt)nodes[k]).Id < 0)
                    {
                        nodes[k].Remove(); //видаляємо елемент
                        k--;               //Якщо видалили елемент 2, то той що був на 3 місці тепер став на 2 місце, тому повертаємся на крок назад.
                        count--;
                    }
                }
            }
        }

        /// <summary>
        /// Перевіряє чи число входить в масив чисел
        /// </summary>
        /// <param name="array"></param>
        /// <param name="findElem"></param>
        /// <returns></returns>
        private Boolean IsInArray(Int32[] array, Int32 findElem)
        {
            Boolean isIn = false;
            foreach (Int32 elem in array)
            {
                if (elem == findElem)
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        #endregion 
    }

   
    /// <summary>
    /// Делегат для реалізації події зміни значення в контролі
    /// </summary>
    /// <param name="sender">Делегат відправляє один параметр - об"єкт типу ComboTreeBox_Advanced, який показує, який контрол відправив подію</param>
    public delegate void SelectedItemChangedDelegate(ComboTreeBox_Advanced sender);

    /// <summary>
    /// Помилка, що виникає, коли в ноді не прописаний відповідний паралельний нод з "паралельного" дерева
    /// </summary>
    public class ParallelNodeIsNullException : Exception
    {
        public ParallelNodeIsNullException(TreeNodeExt node) : base("в Ноді (" + node.Text + ") не заповнене посилання на паралельну ноду. Мабуть після доповнення повного дерева видиме дерево не оновлювалося. Спробуйте викликати після створення і наповнення контролу функцію CopyFullToVisibleTree") { }
    }

    /// <summary>
    /// переривання виникає, якщо неможливо встановити деякі значення для контролу за допомогою функцій SetSelectedOneItem і SetSelectedmanyItems
    /// </summary>
    public class CanNotBeSetValueException : Exception
    {
        private String message;

        private Int32[] valuesId;
        /// <summary>
        /// Значення Id, які не вдалося знайтив  контролі
        /// </summary>
        public Int32[] ValuesId
        {
            get { return valuesId; }
        }

        private String controlType;
        /// <summary>
        /// Ім"я контрола, в якому не вдалося встановити значення
        /// </summary>
        public String ControlType
        {
            get { return controlType; }
        }

        /// <summary>
        /// Повідомлення переривання
        /// </summary>
        public override string Message
        {
            get { return message; }
        }

        public CanNotBeSetValueException(String controlType, Int32[] valuesId)
        {
            this.valuesId = valuesId;
            this.controlType = controlType;

            message = "Ви намагалися встановити контролу " + controlType + " такі значення: ";
            foreach (Int32 valueId in valuesId)
            {
                message += valueId + ", ";
            }
            message += " В дереві контролу немає елементів з такими Id!";
        }
    }
}

/*
        /// масив Id елементів, які потрібно вибрати в дереві та значенні контрола
        /// </summary>
        /// <param name="itemIds"></param>
        public void SetSelectedManyItems(Int32[] itemIds)
        {
            List<Int32> errorIdList = new List<int>();

            if (isCheckBoxes)
            {
                foreach (Int32 itemId in itemIds)
                {
                    //Заповнюється невидиме дерево
                    TreeNodeExt findNode = tv_Full.FindByEssenceId(itemId);
                    if (findNode != null)
                    {
                        //проставляєм вибрані ноди в невидимому дереві
                        findNode.Checked = true;
                        //TreeViewEventArgs e = new TreeViewEventArgs(findNode, TreeViewAction.ByKeyboard);
                    }
                    else
                        errorIdList.Add(itemId);
                }

                if (errorIdList.Count == 0)
                    //також змінюємо значення контрола на ці ноди, що вибрали в дереві
                    //Вибрані ноди з повного дерева заносяться у список вибраних значень
                    ChangeItem(null);
                else
                {
                    Int32[] errorIdArray = new Int32[errorIdList.Count];
                    Int32 i;
                    for(i = 0; i< errorIdList.Count; i++)
                    {
                        errorIdArray[i] = errorIdList[i];
                    }
                    throw new CanNotBeSetValueException(this.GetType().ToString(), errorIdArray);
                }
            }
            else
                throw new Exception("ComboTreeBox отримав ззовні масив значень, які потрібно вибрати в дереві. Але в дереві вимкнений мультивибір. Скористуйтеся функцією SetSelectedItem(Int32 itemId)");
        }

        /// <summary>
        /// Функція ощичає вибрані користувачем елементи.
        /// Тобто обнуляютясь властивості selectedItem та selectedList, скидає текст контрола та вибраний елемент в дереві!
        /// </summary>
        public void SelectedItemsClear()
        {
            //Скидування значення.
            selectedItem = null;
            selectedList = null;
            //Скидування тексту
            FillText(null);

            #region Скидування дерева
            tv_Visible.SelectedNode = null;
            ResetBothTree();

            //скидування кольору виділеного останній раз ноду
            if (lastSelectedNode != null)
            {
                TreeNode tn = TreeManager.FindNode(tv_Visible.Nodes, lastSelectedNode.Text, true);
                if (tn != null)
                {
                    tn.BackColor = SystemColors.Window; //selectedNodeNoFocus_BackColor;
                    tn.ForeColor = SystemColors.ControlText; //selectedNodeNoFocus_ForeColor;
                }
            }

            tv_Visible.CollapseAll();
            #endregion

        }
        #endregion

        //перехід по вибраним елементам стрілочками  (Обробка натиснення клавіш шифт + трілка)
        void tv_Visible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                //перехід по вибраним елементам стрілочками
                if (isCheckBoxes) //якщо включений мультивибір
                {
                    if (selectedList != null && selectedList.Count > 0) //якщо щось вибрано
                    {
                        Int32 indexOfList = -1;

                        //Чи вибраний нод в дереві є в списку значень контрола
                        SelectedTreeItem findedNode = selectedList.Find(delegate(SelectedTreeItem item)
                        {
                            return item.Id == ((TreeNodeExt)tv_Visible.SelectedNode).Id;
                        });

                        //Якщо ніякий нод зі списку значень в дереві не вибраний
                        if (findedNode == null)
                        {
                            indexOfList = 0;
                        }
                        else //якщо вже вибраний нод зі списку занчень
                        {
                            indexOfList = selectedList.IndexOf(findedNode);

                            if (e.Shift && e.KeyCode == Keys.Up)
                            {
                                if (indexOfList == 0) //останній індех
                                    indexOfList = selectedList.Count - 1;
                                else
                                    indexOfList = indexOfList - 1;
                            }
                            else
                                if (e.Shift && e.KeyCode == Keys.Down)
                                {
                                    if (indexOfList == selectedList.Count - 1) //перший елемент
                                        indexOfList = 0; //тоді вибираєм останній
                                    else
                                        indexOfList = indexOfList + 1;
                                }
                        }

                        tv_Visible.SelectedNode = tv_Visible.FindByEssenceId(selectedList[indexOfList].Id);

                        e.SuppressKeyPress = true;
                    }
                }
            }
        }
                    
        protected override void ShowDropDown()
        {
            base.ShowDropDown();
            tv_Visible.BackColor = treeView_BackColor;

            if (isCheckVisibleTreeOnFill && CheckVisibleTreeOnFill()) //Якщо програміст додав нові ноди і не перестворив видиме дерево - воно заповниться автоматично (якщо ввімкнена ця опція)
            {
                //Якщо знайдений новий нод (в ньому не буде проініціалізований паралельний нод з видимого дерева.)
                //тому потрібно скопіювати поновому повне дерева у видиме. 
                //Заодно відновити попередній вид - до додавання нових нодів можуть бути вибрані ноди - відновлюєм вибрані ноди і якщо був - фільтр
                CopyTree(tv_Full, tv_Visible);

                tb_filter_TextChanged(null, null);
            }

            #region Малювання випадаючого списку в різних режимах, тільки дерево чи дерево + допоміжні панелі
            if (treeView.IsActual)
            {
                borderWidth = 1;
                dropDown.BackColor = Color.Gray;
            }
            else
            {
                borderWidth = 2;
                dropDown.BackColor = tb_NoActualTree.ForeColor;
            }
            
            //Якщо мультивибір включений - відображаєм чекбокс і кнопку
            if (isCheckBoxes)
            {
                if (selectedList == null)
                    selectedList = new List<SelectedTreeItem>();
                selectedItem = null;

                //помічаєм на дереві вибір з основного списку
                fromListToTree();

                //перевірка чи заповнені повні групи,  промальовування піктограмок повних:
                try
                {
                    CheckOnSelectedGroup();
                }
                catch (ParallelNodeIsNullException e)
                {
                    MessageBox.Show(e.Message);
                }

                //Якщо фільтр вимкнуний або нодів мало - фільтр НЕ Відображаєм
                if (showFilterAfterNodeCount < 0)
                {
                    if (treeView.IsActual)
                        ShowMultiOnly();
                    else
                        ShowMultiOnly_NoActual();
                }
                else //Якщо фільтр ввімкнений.
                {
                    //Якщо фільтр пустий і нодів мало - фільтр НЕ Відображаєм
                    if (String.IsNullOrEmpty(tb_filter.Text) && tv_Visible.GetNodeCount(true) < showFilterAfterNodeCount)
                    {
                        if (treeView.IsActual)
                            ShowMultiOnly();
                        else
                            ShowMultiOnly_NoActual();
                    }
                    else //Якщо фільтр не пустий або нодів багато
                    {
                        if (treeView.IsActual)
                            ShowMultiAndFilter();
                        else
                            ShowMultiAndFilter_NoActual();
                    }
                }
            }
            else //Якщо мультивибір ВИключений - НЕ відображаєм чекбокс і кнопку
            {
                selectedList = null;

                //Якщо фільтр вимкнуний або нодів мало - фільтр НЕ Відображаєм
                if (showFilterAfterNodeCount < 0)
                {
                    if (treeView.IsActual)
                        ShowSimple();
                    else
                        ShowSimple_NoActual();
                }
                else //Якщо фільтр ввімкнений.
                {
                    //Якщо фільтр пустий і нодів мало - фільтр НЕ Відображаєм
                    if (String.IsNullOrEmpty(tb_filter.Text) && tv_Visible.GetNodeCount(true) < showFilterAfterNodeCount)
                    {
                        if (treeView.IsActual)
                            ShowSimple();
                        else
                            ShowSimple_NoActual();
                    }
                    else //Якщо фільтр не пустий або нодів багато
                    {
                        if (treeView.IsActual)
                            ShowFilterOnly();
                        else
                            ShowFilterOnly_NoActual();
                    }
                }
            }
            #endregion

            //Якщо в дереві мало елементів - то вони розгортаютсья для зручності
            if (tv_Visible.GetNodeCount(true) <= showFilterAfterNodeCount)
                tv_Visible.ExpandAll();
        }
        #region Функції для відображення різних комбінацій випадаючого списку
        /// <summary>
        /// Функція помічає на дереві ноди, що занесені в список.
        /// Коли дерево розкривається ще раз - треба помітити на ньому ті ноді, що ми вибрали попереднього разу.
        /// (відновити вибір з SelectedList)
        /// </summary>
        void fromListToTree()
        {
            //Скидування дерева перед відновлюванням вибору
            ResetBothTree();

            foreach (SelectedTreeItem sti in selectedList)
            {

                //зі списку відновлюються */