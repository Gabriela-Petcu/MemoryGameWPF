using System.Windows;
using Memory.ViewModels;

namespace Memory.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("MainWindow loaded");
            DataContext = new MainViewModel();
        }
    }
}
