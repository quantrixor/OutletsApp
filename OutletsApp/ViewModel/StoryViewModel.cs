using OutletsApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OutletsApp.ViewModel
{
    public static class StoryViewModel
    {
        // Загрузка данных специализации
        public static void LoadSpecializations(ComboBox SpecializationComboBox)
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var specializations = db.Специализации.ToList();
                SpecializationComboBox.ItemsSource = specializations;
            }
        }

        // Загрузка форм собственности из базы данных
        public static void LoadOwnerships(ComboBox OwnershipComboBox)
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var ownerships = db.Номенклатура.ToList();
                OwnershipComboBox.ItemsSource = ownerships;
            }
        }
    }
}
