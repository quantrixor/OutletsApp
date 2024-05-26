using OutletsApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Удаление товара из базы данных
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный объект номенклатуры? Данные будут удалены без возможности восстановления.", "Внимание, подтвердите действие", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                return;
            }

            var selectedItem = ProductsDataGrid.SelectedItem as Номенклатура;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    var itemToDelete = db.Номенклатура.Find(selectedItem.НоменклатураID);
                    if (itemToDelete != null)
                    {
                        db.Номенклатура.Remove(itemToDelete);
                        await db.SaveChangesAsync();
                        MessageBox.Show("Вы успешно удалили выбранную запись.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Обновляем данные на экране
                        LoadInventoryData();
                    }
                    else
                    {
                        MessageBox.Show("Не найден объект для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка: " + ex.Message, "Внимание, сбой.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Переход в окно редактирования данных
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ProductsDataGrid.SelectedItem as Номенклатура;
            if (selectedItem != null)
            {
                NavigationService.Navigate(new ManageNomenclaturePage(selectedItem));
            }
            else
            {
                MessageBox.Show("Выберите запись из списка!", "Внимание! Неправильное действие", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Загрузка данных номенклатуры
        public void LoadInventoryData()
        {
            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    var inventoryQuery = db.Номенклатура
                                           .Include("Магазины")
                                           .Include("Категории").ToList();

                    ProductsDataGrid.ItemsSource = inventoryQuery;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Переход в окно добавления магазина
        private void AddStore_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageStoryPage(new МагазинDTO()));
        }

        // Переход в окно редактирования магазина
        private void EditStore_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = StoresDataGrid.SelectedItem as МагазинDTO;
            if (selectedItem != null)
            {
                NavigationService.Navigate(new ManageStoryPage(selectedItem));
            }
            else
            {
                MessageBox.Show("Выберите запись из списка!", "Внимание! Неправильное действие", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Удаление выбранного магазина
        private async void DeleteStore_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранный объект магазина? Данные будут удалены без возможности восстановления.", "Внимание, подтвердите действие", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                return;
            }

            var selectedItem = StoresDataGrid.SelectedItem as МагазинDTO;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите магазин для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    // Найдите реальный объект из базы данных по его ID
                    var itemToDelete = db.Магазины.Find(selectedItem.МагазинID);
                    if (itemToDelete != null)
                    {
                        // Удалите объект
                        db.Магазины.Remove(itemToDelete);
                        await db.SaveChangesAsync();
                        MessageBox.Show("Вы успешно удалили выбранную запись.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Обновляем список магазинов
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
                MessageBox.Show("Произошла системная ошибка: " + ex.Message, "Внимание, сбой.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка данных при загрузке страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataStory();
                LoadInventoryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка данных магазинов
        public void LoadDataStory()
        {
            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    var query = from m in db.Магазины
                                join s in db.Специализации on m.СпециализацияID equals s.СпециализацияID
                                join f in db.ФормыСобственности on m.ФормаСобственностиID equals f.ФормаСобственностиID
                                select new МагазинDTO
                                {
                                    МагазинID = m.МагазинID,
                                    Название = m.Название,
                                    Адрес = m.Адрес,
                                    Телефоны = m.Телефоны,
                                    ВремяРаботы = m.ВремяРаботы,
                                    СпециализацияОписание = s.Описание,
                                    ФормаСобственностьОписание = f.Описание
                                };

                    StoresDataGrid.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных магазинов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Поиск магазинов по общему тексту
        public List<МагазинDTO> SearchStores(string searchText)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при поиске магазинов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<МагазинDTO>();
            }
        }

        // Поиск магазинов по товарам
        public List<МагазинDTO> SearchStoresByProduct(string searchText)
        {
            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    // Основной запрос для получения магазинов, содержащих указанный товар
                    var query = from n in db.Номенклатура
                                join m in db.Магазины on n.МагазинID equals m.МагазинID
                                join s in db.Специализации on m.СпециализацияID equals s.СпециализацияID
                                join f in db.ФормыСобственности on m.ФормаСобственностиID equals f.ФормаСобственностиID
                                where n.НаименованиеТовара.Contains(searchText)
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

                    // Преобразуем запрос в список
                    var result = query
                                 .AsEnumerable()
                                 .GroupBy(x => x.МагазинID) // Группируем по ID магазина
                                 .Select(g => g.First()) // Берем первый элемент из каждой группы
                                 .Select(x => new МагазинDTO
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
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при поиске магазинов по товару: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<МагазинDTO>();
            }
        }

        // Обработчик для кнопки поиска по тексту
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StoresDataGrid.ItemsSource = SearchStores(SearchBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении поиска: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для кнопки обновления данных
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataStory();
                SearchBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для кнопки поиска по товарам
        private void btnSearchProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StoresDataGrid.ItemsSource = SearchStoresByProduct(SearchBoxProduct.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении поиска по товарам: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBoxProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
