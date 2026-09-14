using System.Windows;
using ITTicketSystem.ViewModels;

namespace ITTicketSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TicketListViewModel();
        }
    }
}