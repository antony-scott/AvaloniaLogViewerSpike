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
    public class LogViewModel : Document, IActivatableViewModel
    {
        [JsonProperty]
        public string Name { get; set; } = null;

        IDisposable _messageSubscriber = null;
        IDisposable _logEntrySubscriber = null;

        public ObservableCollection<LogEntryModel> LogEntries { get; } = new ObservableCollection<LogEntryModel>();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public LogViewModel()
        {
            this
                .WhenActivated(disposables =>
                {
                    this.HandleActivation(disposables);
                });
        }

        private void HandleActivation(Action<IDisposable> disposables)
        {
            this
                .WhenAnyValue(x => x.Name)
                //.WhenAnyValue(x => x.Filters)
                .Subscribe(name =>
                {
                    var observer = MessageBus
                        .Current
                        .Listen<LogEntriesMessage>();

                    if (name != null)
                    {
                        observer = observer
                            .Where(x => x.Name == name);
                    }

                    _messageSubscriber?.Dispose();
                    _messageSubscriber = observer
                        .Subscribe(logEntriesMessage =>
                        {
                            var logEntriesObserver = logEntriesMessage
                                .LogEntries
                                .ToObservable()
                                .ObserveOn(RxApp.MainThreadScheduler);

                            // TODO: this will change based on filters set in the UI (ie - debug only / message contains "x" / etc)
                            //logEntriesObserver = logEntriesObserver
                            //    .Where(x => x.Severity == "Debug");

                            _logEntrySubscriber?.Dispose();
                            _logEntrySubscriber = logEntriesObserver.Subscribe(LogEntries.Add);
                        });
                });
        }
    }
}
