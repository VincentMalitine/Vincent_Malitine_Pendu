using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
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
        string processlettresDevinees = ""; // Lettres déjà devinées durant le traitement
        string lettresDevinees = ""; // Lettres déjà devinées
        string lettresUtilisees = ""; // Lettres déjà devinées
        char TextBox_Result = ' ';
        char tentative = ' ';



        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("Bienvenue au jeu du Pendu ! Devinez le mot en proposant des lettres. Vous avez 6 vies. Bonne chance !");
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
                SoundPlayer wrong = new SoundPlayer(@".\Sons\Wrong.wav");
                wrong.Play();
                MessageBox.Show("Vous avez déjà utilisé la lettre : " + tentative);
            }
            else
            {
                if (mot.Contains(tentative))
                {
                    lettresUtilisees += tentative;
                    processlettresDevinees = lettresDevinees;
                    lettresDevinees = "";
                    // son de réussite
                    SoundPlayer correct = new SoundPlayer(@".\Sons\Correct.wav");
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
                            lettresDevinees += "_";
                        }
                    }
                    if (lettresDevinees == mot)
                    {
                        // son de victoire
                        SoundPlayer victory = new SoundPlayer(@".\Sons\Victory.wav");
                        victory.Play();
                        MessageBox.Show("Félicitations ! Vous avez deviné le mot : " + mot);
                    }
                }
                else
                {
                    vie--;
                    LifeTextBox.Text = "Vies restantes : " + vie;
                    LifeProgressBar.Value = vie * 10;
                    if (vie <= 0)
                    {
                        // son de défaite
                        SoundPlayer gameover = new SoundPlayer(@".\Sons\GameOver.wav");
                        gameover.Play();
                        MessageBox.Show("Game Over ! Le mot était : " + mot);
                    }
                    lettresUtilisees += tentative;

                    // son d'erreur
                    SoundPlayer wrong = new SoundPlayer(@".\Sons\Wrong.wav");
                    wrong.Play();
                }
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
            vie = 10;
            LifeProgressBar.Value = 100;
            lettresDevinees = "";
            LifeTextBox.Text = "Vies restantes : " + vie;
            lettresUtilisees = "";
            processlettresDevinees = "";
            UsedTextBox.Text = lettresUtilisees;
            FoundedTextBox.Text = lettresDevinees;
            ResultTextBox.Text = "";

            // Recherche Words.txt en remontant depuis le dossier de l'exécutable
            // (permet de trouver le fichier placé à la racine du projet source)
            try
            {
                string? wordsPath = FindFileInParentDirectories("Words.txt");
                if (string.IsNullOrEmpty(wordsPath))
                {
                    MessageBox.Show("Fichier Words.txt introuvable (recherche depuis le dossier de l'exécutable).");
                    mot = string.Empty;
                    return;
                }

                var lines = File.ReadAllLines(wordsPath)
                                .Select(l => l.Trim())
                                .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                                .ToArray();

                if (lines.Length == 0)
                {
                    MessageBox.Show("Aucun mot valide trouvé dans Words.txt.");
                    mot = string.Empty;
                    return;
                }

                mot = lines[new System.Random().Next(lines.Length)].ToUpperInvariant();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du mot : " + ex.Message);
                mot = string.Empty;
            }
        }

        // Remonte les répertoires à partir du dossier de l'exécutable (puis CurrentDirectory)
        // et retourne le chemin complet du fichier s'il est trouvé.
        private string? FindFileInParentDirectories(string fileName)
        {
            DirectoryInfo? dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                string candidate = System.IO.Path.Combine(dir.FullName, fileName);
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }

            // fallback : essayer depuis le répertoire courant
            dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                string candidate = System.IO.Path.Combine(dir.FullName, fileName);
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }

            return null;
        }
    }
}