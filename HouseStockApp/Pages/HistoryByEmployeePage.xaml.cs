// Pages/HistoryByEmployeePage.xaml.cs
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace HouseStockApp.Pages
{
    public partial class HistoryByEmployeePage : Window
    {
        private readonly HouseStockDBEntities _context;
        private readonly int _employeeId;

        public HistoryByEmployeePage(HouseStockDBEntities context, int employeeId)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _employeeId = employeeId;

            // Получаем ФИО сотрудника для заголовка
            var employee = _context.Employees.Find(_employeeId);
            if (employee != null)
            {
                EmployeeTitleBlock.Text = $"История заявок: {employee.FullName}";
            }
            else
            {
                EmployeeTitleBlock.Text = "История заявок: неизвестный сотрудник";
            }

            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                var requests = _context.Requests
                    .Where(r => r.EmployeeId == _employeeId)
                    .Include("Apartment.House")
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                if (requests.Count == 0)
                {
                    MessageBox.Show(
                        "У данного сотрудника пока нет назначенных заявок.",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }

                HistoryListView.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить историю заявок.\n\nОшибка: {ex.Message}",
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}