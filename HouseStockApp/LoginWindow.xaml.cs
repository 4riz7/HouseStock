using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HouseStockApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (var db = new HouseStockDBEntities())
                {
                    EmployeeComboBox.ItemsSource = db.Employees.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к БД: " + ex.Message);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeeComboBox.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника.");
                return;
            }

            string inputPassword = PasswordField.Password;

            // In a real app, hash password. Here we compare plain text as per simplistic requirements or mock.
            // If password in DB is null, allow any password (or require empty).
            // Let's implement logic: if DB password is set, check it. If not, allow "123" or empty.
            // But since I just added the column, all are NULL. 
            // I'll update the first user to have a password during check? No.
            // I will accept ANY result if DB password is null, or enforce '123' if null.
            
            bool isPasswordCorrect = false;

            using (var db = new HouseStockDBEntities())
            {
                var emp = db.Employees.FirstOrDefault(u => u.EmployeeId == selectedEmployee.EmployeeId);
                if (emp != null)
                {
                     if (string.IsNullOrEmpty(emp.Password)) 
                     {
                         // Temporary: Allow login if password is not set in DB yet, or maybe set it?
                         // Let's create a "Setup" mode implicitly. If null, update it?
                         // No, that's unsafe.
                         // Let's just say if null, password must be "123".
                         isPasswordCorrect = inputPassword == "123";
                     }
                     else
                     {
                         isPasswordCorrect = emp.Password == inputPassword;
                     }
                }
            }

            if (isPasswordCorrect)
            {
                // Open Main Window
                MainWindow mainWindow = new MainWindow(selectedEmployee); 
                // MainWindow needs a constructor update to accept User!
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль! (По умолчанию: 123)");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        // Drag window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
