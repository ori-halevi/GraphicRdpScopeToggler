using System.Windows;

namespace GraphicRdpScopeToggler.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // ביטול הסגירה
            e.Cancel = true;

            // הסתרת החלון במקום סגירה
            this.Hide();
        }
    }

}
