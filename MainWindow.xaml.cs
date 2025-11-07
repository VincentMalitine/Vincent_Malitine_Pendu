using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading; // Ajout pour DispatcherTimer

namespace Pendu_Vincent_Malitine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Initialisation des sons
        SoundPlayer correct = new SoundPlayer(@"Sons\Correct.wav");
        SoundPlayer wrong = new SoundPlayer(@"Sons\Wrong.wav");
        SoundPlayer victory = new SoundPlayer(@"Sons\Victory.wav");
        SoundPlayer gameover = new SoundPlayer(@"Sons\GameOver.wav");

        // Variables de jeu
        string mot = " "; // Le mot à deviner
        int vie = 10; // Nombre d'essais restants
        string processlettresDevinees = ""; // Lettres déjà devinées durant le traitement
        string lettresDevinees = ""; // Lettres déjà devinées
        string TimerlettresUtilisees = ""; // Lettres devinées pour le timer
        string lettresUtilisees = ""; // Lettres déjà devinées
        char TextBox_Result = ' '; // Caractère entré dans la TextBox
        char tentative = ' '; // Caractère proposé
        int Difficulty = 0; // Niveau de difficulté
        int Joker = 1; // Nombre de jokers disponibles

        // Timer de tentative
        DispatcherTimer attemptTimer;
        int remainingSeconds = 30;

        public MainWindow()
        {
            InitializeComponent();
            // Timer d’intervalle entre tentatives
            attemptTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            attemptTimer.Tick += AttemptTimer_Tick;

            // Utiliser PreviewTextInput pour les caractères (meilleure compatibilité clavier)
            this.PreviewTextInput += Window_PreviewTextInput;
            // Utiliser PreviewKeyDown pour capter Enter avant qu'un contrôle le consomme
            this.PreviewKeyDown += Window_PreviewKeyDown;
            this.Focusable = true;
            this.Focus(); // donne le focus à la fenêtre pour recevoir les frappes
            MessageBox.Show("Bienvenue au jeu du Pendu ! Devinez le mot en proposant des lettres. Vous avez 10 vies. Bonne chance !");
            // Initialise une nouvelle partie
            RestartButton_Click(this, new RoutedEventArgs());
        }

        private void AttemptTimer_Tick(object? sender, EventArgs e)
        {
            remainingSeconds--;
            TimeBar.Value = remainingSeconds;
            if (remainingSeconds <= 0)
            {
                // Temps écoulé : -1 vie et reset
                DecrementLife("Temps écoulé (-1 vie)");
                if (vie > 0)
                {
                    ResetAttemptTimer();
                }
            }
        }

        private void ResetAttemptTimer()
        {
            remainingSeconds = 30;
            TimeBar.Maximum = 30;
            TimeBar.Value = remainingSeconds;
            if (!attemptTimer.IsEnabled)
                attemptTimer.Start();
        }

        private void StopAttemptTimer()
        {
            attemptTimer.Stop();
        }

        private void DecrementLife(string reason)
        {
            vie--;
            LifeTextBox.Text = "Vies restantes : " + vie;
            LifeProgressBar.Value = vie * 10;
            LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
            wrong.Play();
            ResultTextBox.Text = "Proposition : ";
            UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
            FoundedTextBox.Text = lettresDevinees;

            if (vie <= 0)
            {
                gameover.Play();
                MessageBox.Show(reason + "\nGame Over ! Le mot était : " + mot);
                StopAttemptTimer();
                RestartButton_Click(this, new RoutedEventArgs());
            }
            else if (reason.StartsWith("Temps"))
            {
                MessageBox.Show(reason);
            }
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

            // lettre déjà utilisée
            if (lettresUtilisees.Contains(tentative))
            {
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Vous avez déjà utilisé la lettre : " + tentative);
                ResetAttemptTimer();
                return;
            }
            // tentative vide
            else if (tentative == ' ')
            {
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Proposition ne peut être vide.");
                // Ne reset pas le timer pour éviter spam reset sans jouer
                return;
            }
            else
            {
                bool lettreConsommee = false;

                if (mot.Contains(tentative))
                {
                    lettresUtilisees += tentative;
                    lettreConsommee = true;
                    processlettresDevinees = lettresDevinees;
                    lettresDevinees = "";
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
                        // conserve les # pour les lettres non encore devinées
                        else
                        {
                            lettresDevinees += '#';
                        }
                        // ajoute les tirets si le mot en contient
                        if (mot[i] == '-')
                        {
                            lettresDevinees.Substring(i, 1);
                            lettresDevinees = lettresDevinees.Remove(i, 1).Insert(i, "-");
                        }
                    }
                    ResultTextBox.Text = "Proposition : ";
                    UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                    FoundedTextBox.Text = lettresDevinees;

                    if (lettresDevinees == mot)
                    {
                        victory.Play();
                        MessageBox.Show("Félicitations ! Vous avez deviné le mot : " + mot);
                        StopAttemptTimer();
                        RestartButton_Click(this, new RoutedEventArgs());
                        return;
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
                    FoundedTextBox.Text = lettresDevinees;
                    lettresUtilisees += tentative;
                    lettreConsommee = true;
                    wrong.Play();
                    if (vie <= 0)
                    {
                        gameover.Play();
                        MessageBox.Show("Game Over ! Le mot était : " + mot);
                        StopAttemptTimer();
                        RestartButton_Click(this, new RoutedEventArgs());
                        return;
                    }
                }

                // Désactivation + couleur rouge si la lettre vient d'être consommée
                if (lettreConsommee)
                {
                    char c = char.ToUpperInvariant(tentative);
                    foreach (var child in LettersPanel.Children)
                    {
                        if (child is Button b && b.Content is string s && s.Length == 1 && s[0] == c)
                        {
                            b.IsEnabled = false;
                            b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A0000"));
                            break;
                        }
                    }
                }

                ResultTextBox.Text = "Proposition : ";
                UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                FoundedTextBox.Text = lettresDevinees;
                // Reset du timer après une tentative traitée
                ResetAttemptTimer();
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
            var words = File.ReadAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WordsMAJONLY.txt"), Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            mot = words.Length == 0 ? "prototype" : words[RandomNumberGenerator.GetInt32(words.Length)].Trim();
            if (Difficulty == 1) // Hard
            {
                while (mot.Length < 7)
                {
                    mot = words[RandomNumberGenerator.GetInt32(words.Length)].Trim();
                }
            }
            if (Difficulty == 2) // Extreme
            {
                while (mot.Length < 10)
                {
                    mot = words[RandomNumberGenerator.GetInt32(words.Length)].Trim();
                }
            }
            else if (Difficulty == 0) // Easy
            {
                while (mot.Length > 6)
                {
                    mot = words[RandomNumberGenerator.GetInt32(words.Length)].Trim();
                }
            }
            lettresDevinees = "";
            for (int i = 0; i < mot.Length; i++)
            {
                lettresDevinees += "#";
                if (mot[i] == '-')
                {
                    lettresDevinees.Substring(i, 1);
                    lettresDevinees = lettresDevinees.Remove(i, 1).Insert(i, "-");
                }
            }
            vie = 10;
            LifeProgressBar.Value = 100;
            LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
            LifeTextBox.Text = "Vies restantes : " + vie;
            processlettresDevinees = "";
            UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
            FoundedTextBox.Text = lettresDevinees;
            ResultTextBox.Text = "Proposition : ";
            lettresUtilisees = "";
            Joker = 1;

            // Réactive et remet la couleur d'origine de toutes les lettres
            foreach (var child in LettersPanel.Children)
            {
                if (child is Button b && b.Content is string s && s.Length == 1 && char.IsLetter(s[0]))
                {
                    b.IsEnabled = true;
                    b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252526"));
                }
            }

            ResetAttemptTimer();
        }

        private void ChangeDifficultyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Difficulty == 0)
            {
                Difficulty = 1;
                DifficultyTextBox.Text = "Difficulté : Hard";
                MessageBox.Show("Difficulté changée en Hard. Bonne chance !");
                RestartButton_Click(this, new RoutedEventArgs());
            }
            else if (Difficulty == 1)
            {
                Difficulty = 2;
                DifficultyTextBox.Text = "Difficulté : Extreme";
                MessageBox.Show("Difficulté changée en Extreme. Sortez le dictionnaire !");
                RestartButton_Click(this, new RoutedEventArgs());
            }
            else
            {
                Difficulty = 0;
                DifficultyTextBox.Text = "Difficulté : Easy";
                MessageBox.Show("Difficulté changée en Easy. Amusez-vous bien !");
                RestartButton_Click(this, new RoutedEventArgs());
            }
        }

        // Propose une lettre aléatoire non encore révélée du mot, puis lance le traitement standard
        private bool TryProposeRandomLetterFromWord()
        {
            if (string.IsNullOrWhiteSpace(mot) || string.IsNullOrEmpty(lettresDevinees))
                return false;

            // Sélectionne les lettres des positions encore masquées (#) et non des tirets
            var candidates = mot
                .Select((ch, idx) => new { ch, idx })
                .Where(x => x.ch != '-' && lettresDevinees.Length > x.idx && lettresDevinees[x.idx] == '#')
                .Select(x => x.ch)
                .Distinct()
                .ToArray();

            if (candidates.Length == 0)
                return false;

            // Choix aléatoire sécurisé
            var chosen = candidates[RandomNumberGenerator.GetInt32(candidates.Length)];

            // Sécurité supplémentaire si jamais la lettre est déjà marquée utilisée (ne devrait pas arriver)
            if (lettresUtilisees.Contains(chosen))
            {
                var alt = candidates.FirstOrDefault(c => !lettresUtilisees.Contains(c));
                if (alt == default(char))
                    return false;
                chosen = alt;
            }

            // Place la lettre dans la "proposition" et réutilise le flux standard
            TextBox_Result = chosen;
            ResultTextBox.Text = "Proposition : " + chosen + " (Joker)";
            Button_Done_Click(this, new RoutedEventArgs());
            return true;
        }

        private void UseJokerButton_Click(object sender, RoutedEventArgs e)
        {
            if (Joker <= 0)
            {
                MessageBox.Show("Vous n'avez plus de jokers disponibles.");
                return;
            }

            // Tente de proposer une lettre aléatoire du mot (non révélée)
            if (!TryProposeRandomLetterFromWord())
            {
                MessageBox.Show("Aucune lettre à proposer. Toutes les lettres ont déjà été révélées ou ne sont pas valides.");
                return;
            }

            // Joker consommé uniquement si une lettre a bien été proposée
            Joker -= 1;
            vie--;
            LifeTextBox.Text = "Vies restantes : " + vie;
            LifeProgressBar.Value = vie * 10;
            LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
            ResultTextBox.Text = "Proposition : ";
            UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
            FoundedTextBox.Text = lettresDevinees;
            lettresUtilisees += tentative;
            if (vie <= 0)
            {
                gameover.Play();
                MessageBox.Show("Game Over ! Le mot était : " + mot);
                StopAttemptTimer();
                RestartButton_Click(this, new RoutedEventArgs());
                return;
            }
        }
    }
}