using System;
using System.Windows;
using WaterCommunications.Localization;

namespace WaterCommunications
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message
    {
        public Message(Window owner, String title, String message)
        {
            createWindow(owner, title, message, false);
        }
        public Message(Window owner, String title, String message, bool dialogResultMode)
        {
            createWindow(owner, title, message, dialogResultMode);
        }
        private void createWindow(Window owner, String title, String message, bool dialogResultMode)
        {                      
            InitializeComponent();
            this.DataContext = Languages.current;
            this.Owner = owner;
            this.Title = title;
            tMessage.Text = message;
            Languages ui = Languages.current;
            if (dialogResultMode == true)
            {                       
                bOk.Visibility = Visibility.Hidden;
                bYes.Visibility = Visibility.Visible;
                bNo.Visibility = Visibility.Visible;
            }
            else
            {
                bOk.Visibility = Visibility.Visible;
                bYes.Visibility = Visibility.Hidden;
                bNo.Visibility = Visibility.Hidden;
            }
        }
        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void bNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
