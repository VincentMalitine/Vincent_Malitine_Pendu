# Jeu du Pendu — Mode d'emploi

## 1. Mode d'emploi (utilisation)
- Ouvrez la solution dans Visual Studio (compatible .NET 9).
- Objectif : deviner le mot secret en proposant des lettres.
- Contrôles :
  - Appuyez sur une touche alphabétique du clavier pour sélectionner immédiatement la lettre proposée.
  - Appuyez sur la touche `Entrée` pour valider la proposition sélectionnée.
  - Vous pouvez aussi cliquer sur les boutons de lettres puis sur le bouton de validation.
- Indicateurs à l'écran :
  - `ResultTextBox` : affiche la proposition courante.
  - `FoundedTextBox` : affiche les lettres correctement trouvées (masquées par `#`).
  - `UsedTextBox` : liste les lettres déjà utilisées.
  - `LifeTextBox`, `LifeProgressBar`, `LifeImage` : montrent les vies restantes.
- Sons : effets pour réussite, erreur, victoire et défaite (dossier `Sons\`).
- Si vous voulez modifier la liste des mots, éditez `WordsMAJONLY.txt` (un mot par ligne, en majuscules).

---

## 2. Spécificités de cette version
- Lecture des mots depuis `WordsMAJONLY.txt` (encodage UTF-8) ; le mot est choisi aléatoirement avec `RandomNumberGenerator`.
- Gestion clavier améliorée :
  - La fenêtre capte les frappes alphabétiques et affiche la proposition instantanément.
  - La touche `Entrée` valide la lettre sélectionnée.
  - Gestion via `PreviewTextInput` et `PreviewKeyDown` (meilleure compatibilité AZERTY/QWERTY et prise en charge des caractères textuels).
- Affichage des lettres trouvées sous forme masquée (`#`) jusqu'à leur découverte.
- Sons pour retours utilisateur :
  - `Sons\Correct.wav`, `Sons\Wrong.wav`, `Sons\Victory.wav`, `Sons\GameOver.wav`.
- Réinitialisation automatique du jeu après victoire ou défaite.
- Mise à jour dynamique de l'interface (`LifeProgressBar`, `LifeImage` suivant le nombre de vies).

---

## 3. Ce qui pourrait manquer / améliorations suggérées
- Validation et robustesse :
  - Vérifier l'existence et le format de `WordsMAJONLY.txt` ; message utilisateur clair si absent.
  - Gestion des exceptions lors du chargement des fichiers (Images / Sons).
- Internationalisation :
  - Support des accents et lettres non ASCII (actuellement conçu pour lettres A–Z MAJUSCULES).
  - Option pour mots en minuscules / casse mixte.
- Accessibilité et ergonomie :
  - Indicateur visuel pour la lettre actuellement sélectionnée dans l'interface.
  - Bouton `Annuler` ou possibilité d'effacer la sélection courante.
  - Option pour activer/désactiver les sons.
- Gameplay :
  - Option de niveau de difficulté (nombre de vies, catégories de mots).
  - Sauvegarde des scores / historique local.
  - Affichage d'indices (définition, catégorie).
- Tests et qualité :
  - Ajout de tests unitaires couvrant la logique du traitement des propositions.
  - Mesures de robustesse (fichiers manquants, mots vides).

---

## 4. Fichiers importants (bref)
- `MainWindow.xaml` / `MainWindow.xaml.cs` — logique UI et jeu.
- `WordsMAJONLY.txt` — liste de mots (à placer dans le répertoire de l'exécutable).
- Dossiers : `Images\` (images vies), `Sons\` (fichiers .wav).

---