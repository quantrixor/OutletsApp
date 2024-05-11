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

        private void txbPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txbPrice_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

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
    }
}
