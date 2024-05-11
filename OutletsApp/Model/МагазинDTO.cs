using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutletsApp.Model
{
    public class МагазинDTO
    {
        public int МагазинID { get; set; }
        public string Название { get; set; }
        public string Адрес { get; set; }
        public string Телефоны { get; set; }
        public string ВремяРаботы { get; set; }
        public string СпециализацияОписание { get; set; }
        public string ФормаСобственностьОписание { get; set; }
    }

}
