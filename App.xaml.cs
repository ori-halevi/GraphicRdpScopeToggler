using System.Windows;
using GraphicRdpScopeToggler.Services.RdpService;
using GraphicRdpScopeToggler.Views;
using Prism.Ioc;

using System.Windows.Forms;
using System.Drawing;
using GraphicRdpScopeToggler.Services.FilesService;

namespace GraphicRdpScopeToggler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private NotifyIcon notifyIcon;
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IRdpService, RdpService>();
            containerRegistry.RegisterSingleton<IFilesService, FilesService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("Resources/remote-desktop.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "השם של התוכנה שלך";

            var contextMenu = new ContextMenuStrip();

            MainWindow.Hide();
            contextMenu.Items.Add("פתח חלון", null, (s, ea) => ShowMainWindow());
            contextMenu.Items.Add("יציאה", null, (s, ea) =>
            {
                notifyIcon.Visible = false;
                Shutdown();
            });

            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.MouseClick += (s, ea) =>
            {
                if (ea.Button == MouseButtons.Left)
                    ShowMainWindow();
            };
        }

        private void ShowMainWindow()
        {
            if (MainWindow == null)
                return;

            MainWindow.Show();
            MainWindow.Activate();
        }
    }
}
