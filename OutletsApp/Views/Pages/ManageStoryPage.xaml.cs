using OutletsApp.Model;
using OutletsApp.ViewModel;
using System;
using System.Data.Entity;
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
            _store = store;
            this.DataContext = this;
            LoadFormData();
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
                        // Создаем новую запись
                        storeEntity = new Магазины();
                        db.Магазины.Add(storeEntity);
                    }
                    else
                    {
                        // Находим существующую запись
                        storeEntity = db.Магазины.Find(_store.МагазинID);
                        if (storeEntity == null)
                        {
                            MessageBox.Show("Не удалось найти запись для обновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Проверка заполнения всех обязательных полей
                    if (string.IsNullOrWhiteSpace(txbName.Text) ||
                        string.IsNullOrWhiteSpace(txbAddress.Text) ||
                        string.IsNullOrWhiteSpace(txbPhoneNumber.Text) ||
                        string.IsNullOrWhiteSpace(txbTimeWorking.Text))
                    {
                        MessageBox.Show("Заполните все поля, они являются обязательными!", "Пустые значения недопустимы!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Обновляем данные из формы
                    storeEntity.Название = txbName.Text;
                    storeEntity.Адрес = txbAddress.Text;
                    storeEntity.Телефоны = txbPhoneNumber.Text;

                    if (SpecializationComboBox.SelectedItem != null)
                    {
                        storeEntity.СпециализацияID = ((Специализации)SpecializationComboBox.SelectedItem).СпециализацияID;
                    }
                    else
                    {
                        MessageBox.Show("Выберите специализацию из списка!", "Внимание! Недопустимое значение", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    if (OwnershipComboBox.SelectedItem != null)
                    {
                        storeEntity.ФормаСобственностиID = ((ФормыСобственности)OwnershipComboBox.SelectedItem).ФормаСобственностиID;
                    }
                    else
                    {
                        MessageBox.Show("Выберите форму собственности из списка!", "Внимание! Недопустимое значение", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    storeEntity.ВремяРаботы = txbTimeWorking.Text;

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

        private void LoadFormData()
        {
            // Загрузка значений в поля формы
            txbName.Text = _store.Название;
            txbAddress.Text = _store.Адрес;
            txbPhoneNumber.Text = _store.Телефоны;
            txbTimeWorking.Text = _store.ВремяРаботы;

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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
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

        private void TimeTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Разрешаем ввод только чисел и двоеточия
            e.Handled = !char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ":" && e.Text != e.Text;
        }

        private void txbTimeWorking_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            int cursorPosition = textBox.SelectionStart;
            string text = textBox.Text;
            string newText = string.Empty;

            foreach (char c in text)
            {
                // Принимаем только цифры и двоеточия
                if (char.IsDigit(c) || c == ':' && !newText.Contains(":"))
                {
                    newText += c;
                }
            }

            // Разбиение и проверка корректности времени
            string[] parts = newText.Split(':');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int hours) && hours > 23)
                    parts[0] = "23"; // Корректировка часов
                if (int.TryParse(parts[1], out int minutes) && minutes > 59)
                    parts[1] = "59"; // Корректировка минут
                newText = parts[0] + ":" + parts[1];
            }

            textBox.Text = newText;
            textBox.SelectionStart = cursorPosition > textBox.Text.Length ? textBox.Text.Length : cursorPosition;
        }

        private void txbTimeWorking_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }
    }
}
