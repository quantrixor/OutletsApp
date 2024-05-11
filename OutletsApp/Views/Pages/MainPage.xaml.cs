using OutletsApp.Model;
using System;
using System.Collections.Generic;
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
        // Переход в окно добавления данных
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageNomenclaturePage(new Номенклатура()));
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ProductsDataGrid.SelectedItems as Номенклатура;
        }

        // Редактирование данных
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ProductsDataGrid.SelectedItems as Номенклатура;
            if (selectedItem != null)
            {
                NavigationService.Navigate(new ManageNomenclaturePage(selectedItem));
            }
            else
            {
                MessageBox.Show("Выберите запись из списка!", "Внимание! Не правильное действие", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void LoadInventoryData()
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var inventoryQuery = db.Номенклатура
                                       .Include("Магазины")
                                       .Include("Категории").ToList();

                ProductsDataGrid.ItemsSource = inventoryQuery;
            }
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
            if (MessageBox.Show("Вы действительно хотите удалить выбранный объект магазина? Данные будут удалены без возможности восстановления.", "Внимание, подтвердите действие", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                return;
            }

            var selectedItem = StoresDataGrid.SelectedItem as Магазины;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите магазин для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    // Выбираем конкретный объект, который необходимо удалить из списка
                    var itemToDelete = db.Магазины.Find(selectedItem.МагазинID);
                    if (itemToDelete != null)
                    {
                        // Отправляем запрос на удаление данных
                        db.Магазины.Remove(itemToDelete);
                        await db.SaveChangesAsync();
                        MessageBox.Show("Вы успешно удалили выбранную запись.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Обновляем список
                        LoadDataStory();
                    }
                    else
                    {
                        MessageBox.Show("Не найден объект для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка!" + ex.Message, "Внимание, сбой.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataStory();
            LoadInventoryData();
        }

        // Загрзука магазинов
        private void LoadDataStory()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка: " + ex.Message, "Внимание, сбой!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<МагазинDTO> SearchStores(string searchText)
        {
            using (var db = new dbТорговыеТочкиEntities())
            {
                var query = from m in db.Магазины
                            join s in db.Специализации on m.СпециализацияID equals s.СпециализацияID
                            join f in db.ФормыСобственности on m.ФормаСобственностиID equals f.ФормаСобственностиID
                            where m.Название.Contains(searchText) ||
                                  m.Адрес.Contains(searchText) ||
                                  m.Телефоны.Contains(searchText) ||
                                  m.ВремяРаботы.Contains(searchText) ||
                                  s.Описание.Contains(searchText) ||
                                  f.Описание.Contains(searchText)
                            select new
                            {
                                m.МагазинID,
                                m.Название,
                                m.Адрес,
                                m.Телефоны,
                                m.ВремяРаботы,
                                СпециализацияОписание = s.Описание,
                                ФормаСобственностьОписание = f.Описание
                            };

                var result = query.AsEnumerable().Select(x => new МагазинDTO
                {
                    МагазинID = x.МагазинID,
                    Название = x.Название,
                    Адрес = x.Адрес,
                    Телефоны = x.Телефоны,
                    ВремяРаботы = x.ВремяРаботы,
                    СпециализацияОписание = x.СпециализацияОписание,
                    ФормаСобственностьОписание = x.ФормаСобственностьОписание
                }).ToList();

                return result;
            }
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            StoresDataGrid.ItemsSource = SearchStores(SearchBox.Text);
        }

        // Алгоритм нечетного поиска по мере ввода данных

    }
}
