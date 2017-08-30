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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            this.DataContext = Languages.current;
            InitializeComponent();            
            getLanguages();          
        }
        #region localization
        //to localization
        private void getLanguages()
        {
            List<String> languages = Languages.list();
            foreach(String l in languages)
            {
                System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem { Header = l };
                item.Click += new RoutedEventHandler(changeLanguage);
                mLanguage.Items.Add(item);
            }                     
        }
        private void changeLanguage(object sender, RoutedEventArgs e)
        {
            checkOnlyOneMenu(sender);
            Properties.Settings.Default.language = (String)((System.Windows.Controls.MenuItem)sender).Header;
            this.DataContext = Languages.current;
        }
        #endregion
        #region interfaceVisualEffectsLogic
        private bool showDialogResult(String title, String message)
        {
            return ((new Message(this, title, message, true)).ShowDialog() == true ? true : false);
        }
        private void showMessageBox(String title, String message)
        {
            (new Message(this, title, message)).ShowDialog();
        }
        private void checkOnlyOneMenu(object sender)
        {
            foreach (System.Windows.Controls.MenuItem m in ((System.Windows.Controls.MenuItem)((System.Windows.Controls.MenuItem)sender).Parent).Items)
            {
                m.IsChecked = false;
            }
            ((System.Windows.Controls.MenuItem)sender).IsChecked = true;
        }
        #endregion
        #region calculating
        private IDataReaderWriter getDataReaderWriter(String path)
        {
            IDataReaderWriter data = null;
            String extension = Path.GetExtension(path);
            if (extension == ".csv") data = new DataFromToCSV();
            else if (extension == ".xlsx") data = new DataFromToXLSX();
            return data;
        }
        internal class InputSettings
        {
            public double h;
            public double hMin;
            public double accidentPercent;
            public double repairSectionMinimumLength;
            public String pathLoad;
            public String pathSave;
            public bool onlyMainInfo;
            public InputSettings(double h, double hMin, double accidentPercent, double repairSectionMinimumLength, String pathLoad, String pathSave, bool onlyMainInfo)
            {
                this.h = h;
                this.hMin = hMin;
                this.accidentPercent = accidentPercent;
                this.repairSectionMinimumLength = repairSectionMinimumLength;
                this.pathLoad = pathLoad;
                this.pathSave = pathSave;
                this.onlyMainInfo = onlyMainInfo;
            }
        }
        private Communications getCommunications(InputSettings inputSettings)
        {
            IDataReaderWriter data = getDataReaderWriter(inputSettings.pathLoad);

            List<Station> stations = data.ReadFromFile(inputSettings.pathLoad);
            stations.Insert(0, new Station(Convert.ToInt32(tbMainStationId.Text)));

            Communications communications = new Communications(stations);

            communications.h = inputSettings.h;
            communications.hMin = inputSettings.hMin;
            communications.accidentPercent = inputSettings.accidentPercent / 100;
            communications.repairSectionMinimumLength = inputSettings.repairSectionMinimumLength;

            communications.NonCriticalError += Communications_NonCriticalError;
            communications.checkData();           

            return communications;
        }
        private Task generateResult(Communications communications, String pathSave, bool onlyMainInfo)
        {
            return Task.Run(() =>
            {
                IDataReaderWriter data = getDataReaderWriter(pathSave);
                for (int i = 1; i < communications.stations.Count; i++)
                {
                    communications.calculateOptimalK(i);
                    if (onlyMainInfo == false) data.WriteInFile(pathSave, communications, i, (i == 1) ? true : false);
                }
                data.WriteInFile(pathSave, communications, onlyMainInfo);
            });           
        }
        private void Communications_NonCriticalError(object sender, Communications.NonCriticalErrorEventArgs e)
        {           
            bool result = showDialogResult(Languages.current.error, e.Message);
            if(result == false) throw new OperationCanceledException();
        }
        
        private async void calculating(InputSettings inputSettings)
        {          
                try
                {
                    Communications communications = getCommunications(inputSettings);
                    await generateResult(communications, inputSettings.pathSave, inputSettings.onlyMainInfo);
                    showMessageBox(Languages.current.messageTitleFinishCalculating, Languages.current.messageFinishCalculating);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    showMessageBox(Languages.current.error, ex.Message);
                }
                finally
                {
                    gMain.IsEnabled = true;
                    prCalculatingProcess.IsActive = false;
                }
            
        }
        private InputSettings getInputSettings()
        {
            return new InputSettings(Convert.ToDouble(tbH.Text), Convert.ToDouble(tbHMin.Text), Convert.ToDouble(tbAccidentPercent.Text), Convert.ToDouble(tbRepairSectionMinimumLength.Text),
                tbLoadPath.Text, tbSavePath.Text, (bool)cbOnlyMainInfo.IsChecked);
        }
        #endregion
        #region buttons
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
        private void tbLoadPath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (File.Exists(@tbLoadPath.Text)) System.Diagnostics.Process.Start(@tbLoadPath.Text);
        }
        private void tbSavePath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (File.Exists(@tbLoadPath.Text)) System.Diagnostics.Process.Start(tbSavePath.Text);
        }
        private void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            prCalculatingProcess.IsActive = true;
            gMain.IsEnabled = false;
            calculating(getInputSettings());        
        }
        private void bRefresh_Click(object sender, RoutedEventArgs e)
        {
            createGraphToVisualize();
        }
        #endregion
        #region menu
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
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }
        private void Menu_AboutProgram(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((Environment.CurrentDirectory + @"\Help\index.html"));
        }  
        #endregion
        #region graph 
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
        private void createGraphToVisualize()
        {
            try
            {
                Communications communications = getCommunications(getInputSettings());
                
                List<Station> stations = communications.stations;

                var g = new BidirectionalGraph<object, IEdge<object>>();
                List<String> vertices = new List<String>();
                for (int i = 0; i < stations.Count; i++)
                {
                    vertices.Add(stations[i].id.ToString());
                    g.AddVertex(vertices[i]);
                }
                for (int i = 1; i < stations.Count; i++)
                {
                    g.AddEdge(new Edge<object>(stations[i].sourceId.ToString(), stations[i].id.ToString()));
                }
                graphToVisualize = g;
            }
            catch (OperationCanceledException){}
            catch (Exception ex)
            {
                showMessageBox(Languages.current.error, ex.Message);
            }          
        }       
        #endregion
    }
}
