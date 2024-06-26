//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OutletsApp.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Магазины
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Магазины()
        {
            this.Номенклатура = new HashSet<Номенклатура>();
        }
    
        public int МагазинID { get; set; }
        public string Название { get; set; }
        public string Адрес { get; set; }
        public string Телефоны { get; set; }
        public int СпециализацияID { get; set; }
        public int ФормаСобственностиID { get; set; }
        public Специализации Специализация { get; set; } // Навигационное свойство
        public ФормыСобственности ФормаСобственности { get; set; } // Навигационное свойство
        public string ВремяРаботы { get; set; }
    
        public virtual Специализации Специализации { get; set; }
        public virtual ФормыСобственности ФормыСобственности { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Номенклатура> Номенклатура { get; set; }
    }
}
