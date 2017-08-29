using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WaterCommunications.DataReaderWriter;
using WaterCommunications.Localization;
using MahApps.Metro.Controls.Dialogs;
using QuickGraph;
using System.ComponentModel;
using System.Threading.Tasks;

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
            this.Owner = owner;
            this.Title = title;
            tMessage.Text = message;
            LocalizationUserInterface ui = CurrentLocalization.localizationUserInterface;
            bOk.Content = ui.messages.ok;
            bYes.Content = ui.messages.yes;
            bNo.Content = ui.messages.no;
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
