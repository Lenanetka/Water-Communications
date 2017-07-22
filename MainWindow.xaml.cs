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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

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
        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            IDataReaderWriter data = new DataForTest();
            Communications communications = data.ReadFromFile("");
            for (int i = 1; i < communications.stations.Count; i++)
            {
                communications.calculateOptimalK(i);
                data.WriteInFile("", communications, i);
            }
            data.WriteInFile("", communications);
        }

        private void bBrowseLoadPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = tbLoadPath.Text;
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbLoadPath.Text = folderDialog.SelectedPath;

        }

        private void bBrowseSavePath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = tbSavePath.Text;
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbSavePath.Text = folderDialog.SelectedPath;
        }
    }
}
