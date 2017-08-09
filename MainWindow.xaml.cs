using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WaterCommunications.DataReaderWriter;
using WaterCommunications.Localization;

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
                DataFromToCSV data = new DataFromToCSV();
                Communications communications = data.ReadFromFile(tbLoadPath.Text, tbMainStationId.Text);

                communications.h = Convert.ToDouble(tbH.Text);
                communications.hMin = Convert.ToDouble(tbHMin.Text);
                communications.accidentPercent = Convert.ToDouble(tbAccidentPercent.Text) / 100;

                if (mSystemOfUnitsSI.IsChecked == true) communications.systemOfUnits = SystemOfUnits.SI;
                if (mSystemOfUnitsGOST.IsChecked == true) communications.systemOfUnits = SystemOfUnits.GOST;

                for (int i = 1; i < communications.stations.Count; i++)
                {
                    communications.calculateOptimalK(i);
                    if (cbOnlyMainInfo.IsChecked == false) data.WriteInFile(tbSavePath.Text, communications, i, (i == 1) ? true : false);
                }
                data.WriteInFile(tbSavePath.Text, communications, cbOnlyMainInfo.IsChecked == true ? true : false);

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
                if (tbSavePath.Text == "") tbSavePath.Text = fileDialog.FileName.Replace(".csv","-calculated.csv");
            }                          
        }

        private void bBrowseSavePath_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Filter = "csv files (*.csv)|*.csv";          
            fileDialog.FileName = tbSavePath.Text;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbSavePath.Text = fileDialog.FileName;
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

        private void Menu_Coefficients(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_About(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Water Communications v0.5b\nCreated by Lenanetka (c) 2017", "About program");
        }

        private void Menu_How_to_use(object sender, RoutedEventArgs e)
        {

                Help.ShowHelp(null, @"..\..\Resources\Help\index.html");

        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }

        private void tbLoadPath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(File.Exists(@tbLoadPath.Text)) Help.ShowHelp(null, @tbLoadPath.Text);
        }

        private void tbSavePath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (File.Exists(@tbLoadPath.Text)) Help.ShowHelp(null, @tbSavePath.Text);
        }
    }
}
