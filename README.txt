# Jeu du Pendu � Mode d'emploi

## 1. Mode d'emploi (utilisation)
- Ouvrez la solution dans Visual Studio (compatible .NET 9).
- Objectif : deviner le mot secret en proposant des lettres.
- Contr�les :
  - Appuyez sur une touche alphab�tique du clavier pour s�lectionner imm�diatement la lettre propos�e.
  - Appuyez sur la touche `Entr�e` pour valider la proposition s�lectionn�e.
  - Vous pouvez aussi cliquer sur les boutons de lettres puis sur le bouton de validation.
- Indicateurs � l'�cran :
  - `ResultTextBox` : affiche la proposition courante.
  - `FoundedTextBox` : affiche les lettres correctement trouv�es (masqu�es par `#`).
  - `UsedTextBox` : liste les lettres d�j� utilis�es.
  - `LifeTextBox`, `LifeProgressBar`, `LifeImage` : montrent les vies restantes.
- Sons : effets pour r�ussite, erreur, victoire et d�faite (dossier `Sons\`).
- Si vous voulez modifier la liste des mots, �ditez `WordsMAJONLY.txt` (un mot par ligne, en majuscules).

---

## 2. Sp�cificit�s de cette version
- Lecture des mots depuis `WordsMAJONLY.txt` (encodage UTF-8) ; le mot est choisi al�atoirement avec `RandomNumberGenerator`.
- Gestion clavier am�lior�e :
  - La fen�tre capte les frappes alphab�tiques et affiche la proposition instantan�ment.
  - La touche `Entr�e` valide la lettre s�lectionn�e.
  - Gestion via `PreviewTextInput` et `PreviewKeyDown` (meilleure compatibilit� AZERTY/QWERTY et prise en charge des caract�res textuels).
- Affichage des lettres trouv�es sous forme masqu�e (`#`) jusqu'� leur d�couverte.
- Sons pour retours utilisateur :
  - `Sons\Correct.wav`, `Sons\Wrong.wav`, `Sons\Victory.wav`, `Sons\GameOver.wav`.
- R�initialisation automatique du jeu apr�s victoire ou d�faite.
- Mise � jour dynamique de l'interface (`LifeProgressBar`, `LifeImage` suivant le nombre de vies).

---

## 3. Ce qui pourrait manquer / am�liorations sugg�r�es
- Validation et robustesse :
  - V�rifier l'existence et le format de `WordsMAJONLY.txt` ; message utilisateur clair si absent.
  - Gestion des exceptions lors du chargement des fichiers (Images / Sons).
- Internationalisation :
  - Support des accents et lettres non ASCII (actuellement con�u pour lettres A�Z MAJUSCULES).
  - Option pour mots en minuscules / casse mixte.
- Accessibilit� et ergonomie :
  - Indicateur visuel pour la lettre actuellement s�lectionn�e dans l'interface.
  - Bouton `Annuler` ou possibilit� d'effacer la s�lection courante.
  - Option pour activer/d�sactiver les sons.
- Gameplay :
  - Option de niveau de difficult� (nombre de vies, cat�gories de mots).
  - Sauvegarde des scores / historique local.
  - Affichage d'indices (d�finition, cat�gorie).
- Tests et qualit� :
  - Ajout de tests unitaires couvrant la logique du traitement des propositions.
  - Mesures de robustesse (fichiers manquants, mots vides).

---

## 4. Fichiers importants (bref)
- `MainWindow.xaml` / `MainWindow.xaml.cs` � logique UI et jeu.
- `WordsMAJONLY.txt` � liste de mots (� placer dans le r�pertoire de l'ex�cutable).
- Dossiers : `Images\` (images vies), `Sons\` (fichiers .wav).

---