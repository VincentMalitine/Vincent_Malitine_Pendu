# Jeu du Pendu — Vincent Malitine

_____________________________________________________________________________________________________________________________________________________

## But
Deviner le mot secret avant de perdre toutes les vies.

_____________________________________________________________________________________________________________________________________________________

## Prérequis
- Visual Studio 2022
- .NET 9 (WPF Desktop)
- Copier les dossiers `Images\` et `Sons\` dans le répertoire de sortie de l'application (par ex. `bin\Debug\net9.0-windows\`).
- Fichier de mots : `WordsMAJONLY.txt` (un mot par ligne, UTF‑8). MAJUSCULES recommandé mais non obligatoire.

_____________________________________________________________________________________________________________________________________________________

## Lancer l'application
1. Ouvrir la solution dans Visual Studio 2022.
2. Sélectionner le projet comme projet de démarrage.
3. Lancer avec __Déboguer > Démarrer le débogage__ (F5) ou __Déboguer > Démarrer sans débogage__ (Ctrl+F5).

_____________________________________________________________________________________________________________________________________________________

## Fichiers importants
- `MainWindow.xaml` — interface utilisateur (Windows, boutons lettres, barres de vie/temps).
- `MainWindow.xaml.cs` — logique du jeu (sélection de mot, gestion des tentatives, timer, sons, joker, difficulté).
- `App.xaml` — configuration de démarrage.
- `WordsMAJONLY.txt` — liste de mots (doit être à la racine de l'exécutable).
- Dossier `Images\` — images d'état de vie attendues : `0.png`, `1.png`, ..., `10.png`.
- Dossier `Sons\` — fichiers son : `Correct.wav`, `Wrong.wav`, `Victory.wav`, `GameOver.wav`.

_____________________________________________________________________________________________________________________________________________________

## Règles et mécanique
- Vies initiales : 10.
- Timer par tentative : 30 secondes. À 0 → perdu 1 vie, message et redémarrage du compte à 30s.
- Barre de vie (`LifeProgressBar`) utilise une échelle 0–100 ; la valeur est calculée comme `vie * 10`.
- Image de vie : `LifeImage` charge `Images\<vie>.png`.
- Saisie :
  - Cliquer sur une lettre (boutons sur la fenêtre) ou taper une lettre au clavier.
  - Les lettres sont normalisées en majuscule (`ToUpperInvariant`).
  - Valider la proposition avec la touche Entrée ou le bouton `✅`.
- Lettres déjà utilisées : désactivées et colorées en `#7A0000`.
- Joker :
  - 1 par partie.
  - Propose aléatoirement une lettre encore masquée (implémentation intelligente).
  - Utilisation du joker retire 1 vie.
  - Si aucune lettre pertinente n'est trouvée, le joker ne s'applique pas.
- Sélection du mot :
  - Chargement via `File.ReadAllLines(..., Encoding.UTF8)` et sélection sécurisée via `RandomNumberGenerator.GetInt32(...)`.
  - Difficulté (valeur interne `Difficulty`) :
    - `0` — Facile : mots courts (<= 6 caractères).
    - `1` — Moyen : mots 7 à 11 caractères.
    - `2` — Difficile : mots ≥ 12 caractères.
  - Changer la difficulté cycle entre Facile → Moyen → Difficile → Facile et redémarre la partie.
- Sons : la lecture des sons se fait via `SoundPlayer` pour les fichiers `Sons\Correct.wav`, `Wrong.wav`, `Victory.wav`, `GameOver.wav`.
- Messages utilisateur : usage intensif de `MessageBox.Show(...)` (modal).

_____________________________________________________________________________________________________________________________________________________

## Comportements UX
- Au démarrage une boîte de dialogue de bienvenue s'affiche.
- En cas d'erreur (lettre déjà utilisée, proposition vide), un message s'affiche et le son "wrong" est joué.
- Victoire / défaite : son puis message, puis redémarrage automatique.
- Le clavier contient aussi les lettres accentuées courantes (Â, À, É, È, Ê, Î, Ô, Ù, Û).

_____________________________________________________________________________________________________________________________________________________

## Points à vérifier avant distribution
- Présence des fichiers :
  - `WordsMAJONLY.txt` dans le dossier de l'exécutable.
  - Images `0.png` à `10.png` dans `Images\`.
  - Fichiers WAV dans `Sons\`.
  - Si des fichiers manquent, l'application lève des erreurs ou se comporte de façon inattendue.
- Encodage de `WordsMAJONLY.txt` : UTF‑8 recommandé pour les accents.
- S'assurer que le projet cible bien `net9.0-windows` (WPF .NET 9).

_____________________________________________________________________________________________________________________________________________________

## Notes techniques rapides
- Timer de tentative : `DispatcherTimer` avec Interval = 1s.
- Réinitialisation du timer : remise à 30s via `ResetAttemptTimer()` et (re)démarrage si nécessaire.
- La sélection aléatoire est cryptographiquement sécurisée grâce à `RandomNumberGenerator`.
- Les lettres proposées par le joker proviennent uniquement des positions encore masquées (`'#'`) et excluent `'-'`.
