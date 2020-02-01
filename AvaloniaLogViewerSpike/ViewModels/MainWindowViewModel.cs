using AvaloniaLogViewerSpike.Services;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LogFileMonitorService LogFileMonitorService { get; } = new LogFileMonitorService();
        public LogViewModel LogViewModel { get; }

        public MainWindowViewModel()
        {
            LogFileMonitorService.AddMonitor("Log1", @"D:\logfile.log.txt");
            LogViewModel = new LogViewModel("Log1");
        }
    }
}
