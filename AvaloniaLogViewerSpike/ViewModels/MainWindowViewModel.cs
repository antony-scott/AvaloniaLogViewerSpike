using AvaloniaLogViewerSpike.Services;
using System;
using System.IO;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _filename = @"D:\logfile.log.txt";
        public LogFileMonitorService LogFileMonitorService { get; } = new LogFileMonitorService();
        public LogViewModel LogViewModel { get; }

        public MainWindowViewModel()
        {
            LogViewModel = new LogViewModel();

            if (File.Exists(_filename))
            {
                if (LogFileMonitorService.IsFileBeingMonitored(_filename))
                {
                    MessageBox
                        .Avalonia
                        .MessageBoxManager
                        .GetMessageBoxStandardWindow(
                            "Error",
                            $"File is already being monitored - {_filename}",
                            icon: MessageBox.Avalonia.Enums.Icon.Error)
                        .Show();
                }
                else
                {
                    LogFileMonitorService.AddFile(Guid.NewGuid(), _filename);
                }
            }
            else
            {
                MessageBox
                    .Avalonia
                    .MessageBoxManager
                    .GetMessageBoxStandardWindow(
                        "Error",
                        $"Cannot find file - {_filename}",
                        icon: MessageBox.Avalonia.Enums.Icon.Error)
                    .Show();
            }
        }
    }
}
