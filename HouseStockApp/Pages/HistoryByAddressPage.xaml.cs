// Pages/HistoryByAddressPage.xaml.cs
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace HouseStockApp.Pages
{
    public partial class HistoryByAddressPage : Window
    {
        private readonly HouseStockDBEntities _context;
        private readonly string _address;

        public HistoryByAddressPage(HouseStockDBEntities context, string address)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _address = address ?? throw new ArgumentNullException(nameof(address));

            AddressTitleBlock.Text = $"История заявок: {_address}";
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                var requests = _context.Requests
                    .Where(r => r.Apartment.House.Address == _address)
                    .Include("Apartment")
                    .Include("Employee")
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                if (requests.Count == 0)
                {
                    MessageBox.Show(
                        $"По адресу \"{_address}\" ещё нет заявок.",
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