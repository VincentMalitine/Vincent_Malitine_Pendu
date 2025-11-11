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

        // Variables de jeu (champs - conservées)
        string mot = " ";
        int vie = 10;
        string processlettresDevinees = "";
        string lettresDevinees = "";
        string TimerlettresUtilisees = "";
        string lettresUtilisees = "";
        char TextBox_Result = ' ';
        char tentative = ' ';
        int Difficulty = 0;
        int Joker = 1;

        // Timer de tentative
        DispatcherTimer attemptTimer;
        int remainingSeconds = 30;

        public MainWindow()
        {
            InitializeComponent();
            attemptTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            attemptTimer.Tick += AttemptTimer_Tick;

            this.PreviewTextInput += Window_PreviewTextInput;
            this.PreviewKeyDown += Window_PreviewKeyDown;
            this.Focusable = true;
            this.Focus();
            MessageBox.Show("Bienvenue au jeu du Pendu ! Devinez le mot en proposant des lettres. Vous avez 10 vies. Bonne chance !");
            RestartButton_Click(this, new RoutedEventArgs());
        }

        private void AttemptTimer_Tick(object? sender, EventArgs e)
        {
            remainingSeconds--;
            TimeBar.Value = remainingSeconds;
            if (remainingSeconds <= 0)
            {
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

        // Renommage : content -> texteBoutonLettre
        private void Button_Letter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string texteBoutonLettre && texteBoutonLettre.Length == 1)
            {
                TextBox_Result = texteBoutonLettre[0];
                ResultTextBox.Text = "Proposition : " + TextBox_Result.ToString();
            }
        }

        // Renommage : c -> caractereSaisi
        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) && char.IsLetter(e.Text[0]))
            {
                char caractereSaisi = char.ToUpperInvariant(e.Text[0]);
                TextBox_Result = caractereSaisi;
                ResultTextBox.Text = "Proposition : " + caractereSaisi;
                e.Handled = true;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Button_Done_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        // Renommages locaux :
        // lettreConsommee -> lettreConsommeeDansTentative
        // child -> uiChild, b -> boutonLettre, s -> texteBouton, c -> lettreEntree
        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            tentative = TextBox_Result;
            TextBox_Result = ' ';

            if (lettresUtilisees.Contains(tentative))
            {
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Vous avez déjà utilisé la lettre : " + tentative);
                return;
            }
            else if (tentative == ' ')
            {
                wrong.Play();
                ResultTextBox.Text = "Proposition : ";
                MessageBox.Show("Proposition ne peut être vide.");
                return;
            }
            else
            {
                bool lettreConsommeeDansTentative = false;

                if (mot.Contains(tentative))
                {
                    lettresUtilisees += tentative;
                    lettreConsommeeDansTentative = true;
                    processlettresDevinees = lettresDevinees;
                    lettresDevinees = "";
                    correct.Play();

                    for (int indexLettre = 0; indexLettre < mot.Length; indexLettre++)
                    {
                        if (mot[indexLettre] == tentative)
                        {
                            lettresDevinees += tentative;
                        }
                        else if (processlettresDevinees.Length > indexLettre)
                        {
                            lettresDevinees += processlettresDevinees[indexLettre];
                        }
                        else
                        {
                            lettresDevinees += '#';
                        }
                        if (mot[indexLettre] == '-')
                        {
                            lettresDevinees.Substring(indexLettre, 1);
                            lettresDevinees = lettresDevinees.Remove(indexLettre, 1).Insert(indexLettre, "-");
                        }
                    }
                    ResultTextBox.Text = "Proposition : ";
                    UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                    FoundedTextBox.Text = lettresDevinees;

                    if (lettresDevinees == mot)
                    {
                        victory.Play();
                        StopAttemptTimer();
                        MessageBox.Show("Félicitations ! Vous avez deviné le mot : " + mot);
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
                    lettreConsommeeDansTentative = true;
                    wrong.Play();
                    if (vie <= 0)
                    {
                        gameover.Play();
                        StopAttemptTimer();
                        MessageBox.Show("Game Over ! Le mot était : " + mot);
                        RestartButton_Click(this, new RoutedEventArgs());
                        return;
                    }
                }

                if (lettreConsommeeDansTentative)
                {
                    char lettreEntree = char.ToUpperInvariant(tentative);
                    foreach (var uiChild in LettersPanel.Children)
                    {
                        if (uiChild is Button boutonLettre && boutonLettre.Content is string texteBouton && texteBouton.Length == 1 && texteBouton[0] == lettreEntree)
                        {
                            boutonLettre.IsEnabled = false;
                            boutonLettre.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A0000"));
                            break;
                        }
                    }
                }

                ResultTextBox.Text = "Proposition : ";
                UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
                FoundedTextBox.Text = lettresDevinees;
                ResetAttemptTimer();
            }
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Renommages : words -> tousLesMots, i -> indexLettre
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Difficulty == 0)
            {
                DifficultyTextBox.Text = "Difficulté : Easy";
            }
            else if (Difficulty == 1)
            {
                DifficultyTextBox.Text = "Difficulté : Hard";
            }
            else
            {
                DifficultyTextBox.Text = "Difficulté : Extreme";
            }
            var tousLesMots = File.ReadAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WordsMAJONLY.txt"), Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            mot = tousLesMots.Length == 0 ? "prototype" : tousLesMots[RandomNumberGenerator.GetInt32(tousLesMots.Length)].Trim();
            if (Difficulty == 1)
            {
                while (mot.Length < 7 || mot.Length > 11)
                {
                    mot = tousLesMots[RandomNumberGenerator.GetInt32(tousLesMots.Length)].Trim();
                }
            }
            if (Difficulty == 2)
            {
                while (mot.Length < 12)
                {
                    mot = tousLesMots[RandomNumberGenerator.GetInt32(tousLesMots.Length)].Trim();
                }
            }
            else if (Difficulty == 0)
            {
                while (mot.Length > 6)
                {
                    mot = tousLesMots[RandomNumberGenerator.GetInt32(tousLesMots.Length)].Trim();
                }
            }
            lettresDevinees = "";
            for (int indexLettre = 0; indexLettre < mot.Length; indexLettre++)
            {
                lettresDevinees += "#";
                if (mot[indexLettre] == '-')
                {
                    lettresDevinees.Substring(indexLettre, 1);
                    lettresDevinees = lettresDevinees.Remove(indexLettre, 1).Insert(indexLettre, "-");
                }
            }
            vie = 10;
            LifeProgressBar.Value = 100;
            LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
            LifeTextBox.Text = "Vies restantes : " + vie;
            processlettresDevinees = "";
            FoundedTextBox.Text = lettresDevinees;
            ResultTextBox.Text = "Proposition : ";
            Joker = 1;

            foreach (var uiChild in LettersPanel.Children)
            {
                if (uiChild is Button boutonLettre && boutonLettre.Content is string texteBouton && texteBouton.Length == 1 && char.IsLetter(texteBouton[0]))
                {
                    boutonLettre.IsEnabled = true;
                    boutonLettre.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252526"));
                }
            }
            lettresUtilisees = "";
            tentative = ' ';
            TextBox_Result = ' ';
            UsedTextBox.Text = "Lettre(s) précédement utilisée(s) : " + lettresUtilisees;
            ResetAttemptTimer();
        }

        private void ChangeDifficultyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Difficulty == 0)
            {
                Difficulty = 1;
                DifficultyTextBox.Text = "Difficulté : Hard";
                StopAttemptTimer();
                MessageBox.Show("Difficulté changée en Hard. Bonne chance !");
                RestartButton_Click(this, new RoutedEventArgs());
            }
            else if (Difficulty == 1)
            {
                Difficulty = 2;
                DifficultyTextBox.Text = "Difficulté : Extreme";
                StopAttemptTimer();
                MessageBox.Show("Difficulté changée en Extreme. Sortez le dictionnaire !");
                RestartButton_Click(this, new RoutedEventArgs());
            }
            else
            {
                Difficulty = 0;
                DifficultyTextBox.Text = "Difficulté : Easy";
                StopAttemptTimer();
                MessageBox.Show("Difficulté changée en Easy. Amusez-vous bien !");
                RestartButton_Click(this, new RoutedEventArgs());
            }
        }

        // Renommages : candidateLetters -> lettresCandidates, selectedLetter -> lettreSelectionnee, alternativeLetter -> lettreAlternative
        private bool TryProposeRandomLetterFromWord()
        {
            if (string.IsNullOrWhiteSpace(mot) || string.IsNullOrEmpty(lettresDevinees))
                return false;

            var lettresCandidates = mot
                .Select((letter, index) => new { letter, index })
                .Where(p => p.letter != '-' && lettresDevinees.Length > p.index && lettresDevinees[p.index] == '#')
                .Select(p => p.letter)
                .Distinct()
                .ToArray();

            if (lettresCandidates.Length == 0)
                return false;

            var lettreSelectionnee = lettresCandidates[RandomNumberGenerator.GetInt32(lettresCandidates.Length)];

            if (lettresUtilisees.Contains(lettreSelectionnee))
            {
                var lettreAlternative = lettresCandidates.FirstOrDefault(letterCandidate => !lettresUtilisees.Contains(letterCandidate));
                if (lettreAlternative == default)
                    return false;
                lettreSelectionnee = lettreAlternative;
            }

            TextBox_Result = lettreSelectionnee;
            ResultTextBox.Text = "Proposition : " + lettreSelectionnee + " (Joker)";
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

            if (!TryProposeRandomLetterFromWord())
            {
                MessageBox.Show("Aucune lettre à proposer. Toutes les lettres ont déjà été révélées ou ne sont pas valides.");
                return;
            }

            Joker -= 1;
            vie--;
            LifeTextBox.Text = "Vies restantes : " + vie;
            LifeProgressBar.Value = vie * 10;
            LifeImage.Source = new ImageSourceConverter().ConvertFromString($@"Images\{vie}.png") as ImageSource;
            ResultTextBox.Text = "Proposition : ";
            UsedTextBox.Text = "Lettre(s) précédement utilisé(s) : " + lettresUtilisees;
            FoundedTextBox.Text = lettresDevinees;
            lettresUtilisees += tentative;
            tentative = ' ';
            TextBox_Result = ' ';
            if (vie <= 0)
            {
                gameover.Play();
                StopAttemptTimer();
                MessageBox.Show("Game Over ! Le mot était : " + mot);
                RestartButton_Click(this, new RoutedEventArgs());
                return;
            }
        }
    }
}