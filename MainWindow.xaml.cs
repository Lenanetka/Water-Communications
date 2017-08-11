using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WaterCommunications.DataReaderWriter;

namespace WaterCommunications
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tbLoadPath.TextChanged += TbLoadPath_TextChanged;
            tbSavePath.TextChanged += TbSavePath_TextChanged;
            checkValidPath();
        }

        private void TbSavePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkValidPath();
        }

        private void TbLoadPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkValidPath();
        }

        private void checkValidPath()
        {
            if (File.Exists(tbLoadPath.Text) && tbSavePath.Text != "")
            {
                bCalculate.IsEnabled = true;
                mCalculate.IsEnabled = true;
            }
                
            else
            {
                bCalculate.IsEnabled = false;
                mCalculate.IsEnabled = false;
            }
        }

        private void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IDataReaderWriter data = new DataFromToCSV();
                List<Station> stations = data.ReadFromFile(tbLoadPath.Text);
                stations.Insert(0, new Station(Convert.ToInt32(tbMainStationId.Text)));

                Communications communications = new Communications(stations);

                communications.h = Convert.ToDouble(tbH.Text);
                communications.hMin = Convert.ToDouble(tbHMin.Text);
                communications.accidentPercent = Convert.ToDouble(tbAccidentPercent.Text) / 100;
                communications.repairSectionMinimumLength = Convert.ToDouble(tbRepairSectionMinimumLength.Text);

                communications.checkData();

                if (tbSavePath.Text.Contains(".xlsx")) data = new DataFromToXML();

                for (int i = 1; i < communications.stations.Count; i++)
                {
                    communications.calculateOptimalK(i);
                    if (cbOnlyMainInfo.IsChecked == false) data.WriteInFile(tbSavePath.Text, communications.stations, i, (i == 1) ? true : false);
                }
                if (cbOnlyMainInfo.IsChecked == true) data.WriteInFile(tbSavePath.Text, communications.stations, true);

                System.Windows.MessageBox.Show("Calculating was finished", "Finish");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }          
        }

        private void bBrowseLoadPath_Click(object sender, RoutedEventArgs e)
        {
            
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "csv files (*.csv)|*.csv";
            fileDialog.FileName = tbLoadPath.Text;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbLoadPath.Text = fileDialog.FileName;
                Properties.Settings.Default.tbLoadPath = tbLoadPath.Text;
            }             
        }

        private void bBrowseSavePath_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csv files (*.csv)|*.csv|Microsort Exel (*.xlsx)|*.xlsx";          
            fileDialog.FileName = tbSavePath.Text;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbSavePath.Text = fileDialog.FileName;
                Properties.Settings.Default.tbSavePath = tbSavePath.Text;
            }
                
        }

        private void Menu_Open(object sender, RoutedEventArgs e)
        {
            bBrowseLoadPath_Click(sender, e);
        }

        private void Menu_Calculate(object sender, RoutedEventArgs e)
        {
            bCalculate_Click(sender, e);
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void checkOnlyOneMenu(object sender)
        {
            foreach (System.Windows.Controls.MenuItem m in ((System.Windows.Controls.MenuItem)((System.Windows.Controls.MenuItem)sender).Parent).Items)
            {
                m.IsChecked = false;
            }
            ((System.Windows.Controls.MenuItem)sender).IsChecked = true;
        }

        private void Menu_Language_English(object sender, RoutedEventArgs e)
        {
            checkOnlyOneMenu(sender);
        }

        private void Menu_Language_Russian(object sender, RoutedEventArgs e)
        {
            checkOnlyOneMenu(sender);
        }
        
        private void Menu_Units_SI(object sender, RoutedEventArgs e)
        {
            checkOnlyOneMenu(sender);
        }

        private void Menu_Units_GOST(object sender, RoutedEventArgs e)
        {
            checkOnlyOneMenu(sender);
        }

        private void Menu_AboutProgram(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((Environment.CurrentDirectory + @"\Help\index.html"));

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }

        private void tbLoadPath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (File.Exists(@tbLoadPath.Text)) System.Diagnostics.Process.Start(@tbLoadPath.Text);
        }

        private void tbSavePath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (File.Exists(@tbLoadPath.Text)) System.Diagnostics.Process.Start(tbSavePath.Text);
        }
    }
}
