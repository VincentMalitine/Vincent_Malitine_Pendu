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
        // Variables de jeu
        string mot = " "; // Le mot à deviner
        int vie = 10; // Nombre d'essais restants
        string processlettresDevinees = ""; // Lettres déjà devinées durant le traitement
        string lettresDevinees = ""; // Lettres déjà devinées
        string lettresUtilisees = ""; // Lettres déjà devinées
        char TextBox_Result = ' '; // Caractère entré dans la TextBox
        char tentative = ' '; // Caractère proposé



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
            // Initialise une nouvelle partie au démarrage en utilisant le bouton de redémarrage afin d'éviter la duplication de code
            RestartButton_Click(this, new RoutedEventArgs());
            
        }

        // Gère le clic sur les boutons de lettres pour capturer la lettre proposée
        private void Button_Letter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string content && content.Length == 1)
            {
                TextBox_Result = content[0];
                ResultTextBox.Text = "Proposition : " + TextBox_Result.ToString();
            }
        }

        // Gère la saisie de texte sur le clavier physique pour capturer les lettres entrées
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

        // Gère la touche Enter pour valider la proposition au même titre qu'un clic sur le bouton "Done"
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Button_Done_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        // Gère le clic sur le bouton "Done" pour valider la proposition (plus ou moins le "coeur" du jeu)
        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            tentative = TextBox_Result;
            TextBox_Result = ' ';
            // vérifie si la lettre a déjà été utilisée
            if (lettresUtilisees.Contains(tentative))
            {
                // son d'erreur
                SoundPlayer wrong = new SoundPlayer(@"Sons\Wrong.wav");
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Vous avez déjà utilisé la lettre : " + tentative);
            }
            // vérifie si la tentative est vide
            else if (tentative == ' ')
            {
                // son d'erreur
                SoundPlayer wrong = new SoundPlayer(@"Sons\Wrong.wav");
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Proposition ne peut être vide.");
            }
            // traite la tentative
            else
            {
                // vérifie si la lettre proposée est dans le mot
                if (mot.Contains(tentative))
                {
                    lettresUtilisees += tentative;
                    processlettresDevinees = lettresDevinees;
                    lettresDevinees = "";
                    // son de réussite
                    SoundPlayer correct = new SoundPlayer(@"Sons\Correct.wav");
                    correct.Play();

                    // met à jour les lettres devinées
                    for (int i = 0; i < mot.Length; i++)
                    {
                        // remplace les # par la lettre proposée si elle est correcte
                        if (mot[i] == tentative)
                        {
                            lettresDevinees += tentative;
                        }
                        // conserve les lettres déjà devinées
                        else if (processlettresDevinees.Length > i)
                        {
                            lettresDevinees += processlettresDevinees[i];
                        }
                        // conserve les # pour les lettres non encore devinées
                        else
                        {
                            lettresDevinees += "#";
                        }
                    }
                    ResultTextBox.Text = "Proposition : ";
                    UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                    FoundedTextBox.Text = "Lettre(s) juste : " + lettresDevinees;
                    // vérifie si le mot est entièrement deviné
                    if (lettresDevinees == mot)
                    {
                        // son de victoire
                        SoundPlayer victory = new SoundPlayer(@"Sons\Victory.wav");
                        victory.Play();
                        MessageBox.Show("Félicitations ! Vous avez deviné le mot : " + mot);
                        RestartButton_Click(this, new RoutedEventArgs());
                    }
                }
                // lettre incorrecte
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
                    // vérifie si le joueur a épuisé toutes ses vies
                    if (vie <= 0)
                    {
                        // son de défaite
                        SoundPlayer gameover = new SoundPlayer(@"Sons\GameOver.wav");
                        gameover.Play();
                        MessageBox.Show("Game Over ! Le mot était : " + mot);
                        RestartButton_Click(this, new RoutedEventArgs());
                    }
                }
                // réinitialise la proposition et met à jour l'affichage "au cas où cela n'as ultérieurement pas été effectué"
                ResultTextBox.Text = "Proposition : ";
                UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                FoundedTextBox.Text = "Lettre(s) juste : " + lettresDevinees;
            }

        }

        // Gère le clic sur le bouton "End" pour fermer l'application
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Gère le clic sur le bouton "Restart" pour initialiser une nouvelle partie
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // charge un mot aléatoire depuis le fichier WordsMAJONLY.txt
            var words = File.ReadAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WordsMAJONLY.txt"), Encoding.UTF8).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            mot = words.Length == 0 ? "prototype" : words[RandomNumberGenerator.GetInt32(words.Length)].Trim();
            // initialise les variables de jeu
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
    }
}