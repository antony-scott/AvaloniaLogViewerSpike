using AvaloniaLogViewerSpike.Services;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _filename = @"D:\logfile.log.txt";
        public LogFileMonitorService LogFileMonitorService { get; } = new LogFileMonitorService();
        public LogViewModel LogViewModel { get; }

        //public ObservableCollection<LogEntryModel> LogEntries { get; }

        public MainWindowViewModel()
        {
            LogViewModel = new LogViewModel();
            //LogEntries = new ObservableCollection<LogEntryModel>();

            //MessageBus
            //    .Current
            //    .Listen<LogEntriesMessage>()
            //    .Subscribe(x =>
            //    {
            //        Dispatcher.UIThread.Post(() =>
            //        {
            //            foreach (var log in x.LogEntries)
            //            {
            //                LogEntries.Add(log);
            //            }
            //        });
            //    });

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
