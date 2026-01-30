// Pages/EmployeesPage.xaml.cs
using System;
using System.Linq;
using System.Windows;

namespace HouseStockApp.Pages
{
    public partial class EmployeesPage : Window
    {
        private readonly HouseStockDBEntities _context;

        public EmployeesPage(HouseStockDBEntities context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _context.Employees.OrderBy(e => e.FullName).ToList();
                EmployeesListView.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить список сотрудников.\n\nОшибка: {ex.Message}",
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