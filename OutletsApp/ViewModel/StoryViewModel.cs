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
        public static void LoadSpecializations(ComboBox SpecializationComboBox, Action afterLoad)
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var specializations = db.Специализации.ToList();
                SpecializationComboBox.ItemsSource = specializations;
                SpecializationComboBox.DisplayMemberPath = "Описание";
                SpecializationComboBox.SelectedValuePath = "СпециализацияID";
                afterLoad?.Invoke();
            }
        }

        // Загрузка форм собственности из базы данных
        public static void LoadOwnerships(ComboBox OwnershipComboBox, Action afterLoad)
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var ownerships = db.ФормыСобственности.ToList();
                OwnershipComboBox.ItemsSource = ownerships;
                OwnershipComboBox.DisplayMemberPath = "Описание";
                OwnershipComboBox.SelectedValuePath = "ФормаСобственностиID";
                afterLoad?.Invoke();
            }
        }
    }
}
