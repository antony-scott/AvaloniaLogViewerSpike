using AvaloniaLogViewerSpike.Services;
using Dock.Model;
using ReactiveUI;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LogFileMonitorService LogFileMonitorService { get; } = new LogFileMonitorService();

        public MainWindowViewModel()
        {
            LogFileMonitorService.AddMonitor("Log1", @"D:\log1\log1.txt");
            LogFileMonitorService.AddMonitor("Log2", @"D:\log2\log2.txt");
        }

        private IFactory _factory;
        private IDock _layout;
        private string _currentView;

        public IFactory Factory
        {
            get => _factory;
            set => this.RaiseAndSetIfChanged(ref _factory, value);
        }

        public IDock Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }

        public string CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }
    }
}
