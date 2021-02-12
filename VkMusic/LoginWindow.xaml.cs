using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VkMusic
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void ClearForm(object sender, RoutedEventArgs e)
        {
            TextBox selectedBox = (TextBox)sender;
            if (selectedBox.Text == "Email или номер" || selectedBox.Text == "Пароль")
                selectedBox.Clear();
        }
        private void RestoreLogin(object sender, RoutedEventArgs e)
        {
            if (loginBox.Text == "")
                loginBox.Text = "Email или номер";
        }
        private void RestorePass(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Text == "")
                passwordBox.Text = "Пароль";
        }
        private void AcceptLogin(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        public string Login => loginBox.Text;
        public string Password => passwordBox.Text;
    }
}
