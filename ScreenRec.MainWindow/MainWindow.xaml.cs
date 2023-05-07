using ScreenRec.Settings;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScreenRec.ScreeanArea;
using System.ComponentModel;
using System.CodeDom;
using System.Threading;

namespace ScreenRec.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ScreenRec.Recoder.MainRecorder recorder;

        public MainWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private async void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            await ScreenRec.Settings.Sett.Load();
            recorder = new();
            EXSettings.DataContext = ScreenRec.Settings.Sett.Setting;
            RunRecordsStatus();
        }

        private async Task RunRecordsStatus(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var list = recorder?.Recorders.Where(x => x != null).Select(x => $"{x}").ToArray();

                ListRecorders.Items.Clear();
                foreach (var record in list)
                {
                    ListRecorders.Items.Add(record);
                }
                await Task.Delay(1000);
            }
        }



        private async void SettScreenArea(object sender, RoutedEventArgs e)
        {
            if (sender == null || !(sender is Button)) return;
            Button b = sender as Button;
            MonitorNum monitor = b.DataContext as MonitorNum;
            if (monitor == null) return;
            Form1 form = new();
            form.MonitorNum = monitor;
            form.Location = monitor.Location;
            form.AreaFormClosing += ScreeanAreaFormClosing;
            form.Show();
        }
        void ScreeanAreaFormClosing(MonitorNum monitorNum)
        {
            Dispatcher.Invoke(() =>
            {
                ListMonitors.ItemsSource = null;
                ListMonitors.ItemsSource = Settings.Sett.Setting.Monitors;
            });
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Sett.Save();
            recorder?.Dispose();
        }

        private void StartRecorder(object sender, RoutedEventArgs e)
        {
            if (recorder == null) recorder = new();
            recorder.Start();
        }

        private void StopRecorder(object sender, RoutedEventArgs e)
        {
            recorder?.Stop();
        }
    }
}