using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Security.Cryptography;
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
        int vie = 10; // Nombre d'essais restants
        string processlettresDevinees = ""; // Lettres déjà devinées durant le traitement
        string lettresDevinees = ""; // Lettres déjà devinées
        string lettresUtilisees = ""; // Lettres déjà devinées
        char TextBox_Result = ' ';
        char tentative = ' ';



        public MainWindow()
        {
            InitializeComponent();
            // Utiliser PreviewTextInput pour les caractères (meilleure compatibilité clavier)
            this.PreviewTextInput += Window_PreviewTextInput;
            // Utiliser PreviewKeyDown pour capter Enter avant qu'un contrôle le consomme
            this.PreviewKeyDown += Window_PreviewKeyDown;
            this.Focusable = true;
            this.Focus(); // donne le focus à la fenêtre pour recevoir les frappes
            MessageBox.Show("Bienvenue au jeu du Pendu ! Devinez le mot en proposant des lettres. Vous avez 10 vies. Bonne chance !");
            RestartButton_Click(this, new RoutedEventArgs());
            
        }

        private void Button_Letter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string content && content.Length == 1)
            {
                TextBox_Result = content[0];
                ResultTextBox.Text = "Proposition : " + TextBox_Result.ToString();
            }
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            tentative = TextBox_Result;
            TextBox_Result = ' ';
            if (lettresUtilisees.Contains(tentative))
            {
                // son d'erreur
                SoundPlayer wrong = new SoundPlayer(@"Sons\Wrong.wav");
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Vous avez déjà utilisé la lettre : " + tentative);
            }
            else if (tentative == ' ')
            {
                // son d'erreur
                SoundPlayer wrong = new SoundPlayer(@"Sons\Wrong.wav");
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Tentative ne peut être vide.");
            }
            else
            {
                if (mot.Contains(tentative))
                {
                    lettresUtilisees += tentative;
                    processlettresDevinees = lettresDevinees;
                    lettresDevinees = "";
                    // son de réussite
                    SoundPlayer correct = new SoundPlayer(@"Sons\Correct.wav");
                    correct.Play();

                    for (int i = 0; i < mot.Length; i++)
                    {
                        if (mot[i] == tentative)
                        {
                            lettresDevinees += tentative;
                        }
                        else if (processlettresDevinees.Length > i)
                        {
                            lettresDevinees += processlettresDevinees[i];
                        }
                        else
                        {
                            lettresDevinees += "#";
                        }
                    }
                    ResultTextBox.Text = "Proposition : ";
                    UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                    FoundedTextBox.Text = "Lettre(s) juste : " + lettresDevinees;
                    if (lettresDevinees == mot)
                    {
                        // son de victoire
                        SoundPlayer victory = new SoundPlayer(@"Sons\Victory.wav");
                        victory.Play();
                        MessageBox.Show("Félicitations ! Vous avez deviné le mot : " + mot);
                        RestartButton_Click(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    vie--;
                    LifeTextBox.Text = "Vies restantes : " + vie;
                    LifeProgressBar.Value = vie * 10;
                    LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
                    ResultTextBox.Text = "Proposition : ";
                    UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                    FoundedTextBox.Text = "Lettre(s) juste : " + lettresDevinees;
                    lettresUtilisees += tentative;
                    // son d'erreur
                    SoundPlayer wrong = new SoundPlayer(@"Sons\Wrong.wav");
                    wrong.Play();
                    if (vie <= 0)
                    {
                        // son de défaite
                        SoundPlayer gameover = new SoundPlayer(@"Sons\GameOver.wav");
                        gameover.Play();
                        MessageBox.Show("Game Over ! Le mot était : " + mot);
                        RestartButton_Click(this, new RoutedEventArgs());
                    }
                }
                ResultTextBox.Text = "Proposition : ";
                UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                FoundedTextBox.Text = "Lettre(s) juste : " + lettresDevinees;
            }

        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            var words = File.ReadAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WordsMAJONLY.txt"), Encoding.UTF8).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            mot = words.Length == 0 ? "prototype" : words[RandomNumberGenerator.GetInt32(words.Length)].Trim();
            lettresDevinees = "";
            for (int i = 0; i < mot.Length; i++)
            {
                lettresDevinees += "#";
            }
            vie = 10;
            LifeProgressBar.Value = 100;
            LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
            LifeTextBox.Text = "Vies restantes : " + vie;
            processlettresDevinees = "";
            UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
            FoundedTextBox.Text = "Lettre(s) juste : " + lettresDevinees;
            ResultTextBox.Text = "Proposition : ";
            lettresUtilisees = "";
        }

        // Récupère le texte entré (respecte la disposition du clavier)
        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) && char.IsLetter(e.Text[0]))
            {
                char c = char.ToUpperInvariant(e.Text[0]);
                TextBox_Result = c;
                ResultTextBox.Text = "Proposition : " + c;
                e.Handled = true; // empêche une éventuelle propagation
            }
        }

        // Intercepte Enter (ou autres touches non textuelles)
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Button_Done_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }
    }
}