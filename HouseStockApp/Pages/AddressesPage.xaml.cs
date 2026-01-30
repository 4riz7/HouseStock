// Pages/AddressesPage.xaml.cs
using System;
using System.Linq;
using System.Windows;

namespace HouseStockApp.Pages
{
    public partial class AddressesPage : Window
    {
        private readonly HouseStockDBEntities _context;

        public AddressesPage(HouseStockDBEntities context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            LoadAddresses();
        }

        private void LoadAddresses()
        {
            try
            {
                var houses = _context.Houses.OrderBy(h => h.Address).ToList();
                AddressesListView.ItemsSource = houses;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить список адресов.\n\nОшибка: {ex.Message}",
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