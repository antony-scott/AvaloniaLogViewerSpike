﻿using Avalonia.Media;
using AvaloniaLogViewerSpike.Messages;
using Dock.Model.Controls;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogViewModel : Document, IActivatableViewModel
    {
        [JsonProperty]
        public string Name { get; set; } = null;

        [JsonIgnore]
        public int FontSize { get; set; } = 20;

        [JsonIgnore]
        public FontFamily FontFamily { get; set; } = new FontFamily("Courgette");

        IDisposable _messageSubscriber = null;
        IDisposable _logEntrySubscriber = null;

        public ObservableCollection<LogEntryViewModel> LogEntries { get; } = new ObservableCollection<LogEntryViewModel>();

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
                            if (logEntriesMessage?.LogEntries?.Any() ?? false)
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
                            }
                        });
                });
        }
    }
}
