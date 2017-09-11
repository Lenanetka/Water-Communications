using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WaterCommunications.DataReaderWriter;
using WaterCommunications.Localization;

namespace WaterCommunications
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private List<Pipe> pipes { get; set; }
        public MainWindow()
        {
            this.DataContext = Languages.current;
            pipes = new DataFromToCSV().loadPipeMaterials(Environment.CurrentDirectory + @"\db_materials.csv");
            InitializeComponent();
            cbPipeMaterial.ItemsSource = pipes;
        }              
        private bool showDialogResult(String title, String message)
        {
            return ((new Message(this, title, message, true)).ShowDialog() == true ? true : false);
        }
        private void showMessageBox(String title, String message)
        {
            (new Message(this, title, message)).ShowDialog();
        }
        private InputSettings getInputSettings()
        {
            SaveLoadInfo saveLoadInfo = new SaveLoadInfo(tbLoadPath.Text, tbSavePath.Text, (bool)cbOnlyMainInfo.IsChecked);
            Parameters parameters = new Parameters(Convert.ToInt32(tbMainStationId.Text), Convert.ToDouble(tbH.Text), Convert.ToDouble(tbHMin.Text),
                                                   Convert.ToDouble(tbAccidentPercent.Text), Convert.ToDouble(tbRepairSectionMinimumLength.Text), Convert.ToDouble(tbAdditionalHeadLoss.Text));
            Pipe pipe = (Pipe)cbPipeMaterial.SelectedItem;
            return new InputSettings(saveLoadInfo, parameters, pipe);
        }        
        private IDataReaderWriter getDataReaderWriter(String path)
        {
            IDataReaderWriter data = null;
            String extension = Path.GetExtension(path);
            if (extension == ".csv") data = new DataFromToCSV();
            else if (extension == ".xlsx") data = new DataFromToXLSX();
            return data;
        }
        private Communications getCommunications(InputSettings inputSettings)
        {
            IDataReaderWriter data = getDataReaderWriter(inputSettings.saveLoadInfo.pathLoad);
            List<Station> stations = data.ReadFromFile(inputSettings.saveLoadInfo.pathLoad);
            Communications communications = new Communications(stations, inputSettings.parameters, inputSettings.pipe);

            communications.NonCriticalError += Communications_NonCriticalError;
            communications.checkData();

            return communications;
        }
        private Task generateResult(Communications communications, SaveLoadInfo saveLoadInfo)
        {
            return Task.Run(() =>
            {               
                IDataReaderWriter data = getDataReaderWriter(saveLoadInfo.pathSave);
                for (int i = 1; i < communications.stations.Count; i++)
                {
                    communications.calculateOptimalK(i);
                    if (saveLoadInfo.onlyMainInfo == false) data.WriteInFile(saveLoadInfo.pathSave, communications, i, (i == 1) ? true : false);
                }
                data.WriteInFile(saveLoadInfo.pathSave, communications, saveLoadInfo.onlyMainInfo);
            });
        }
        void Communications_NonCriticalError(object sender, NonCriticalErrorEventArgs e)
        {
            bool result = showDialogResult(Languages.current.error, e.Message);
            if (result == false) throw new OperationCanceledException();
        }
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
        private async void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            prCalculatingProcess.IsActive = true;
            gMain.IsEnabled = false;
            try
            {
                InputSettings inputSettings = getInputSettings();
                Communications communications = getCommunications(inputSettings);
                await generateResult(communications, inputSettings.saveLoadInfo);
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
        private void bRefresh_Click(object sender, RoutedEventArgs e)
        {
            createGraphToVisualize();
        }
        #endregion
        #region menu
        private void checkOnlyOneMenu(object sender)
        {
            foreach (System.Windows.Controls.MenuItem m in ((System.Windows.Controls.MenuItem)((System.Windows.Controls.MenuItem)sender).Parent).Items)
            {
                m.IsChecked = false;
            }
            ((System.Windows.Controls.MenuItem)sender).IsChecked = true;
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
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }
        private void Menu_AboutProgram(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((Environment.CurrentDirectory + @"\Help\index.html"));
        }
        private void pipeMaterialsLocalize(List<String> names)
        {
            for (int i = 0; i < pipes.Count; i++)
            {
                if (pipes[i].id >= 0 && pipes[i].id < names.Count)
                {
                    pipes[i].name = names[pipes[i].id];
                }
            }
            int selected = cbPipeMaterial.SelectedIndex;
            cbPipeMaterial.SelectedIndex = -1;
            cbPipeMaterial.Items.Refresh();
            cbPipeMaterial.SelectedIndex = selected;
        }
        private void changeLanguage(object sender, RoutedEventArgs e)
        {           
            Properties.Settings.Default.language = (String)((System.Windows.Controls.MenuItem)sender).Header;
            checkOnlyOneMenu(sender);
            Languages lang = Languages.current;
            this.DataContext = lang;
            pipeMaterialsLocalize(lang.cbPipeMaterial);
        }
        private void mLanguage_Loaded(object sender, RoutedEventArgs e)
        {
            mLanguage.Items.Clear();
            List<String> languages = Languages.list();
            foreach (String l in languages)
            {
                System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem { Header = l };
                item.Click += new RoutedEventHandler(changeLanguage);
                mLanguage.Items.Add(item);
            }
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
                for (int i = 0; i < stations.Count; i++)
                {
                    g.AddVertex(stations[i].id.ToString());
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
