using AvaloniaLogViewerSpike.Services;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LogFileMonitorService LogFileMonitorService { get; } = new LogFileMonitorService();
        public LogViewModel LogViewModel { get; }

        public MainWindowViewModel()
        {
            LogFileMonitorService.AddMonitor("Log1", @"D:\log1\log1.txt");
            LogFileMonitorService.AddMonitor("Log2", @"D:\log2\log2.txt");

            LogViewModel = new LogViewModel(
                "Log1"
                );
        }
    }
}
