using Avalonia.Threading;
using AvaloniaLogViewerSpike.Messages;
using AvaloniaLogViewerSpike.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        public ObservableCollection<LogEntryModel> LogEntries { get; }

        public LogViewModel()
        {
            LogEntries = new ObservableCollection<LogEntryModel>();

            MessageBus
                .Current
                .Listen<LogEntriesMessage>()
                .Subscribe(x =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (x.LogEntries != null)
                        {
                            foreach (var log in x.LogEntries)
                            {
                                if (log != null)
                                {
                                    LogEntries.Add(log);
                                }
                            }
                        }
                    });
                });
        }
    }
}
