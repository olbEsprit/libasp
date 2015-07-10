using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebUserControl
{
    /// <summary>
    /// Клас описує вибраний елемент в дереві.
    /// Для сутностей, елементи яких складаються з багатьох нод
    /// </summary>
    public class SelectedTreeItem
    {
        // Приклад з посадами: нода = інженер, піднода = метролог, підпіднода - головний
        //NodeName = "головний"
        //CompoundName = "інженер\\метролог\\головний"
        //FullName = "головний інженер метролог"
        //ShortName = "гол. інж. метрол."
        
        /// <summary>
        /// Id обраного елементу в дереві (не ноди, а всього елементу)
        /// </summary>
        public Int32 Id;

        /// <summary>
        /// Повна назва сутності (назва з "RT таблиці")
        /// 
        /// Приклад з посадами: нода = інженер, піднода = метролог, підпіднода - головний
        /// NodeName = "головний"
        /// CompoundName = "інженер\\метролог\\головний"
        /// FullName = "головний інженер метролог"
        /// ShortName = "гол. інж. метрол."
        /// </summary>
        public String FullName;
        /// <summary>
        /// Скорочена назва сутності (назва з "RT таблиці")
        /// 
        /// Приклад з посадами: нода = інженер, піднода = метролог, підпіднода - головний
        /// NodeName = "головний"
        /// CompoundName = "інженер\\метролог\\головний"
        /// FullName = "головний інженер метролог"
        /// ShortName = "гол. інж. метрол."
        /// </summary>
        public String ShortName;
        /// <summary>
        /// Аббревіатура сутності (назва з "RT таблиці")
        /// 
        /// Приклад з посадами: нода = інженер, піднода = метролог, підпіднода - головний
        /// NodeName = "головний"
        /// CompoundName = "інженер\\метролог\\головний"
        /// FullName = "головний інженер метролог"
        /// ShortName = "гол. інж. метрол."
        /// </summary>
        public String Abbreviation;

        /// <summary>
        /// Назва ноди в дереві.
        /// </summary>
        public String NodeName;
        /// <summary>
        /// Складена назва обраної сутності в дереві.
        /// Назва складаєтсья назв нодів
        /// </summary>
        public String CompoundName;


        /// <summary>
        /// Створення сутності для вибраної ноди
        /// </summary>
        /// <param name="id">Id сутності</param>
        /// <param name="text">Назва сутності з RT табл</param>
        /// <param name="compoundName">Складена з нодів назва сутності </param>
        /// <param name="fullName">Повна назва сутності (з RT таблиці) </param>
        /// <param name="shortName">Скорочена назва сутності (з RT таблиці) </param>
        /// <param name="abbreviation">Абревіатура сутності (з RT таблиці) </param>
        public SelectedTreeItem(Int32 id, String nodeText, String compoundName, String fullName, String shortName, String abbreviation)
        {
            Id = id;
            NodeName = nodeText;
            CompoundName = compoundName;

            FullName = fullName;
            ShortName = shortName;
            Abbreviation = abbreviation;
        }
    }

    /// <summary>
    /// перечислення, що показує, які бувають варіанти відображення вибраного елементу в ComboTreeBox
    /// Тобто після вибору в комбобоксі відображається назва ноду, складена назва чи "повна назва" з RT таблиці
    /// </summary>
    public enum SelectedItemView
    {
        NodeName,
        CompoundName,
        FullName,
    }
}
