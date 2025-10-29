using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pendu_Vincent_Malitine
{
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
    {
        string mot = " "; // Le mot à deviner
        int vie = 6; // Nombre d'essais restants
        string lettresDevinees = ""; // Lettres déjà devinées
        bool jeuTermine = false; // Indique si le jeu est terminé
        bool IsWordGuessed = false;
        char TextBox_Result = ' ';
        char tentative = ' ';

        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Letter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string content && content.Length == 1)
            {
                TextBox_Result = content[0];
                ResultTextBox.Text = TextBox_Result.ToString();
            }
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            tentative = TextBox_Result;
            TextBox_Result = ' ';
            if (mot.Contains(tentative))
            {
                lettresDevinees += tentative;
                if (IsWordGuessed == true)
                {
                    MessageBox.Show("Félicitations ! Vous avez deviné le mot : " + mot);
                    jeuTermine = true;
                }
            }
            else
            {
                vie--;
                LifeTextBox.Text = "Vies restantes : " + vie;
                if (vie <= 0)
                {
                    MessageBox.Show("Game Over ! Le mot était : " + mot);
                    jeuTermine = true;
                }
            }
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            vie=6;
            lettresDevinees="";
            jeuTermine=false;
            LifeTextBox.Text = "Vies restantes : " + vie;

        }
    }
}