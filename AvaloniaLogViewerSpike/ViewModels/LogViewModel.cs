using AvaloniaLogViewerSpike.Messages;
using AvaloniaLogViewerSpike.Models;
using Dock.Model.Controls;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogViewModel : Document//, IActivatableViewModel
    {
        [JsonProperty]
        public string Name { get; set; } = null;

        IObservable<LogEntriesMessage> _logsObserver = MessageBus.Current.Listen<LogEntriesMessage>();

        public ObservableCollection<LogEntryModel> LogEntries { get; } = new ObservableCollection<LogEntryModel>();

        public LogViewModel()
        {
            //this
            //    .WhenAnyValue(x => x.Name)
            //    .Subscribe(name =>
            //    {
            //        _logsObserver = _logsObserver.Where(x => name == null || x.Name == name);
            //    });

            _logsObserver
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
                            if (Name == null || logEntriesMessage.Name == Name)
                            {
                                LogEntries.Add(logEntry);
                            }
                        });
                });
        }
    }
}
