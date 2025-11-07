# Jeu du Pendu

## Objectif
Deviner le mot secret avant de perdre toutes les vies.

## Commandes
- Clavier : tapez une lettre, Entrée pour valider.
- Souris : cliquez sur une lettre puis validez (✅ ou Entrée).
- Lettres déjà jouées : deviennent rouges et sont désactivées jusqu’à la prochaine partie.
- Joker (1 par partie) : révèle une lettre non trouvée et enlève 1 vie.

## Interface
- FoundedTextBox : mot partiellement découvert (# = lettre inconnue, - conservé).
- UsedTextBox : lettres déjà tentées.
- LifeTextBox / LifeProgressBar / LifeImage : suivi des vies.
- TimeBar : 30 s pour jouer; à 0 → -1 vie et redémarrage du compte.
- DifficultyTextBox : niveau actuel.

## Fonctionnalités
- Chargement des mots (UTF-8) dans WordsMAJONLY.txt.
- Sélection d’un mot aléatoire sécurisée (RandomNumberGenerator).
- Difficulté :
  - Easy : toute longueur.
  - Hard : ≥ 7 caractères.
  - Extreme : ≥ 10 caractères.
- Timer entre tentatives (DispatcherTimer).
- Sons (Correct, Wrong, Victory, GameOver).
- Lettres utilisées désactivées + couleur rouge.
- Joker intelligent (choisit une lettre encore masquée).
- Redémarrage automatique après victoire/défaite.

## Ressources
- WordsMAJONLY.txt : un mot par ligne (MAJUSCULES recommandé).
- Dossiers : Images\ (états des vies), Sons\ (*.wav).

## Améliorations possibles
Durée du timer configurable, affichage des secondes restantes, désactivation des sons, scores/records, indices (catégorie), vérification de fichiers manquants.

## Prérequis
Visual Studio 2022, .NET 9.