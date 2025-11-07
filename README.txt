# Jeu du Pendu

## Utilisation
- Ouvrez la solution (Visual Studio 2022, .NET 9).
- But : deviner le mot en moins de vies possibles.
- Entrée valide la lettre sélectionnée (clavier ou clic).
- Lettres utilisées : deviennent rouges et sont bloquées jusqu’à nouvelle partie.
- Joker : révèle une lettre et retire 1 vie (1 seul par partie).

## Interface
- FoundedTextBox : mot partiellement découvert (# pour lettres inconnues).
- UsedTextBox : lettres déjà tentées.
- LifeTextBox / LifeProgressBar / LifeImage : vies restantes.
- TimeBar : 30 s pour jouer une lettre, sinon -1 vie puis reset.

## Difficultés
- Easy : mots quelconques.
- Hard : longueur ≥ 7.
- Extreme : longueur ≥ 10.
(Changer la difficulté relance une partie.)

## Ressources
- WordsMAJONLY.txt (UTF-8, un mot par ligne).
- Dossiers : Images\ (vies), Sons\ (Correct / Wrong / Victory / GameOver).

## Améliorations possibles
Timer configurable, affichage secondes, désactivation sons, scores, indices, validation de fichiers manquants.