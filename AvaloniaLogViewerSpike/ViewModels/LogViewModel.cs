using AvaloniaLogViewerSpike.Messages;
using AvaloniaLogViewerSpike.Models;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogViewModel : Document
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
                .Subscribe(logEntriesMessage =>
                {
                    var observableToo = logEntriesMessage
                        .LogEntries
                        .ToObservable()
                        .ObserveOn(RxApp.MainThreadScheduler);

                    //observableToo = observableToo
                    //    .Where(x => x.Severity == "Debug");

                    observableToo
                        .Subscribe(logEntry =>
                        {
                            LogEntries.Add(logEntry);
                        });
                });
        }
    }
}
