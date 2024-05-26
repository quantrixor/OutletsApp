using OutletsApp.Model;
using OutletsApp.ViewModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace OutletsApp.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ManageStoryPage.xaml
    /// </summary>
    public partial class ManageStoryPage : Page
    {
        private МагазинDTO _store { get; set; }

        public ManageStoryPage(МагазинDTO store)
        {
            InitializeComponent();
            _store = store ?? throw new ArgumentNullException(nameof(store));
            this.DataContext = _store;
            LoadFormData();
        }

        private void LoadFormData()
        {
            // Загрузка значений в поля формы
            txbName.Text = _store.Название;
            txbAddress.Text = _store.Адрес;
            txbPhoneNumber.Text = _store.Телефоны;
            WorkTimeTextBox.Text = _store.ВремяРаботы;

            // Загрузка списков с callback для установки выбранных значений
            StoryViewModel.LoadSpecializations(SpecializationComboBox, () =>
                SpecializationComboBox.SelectedValue = _store.СпециализацияID);
            StoryViewModel.LoadOwnerships(OwnershipComboBox, () =>
                OwnershipComboBox.SelectedValue = _store.ФормаСобственностиID);

            // Выбор текущих значений в комбобоксах
            Dispatcher.Invoke(() =>
            {
                SpecializationComboBox.SelectedValue = _store.СпециализацияID;
                OwnershipComboBox.SelectedValue = _store.ФормаСобственностиID;
            });
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new dbТорговыеТочкиEntities())
                {
                    Магазины storeEntity;

                    if (_store.МагазинID == 0)
                    {
                        storeEntity = new Магазины();
                        db.Магазины.Add(storeEntity);
                    }
                    else
                    {
                        storeEntity = db.Магазины.Find(_store.МагазинID);
                        if (storeEntity == null)
                        {
                            MessageBox.Show("Не удалось найти запись для обновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(txbName.Text) ||
                        string.IsNullOrWhiteSpace(txbAddress.Text) ||
                        string.IsNullOrWhiteSpace(txbPhoneNumber.Text) ||
                        string.IsNullOrWhiteSpace(WorkTimeTextBox.Text))
                    {
                        MessageBox.Show("Заполните все поля, они являются обязательными!", "Пустые значения недопустимы!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var workTime = WorkTimeTextBox.Text;
                    if (!IsValidTime(workTime))
                    {
                        MessageBox.Show("Время работы должно быть в формате 'с HH:MM по HH:MM' и быть в допустимом диапазоне!", "Неверный формат времени", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    storeEntity.Название = txbName.Text;
                    storeEntity.Адрес = txbAddress.Text;
                    storeEntity.Телефоны = txbPhoneNumber.Text;
                    storeEntity.ВремяРаботы = workTime;

                    if (SpecializationComboBox.SelectedItem != null)
                    {
                        storeEntity.СпециализацияID = (int)SpecializationComboBox.SelectedValue;
                    }
                    else
                    {
                        MessageBox.Show("Выберите специализацию из списка!", "Внимание! Недопустимое значение", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    if (OwnershipComboBox.SelectedItem != null)
                    {
                        storeEntity.ФормаСобственностиID = (int)OwnershipComboBox.SelectedValue;
                    }
                    else
                    {
                        MessageBox.Show("Выберите форму собственности из списка!", "Внимание! Недопустимое значение", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
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

        // Проверка вводимого времени на валидность
        private bool IsValidTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time)) return false;

            // Регулярное выражение для проверки формата "с HH:MM по HH:MM"
            var regex = new Regex(@"^с (?<startHour>\d{2}):(?<startMinute>\d{2}) по (?<endHour>\d{2}):(?<endMinute>\d{2})$");
            var match = regex.Match(time);

            if (!match.Success) return false;

            int startHour = int.Parse(match.Groups["startHour"].Value);
            int startMinute = int.Parse(match.Groups["startMinute"].Value);
            int endHour = int.Parse(match.Groups["endHour"].Value);
            int endMinute = int.Parse(match.Groups["endMinute"].Value);

            // Проверка диапазонов
            if (startHour < 0 || startHour > 23 || endHour < 0 || endHour > 23 ||
                startMinute < 0 || startMinute > 59 || endMinute < 0 || endMinute > 59)
            {
                return false;
            }

            return true;
        }

        // Накладываем маску ввода номера телефона
        private void txbPhoneNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;
            string text = textBox.Text + e.Text;

            if (text.Length == 1)
            {
                textBox.Text = "+7 (";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 8)
            {
                textBox.Text += ") ";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 13 || text.Length == 16)
            {
                textBox.Text += "-";
                textBox.SelectionStart = textBox.Text.Length;
            }

            if (textBox.Text.Length >= 18)
            {
                e.Handled = true;
            }
        }
    }

}
