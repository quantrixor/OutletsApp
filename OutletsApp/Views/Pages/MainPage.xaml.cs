using OutletsApp.Model;
using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace OutletsApp.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddStore_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageStoryPage(new Model.Магазины()));
        }

        private void EditStore_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = StoresDataGrid.SelectedItem as Магазины;
            if (selectedItem != null)
            {
                NavigationService.Navigate(new ManageStoryPage(selectedItem));
            }
            else
            {
                MessageBox.Show("Выберите запись из списка!", "Внимание! Не правильное действие", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Удаление выбранного магазина
        private async void DeleteStore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить выбранный объект магазина? Данные будут удалены без возможности восстановления.", "Внимание, подтвердите действие", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    return;
                }

                // Процедура выбора объекта магазина
                var selectedItem = StoresDataGrid.SelectedItem as Магазины;
                if (selectedItem != null)
                {
                    using (var db = new dbТорговыеТочкиEntities())
                    {
                        db.Магазины.Remove(selectedItem);
                        await db.SaveChangesAsync();
                    }

                    MessageBox.Show("Вы успешно удалили выбранную запись.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка!" + ex.Message, "Внимание, сбой.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StoresDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataStory();
        }

        // Загрзука магазинов
        private void LoadDataStory()
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var stores = db.Магазины
                       .Include("Специализации")
                       .Include("ФормыСобственности")
                       .ToList();

                StoresDataGrid.ItemsSource = stores;
            }
        }
    }
}
