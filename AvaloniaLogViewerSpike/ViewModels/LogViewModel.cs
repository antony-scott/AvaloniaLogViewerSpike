using Avalonia.Threading;
using AvaloniaLogViewerSpike.Messages;
using AvaloniaLogViewerSpike.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        //private int _maxNumberOfLogEntriesToKeep = 8000;
        //public List<LogEntryModel> History { get; set; } = new List<LogEntryModel>();
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
                                    //History.Add(log);
                                    LogEntries.Add(log);
                                    //if (LogEntries.Count > _maxNumberOfLogEntriesToKeep)
                                    //{
                                    //    LogEntries.RemoveAt(0);
                                    //}
                                }
                            }
                        }
                    });
                });
        }
    }
}
