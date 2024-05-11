using OutletsApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OutletsApp.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ManageNomenclaturePage.xaml
    /// </summary>
    public partial class ManageNomenclaturePage : Page
    {
        private Номенклатура _nomenclature { get; set; }
        public ManageNomenclaturePage(Номенклатура nomenclature)
        {
            InitializeComponent();
            _nomenclature = nomenclature;
        }

        private void LoadStores()
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                StoreComboBox.ItemsSource = db.Магазины.ToList();
                StoreComboBox.SelectedValue = _nomenclature.МагазинID;
            }
        }

        private void LoadCategories()
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                CategoryComboBox.ItemsSource = db.Категории.ToList();
                CategoryComboBox.SelectedValue = _nomenclature.КатегорияID;
            }
        }

        private void SetFormData()
        {
            txbName.Text = _nomenclature.НаименованиеТовара;
            txbPrice.Text = _nomenclature.Цена.ToString("F2");
        }

        private void txbPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешить ввод только чисел и точки
            e.Handled = !char.IsDigit(e.Text, 0) && e.Text != ".";
        }

        private void txbPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Дополнительная проверка на случай, если в текстовое поле введены некорректные данные
            if (!decimal.TryParse(txbPrice.Text, out _))
            {
                txbPrice.Text = _nomenclature.Цена.ToString("F2");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    var item = _nomenclature.НоменклатураID == 0 ? new Номенклатура() : db.Номенклатура.FirstOrDefault(n => n.НоменклатураID == _nomenclature.НоменклатураID);

                    if (item == null) // Проверка на случай, если поиск возвращается пустым
                    {
                        MessageBox.Show("Не найден объект для обновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Обновляем или устанавливаем данные
                    item.НаименованиеТовара = txbName.Text;
                    item.МагазинID = (int)StoreComboBox.SelectedValue;
                    item.КатегорияID = (int)CategoryComboBox.SelectedValue;
                    item.Цена = decimal.Parse(txbPrice.Text);

                    if (string.IsNullOrWhiteSpace(txbName.Text) || StoreComboBox.SelectedValue == null || CategoryComboBox.SelectedValue == null || string.IsNullOrWhiteSpace(txbPrice.Text))
                    {
                        MessageBox.Show("Все поля должны быть заполнены.", "Пустые поля", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Дополнительно проверяем корректность введенной цены
                    if (!decimal.TryParse(txbPrice.Text, out decimal parsedPrice))
                    {
                        MessageBox.Show("Введите корректную цену.", "Некорректная цена", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Проверяем, является ли объект новым
                    if (_nomenclature.НоменклатураID == 0)
                    {
                        db.Номенклатура.Add(item);
                    }
                    
                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка: " + ex.Message, "Внимание, сбой!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStores();
            LoadCategories();
            SetFormData();
        }
    }
}
