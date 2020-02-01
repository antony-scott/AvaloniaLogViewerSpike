using Avalonia.Threading;
using AvaloniaLogViewerSpike.Messages;
using AvaloniaLogViewerSpike.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        private readonly string _name;

        public ObservableCollection<LogEntryModel> LogEntries { get; }

        public LogViewModel(string name = null)
        {
            _name = name;
            LogEntries = new ObservableCollection<LogEntryModel>();

            var observable = MessageBus
                .Current
                .Listen<LogEntriesMessage>();

            if (_name != null)
            {
                observable = observable.Where(x => x.Name == _name);
            }

            observable
                .Subscribe(x =>
                {
                    //x.LogEntries.ToObservable();
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
