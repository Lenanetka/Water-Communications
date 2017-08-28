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

namespace WaterCommunications
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow: INotifyPropertyChanged
    {       
        public MainWindow()
        {
            InitializeComponent();
            bindingLocalization();
            tbLoadPath.TextChanged += TbLoadPath_TextChanged;
            tbSavePath.TextChanged += TbSavePath_TextChanged;
            checkValidPath();
        }
        private void bindingLocalization()
        {
            LocalizationUserInterface ui = CurrentLocalization.localizationUserInterface;
            LocalizationSystemOfUnits sou = CurrentLocalization.localizationSystemOfUnits;

            tiMain.Header = ui.mainPage.tiMain;
            tiGraph.Header = ui.mainPage.tiGraph;

            mFile.Header = ui.menu.file;
            mOpen.Header = ui.menu.file_open;
            mCalculate.Header = ui.menu.file_calculate;
            mExit.Header = ui.menu.file_exit;
            mSettings.Header = ui.menu.settings;
            mLanguage.Header = ui.menu.settings_language;
            mLanguageEnglish.Header = ui.menu.settings_language_english;
            mLanguageRussian.Header = ui.menu.settings_language_russian;
            mHelp.Header = ui.menu.help;
            mAbout.Header = ui.menu.help_about;

            labelLoad.Content = ui.mainPage.labelSave;
            tbLoadPath.ToolTip = ui.mainPage.tooltipLoadSave;
            labelSave.Content = ui.mainPage.labelSave;
            tbSavePath.ToolTip = ui.mainPage.tooltipLoadSave;
            labelSourceId.Content = ui.mainPage.labelSourceId;
            labelH.Content = ui.mainPage.labelH;
            LabelHMin.Content = ui.mainPage.LabelHMin;
            LabelAccidentPercent.Content = ui.mainPage.LabelAccidentPercent;
            LabelRepairSectionMinimumLength.Content = ui.mainPage.LabelRepairSectionMinimumLength;
            cbOnlyMainInfo.Content = ui.mainPage.cbOnlyMainInfo;
            bCalculate.Content = ui.mainPage.bCalculate;

            tbHMeasurement.Content = sou.m;
            tbHMinMeasurement.Content = sou.m;
            tbAccidentPercentMeasurement.Content = sou.percent;
            LabelRepairSectionMinimumLengthMeasurement.Content = sou.km;
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
        private IDataReaderWriter getDataReaderWriter(String path)
        {
            IDataReaderWriter data = null;
            String extension = Path.GetExtension(path);
            if (extension == ".csv") data = new DataFromToCSV();
            else if (extension == ".xlsx") data = new DataFromToXLSX();
            return data;
        }
        private Communications getCommunications()
        {
            IDataReaderWriter data = getDataReaderWriter(tbLoadPath.Text);

            List<Station> stations = data.ReadFromFile(tbLoadPath.Text);
            stations.Insert(0, new Station(Convert.ToInt32(tbMainStationId.Text)));

            Communications communications = new Communications(stations);

            communications.h = Convert.ToDouble(tbH.Text);
            communications.hMin = Convert.ToDouble(tbHMin.Text);
            communications.accidentPercent = Convert.ToDouble(tbAccidentPercent.Text) / 100;
            communications.repairSectionMinimumLength = Convert.ToDouble(tbRepairSectionMinimumLength.Text);

            communications.checkData();

            return communications;
        }
        private void generateResult()
        {
            Communications communications = getCommunications();

            IDataReaderWriter data = getDataReaderWriter(tbSavePath.Text);

            for (int i = 1; i < communications.stations.Count; i++)
            {
                communications.calculateOptimalK(i);
                if (cbOnlyMainInfo.IsChecked == false) data.WriteInFile(tbSavePath.Text, communications, i, (i == 1) ? true : false);
            }
            data.WriteInFile(tbSavePath.Text, communications, (bool)cbOnlyMainInfo.IsChecked);            
        }
        private async void bCalculate_Click(object sender, RoutedEventArgs e)
        {          
            try
            {
                generateResult();
                await this.ShowMessageAsync(CurrentLocalization.localizationUserInterface.messages.finishCalculatingTitle, CurrentLocalization.localizationUserInterface.messages.finishCalculating);
            }
            catch(OperationCanceledException){}
            catch (Exception ex)
            {
                await this.ShowMessageAsync(CurrentLocalization.localizationErrors.error, ex.Message);
            }           
        }

        private void bBrowseLoadPath_Click(object sender, RoutedEventArgs e)
        {
            
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Microsort Exel 2007-2013 XML (*.xlsx)|*.xlsx|comma-separated values (*.csv)|*.csv";
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
            fileDialog.Filter = "Microsort Exel 2007-2013 XML (*.xlsx)|*.xlsx|comma-separated values (*.csv)|*.csv";          
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
            Properties.Settings.Default.lanuage = 0;
            bindingLocalization();
        }

        private void Menu_Language_Russian(object sender, RoutedEventArgs e)
        {
            checkOnlyOneMenu(sender);
            Properties.Settings.Default.lanuage = 1;
            bindingLocalization();
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
        //LayoutAlgorithmType="BoundedFR"
        //LayoutAlgorithmType="CompoundFDP" 
        //LayoutAlgorithmType="Tree" 
        public IBidirectionalGraph<object, IEdge<object>> _graphToVisualize;
        public IBidirectionalGraph<object, IEdge<object>> graphToVisualize
        {
            get
            {
                return this._graphToVisualize;
            }
            set
            {
                if (!Equals(value, this._graphToVisualize))
                {
                    this._graphToVisualize = value;
                    this.RaisePropChanged("graphToVisualize");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropChanged(string name)
        {
            var eh = this.PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }
        private async void createGraphToVisualize()
        {
            try
            {
                Communications communications = getCommunications();
                List<Station> stations = communications.stations;

                var g = new BidirectionalGraph<object, IEdge<object>>();
                //add vertices
                List<String> vertices = new List<String>();
                for (int i = 0; i < stations.Count; i++)
                {
                    vertices.Add(stations[i].id.ToString());
                    g.AddVertex(vertices[i]);
                }
                //add edges
                for (int i = 1; i < stations.Count; i++)
                {
                    g.AddEdge(new Edge<object>(stations[i].sourceId.ToString(), stations[i].id.ToString()));
                }
                graphToVisualize = g;
            }
            catch (OperationCanceledException){}
            catch (Exception ex)
            {
                await this.ShowMessageAsync(CurrentLocalization.localizationErrors.error, ex.Message);
            }          
        }

        private async void bRefresh_Click(object sender, RoutedEventArgs e)
        {
            createGraphToVisualize();

            LocalizationUserInterface ui = CurrentLocalization.localizationUserInterface;
            var result = await this.ShowMessageAsync("proceed?", "Info", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
            {
                AffirmativeButtonText = ui.messages.yes,
                NegativeButtonText = ui.messages.no,
            });
        }
    }
}
