// Pages/RequestEditPage.xaml.cs
using System;
using System.Linq;
using System.Windows;

namespace HouseStockApp.Pages
{
    public partial class RequestEditPage : Window
    {
        private readonly HouseStockDBEntities _context;
        private readonly Request _requestToEdit;
        private bool _isEditMode;

        public RequestEditPage(HouseStockDBEntities context, Request request = null)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _requestToEdit = request;
            _isEditMode = request != null;

            if (_isEditMode)
            {
                Title = "Редактировать заявку";
            }

            LoadHouses();
            LoadEmployees();
            LoadStatuses();

            if (_isEditMode)
            {
                PopulateFields();
            }
        }

        private void LoadHouses()
        {
            var houses = _context.Houses.OrderBy(h => h.Address).ToList();
            HouseComboBox.ItemsSource = houses;
            HouseComboBox.DisplayMemberPath = "Address";
            HouseComboBox.SelectedValuePath = "HouseId";

            if (_isEditMode)
            {
                HouseComboBox.SelectedValue = _requestToEdit.Apartment.HouseId;
                HouseComboBox.IsEnabled = false;
                HouseComboBox_SelectionChanged(null, null); // принудительно загрузим квартиры
            }
        }

        private void LoadEmployees()
        {
            var employees = _context.Employees.OrderBy(e => e.FullName).ToList();
            EmployeeComboBox.ItemsSource = employees;
            EmployeeComboBox.DisplayMemberPath = "FullName";
            EmployeeComboBox.SelectedValuePath = "EmployeeId";

            if (_isEditMode && _requestToEdit.EmployeeId.HasValue)
            {
                EmployeeComboBox.SelectedValue = _requestToEdit.EmployeeId.Value;
            }
        }

        private void LoadStatuses()
        {
            var statuses = new[] { "Открыта заявка", "Заявка в работе", "Заявка закрыта" };
            StatusComboBox.ItemsSource = statuses;

            if (_isEditMode)
            {
                StatusComboBox.SelectedItem = _requestToEdit.Status;
            }
            else
            {
                StatusComboBox.SelectedIndex = 0;
            }
        }

        private void HouseComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApartmentComboBox.ItemsSource = null; // очистим перед загрузкой

            if (HouseComboBox.SelectedItem is House selectedHouse)
            {
                var apartments = _context.Apartments
                    .Where(a => a.HouseId == selectedHouse.HouseId)
                    .OrderBy(a => a.ApartmentNumber)
                    .ToList();

                ApartmentComboBox.ItemsSource = apartments;
                ApartmentComboBox.DisplayMemberPath = "ApartmentNumber";
                ApartmentComboBox.SelectedValuePath = "ApartmentId";

                if (_isEditMode)
                {
                    ApartmentComboBox.SelectedValue = _requestToEdit.ApartmentId;
                    ApartmentComboBox.IsEnabled = false;
                }
            }
        }

        private void PopulateFields()
        {
            ApplicantNameBox.Text = _requestToEdit.ApplicantName;
            PhoneBox.Text = _requestToEdit.ContactPhone;
            ProblemBox.Text = _requestToEdit.ProblemDescription;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (HouseComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите дом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ApartmentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите квартиру.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ApplicantNameBox.Text))
            {
                MessageBox.Show("Укажите ФИО заявителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Укажите контактный телефон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProblemBox.Text))
            {
                MessageBox.Show("Опишите проблему.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Request request;
                if (_isEditMode)
                {
                    request = _requestToEdit;
                    request.UpdatedAt = DateTime.Now;
                }
                else
                {
                    request = new Request();
                    request.CreatedAt = DateTime.Now;
                    request.UpdatedAt = DateTime.Now;
                }

                request.ApartmentId = (int)ApartmentComboBox.SelectedValue;
                request.ApplicantName = ApplicantNameBox.Text.Trim();
                request.ContactPhone = PhoneBox.Text.Trim();
                request.ProblemDescription = ProblemBox.Text.Trim();
                request.EmployeeId = EmployeeComboBox.SelectedItem is Employee emp ? emp.EmployeeId : (int?)null;
                request.Status = StatusComboBox.SelectedItem?.ToString() ?? "Открыта заявка";

                if (!_isEditMode)
                {
                    _context.Requests.Add(request);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось сохранить заявку.\n\nОшибка: {ex.Message}",
                    "Ошибка сохранения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}