using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaLogViewerSpike.Models;
using AvaloniaLogViewerSpike.ViewModels;
using AvaloniaLogViewerSpike.Views;
using Dock.Model;
using Dock.Model.Controls;
using Dock.Serializer;
using System.IO;

namespace AvaloniaLogViewerSpike
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var factory = new DockFactory(new LogViewerSpikeData());

            IDock layout = null;

            var filename = @"D:\layout.json";
            if (File.Exists(filename))
            {
                try
                {
                    layout = new DockSerializer(typeof(AvaloniaList<>))
                        .Load<RootDock>(filename);
                }
                catch { }
            }
            layout = layout ?? factory.CreateLayout();

            factory.InitLayout(layout);

            var mainWindowViewModel = new MainWindowViewModel
            {
                Factory = factory,
                Layout = layout
            };

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var mainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };

                mainWindow.Closing += (sender, e) =>
                {
                    if (layout is IDock dock)
                    {
                        dock.Close();
                    }
                };

                desktopLifetime.MainWindow = mainWindow;

                desktopLifetime.Exit += (sender, e) =>
                {
                    if (layout is IDock dock)
                    {
                        new DockSerializer(typeof(AvaloniaList<>)).Save(filename, layout);
                        dock.Close();
                    }
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
