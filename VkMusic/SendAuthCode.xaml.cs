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
    /// Логика взаимодействия для SendAuthCode.xaml
    /// </summary>
    public partial class SendAuthCode : Window
    {
        public SendAuthCode()
        {
            InitializeComponent();
        }
        private void ClearForm(object sender, RoutedEventArgs e)
        {
            if (AuthCode.Text == "Введите код...")
                AuthCode.Clear();
        }
        private void RestoreCodeBox(object sender, RoutedEventArgs e)
        {
            if (AuthCode.Text == "")
                AuthCode.Text = "Введите код...";
        }
        private void SendCode(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        public string TwoAuthCode => AuthCode.Text;
    }
}
