using System.Windows;

namespace Pacman
{
    public partial class MainMenu : Window
    {
        public MainMenu() =>
            InitializeComponent();

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            new Game().Show();
            Close();
        }
    }
}
