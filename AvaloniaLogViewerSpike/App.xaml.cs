using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaLogViewerSpike.Models;
using AvaloniaLogViewerSpike.ViewModels;
using AvaloniaLogViewerSpike.Views;
using Dock.Model;

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
            var layout = factory.CreateLayout();
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
                        dock.Close();
                    }
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
