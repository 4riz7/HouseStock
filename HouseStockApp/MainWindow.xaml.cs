using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.Entity;
using HouseStockApp.Pages;

namespace HouseStockApp
{
    public partial class MainWindow : Window
    {
        private Employee _currentUser;
        private HouseStockDBEntities _context;

        public MainWindow(Employee user)
        {
            InitializeComponent();
            _currentUser = user;
            _context = new HouseStockDBEntities(); // Shared context
            ApplyRolePermissions();
            LoadRequests();
        }

        // Default constructor
        public MainWindow()
        {
            InitializeComponent();
            _context = new HouseStockDBEntities();
            LoadRequests();
        }

        private void ApplyRolePermissions()
        {
            if (_currentUser == null) return;

            this.Title = $"Управление заявками - {_currentUser.FullName} ({_currentUser.Position})";
        }

        private void LoadRequests()
        {
            try
            {
                // Reload data from DB
                // Since we use a persistent context, we might want to refresh.
                // But for now just query.
                var requests = _context.Requests
                    .Include("Apartment")
                    .Include("Apartment.House")
                    .Include("Employee")
                     // Use string names if lambda include fails or for safety with older EF6
                    .ToList();

                RequestsListBox.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void RequestsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var request = RequestsListBox.SelectedItem as Request;
            if (request != null)
            {
                OpenEditPage(request);
            }
        }

        private void OpenEditPage(Request request = null)
        {
            // Pass the SHARED context so the entity remains attached
            var editWindow = new RequestEditPage(_context, request);
            
            if (editWindow.ShowDialog() == true)
            {
                 LoadRequests();
            }
        }

        private void OnAddRequestClick(object sender, RoutedEventArgs e)
        {
             OpenEditPage(null);
        }

        private void OnHistoryByAddressClick(object sender, RoutedEventArgs e)
        {
            var request = RequestsListBox.SelectedItem as Request;
            if (request == null)
            {
                MessageBox.Show("Выберите заявку, чтобы посмотреть историю по её адресу.");
                return;
            }

            // Need new context? Pages seem to own context or depend on it being passed?
            // Page constructors take context. Let's pass the shared one? 
            // Pages usually readonly for history. Shared is fine.
            new HistoryByAddressPage(_context, request.Apartment.House.Address).ShowDialog();
        }

        private void OnHistoryByEmployeeClick(object sender, RoutedEventArgs e)
        {
            var request = RequestsListBox.SelectedItem as Request;
            /* 
             * Logic: 
             * If request has an employee, show that employee's history.
             * If not, maybe show current user's history? 
             * Let's Assume Request's Employee. 
             */
             
            if (request == null)
            {
                 MessageBox.Show("Выберите заявку.");
                 return;
            }

            if (request.EmployeeId == null)
            {
                MessageBox.Show("У этой заявки нет исполнителя.");
                return;
            }

            new HistoryByEmployeePage(_context, request.EmployeeId.Value).ShowDialog();
        }

        private void OnDeleteRequestClick(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null && _currentUser.Position.IndexOf("Диспетчер", StringComparison.OrdinalIgnoreCase) < 0)
            {
                MessageBox.Show("У вас нет прав на удаление заявок.");
                return;
            }

            var request = RequestsListBox.SelectedItem as Request;
            if (request == null)
            {
                MessageBox.Show("Выберите заявку для удаления.");
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить заявку?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Requests.Remove(request);
                    _context.SaveChanges();
                    LoadRequests();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message);
                }
            }
        }

        private void OnViewAddressesClick(object sender, RoutedEventArgs e)
        {
             new AddressesPage(_context).ShowDialog();
        }

        private void OnViewEmployeesClick(object sender, RoutedEventArgs e)
        {
             new EmployeesPage(_context).ShowDialog();
        }
    }
}