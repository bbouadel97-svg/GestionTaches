# Cahier Des Charges

## Titre Du Projet

GestionTaches

## Contexte

Le projet s'inscrit dans le cadre du module Techno Boost. Il consiste a realiser une application concrete permettant d'approfondir une technologie choisie et de presenter un resultat exploitable.

Le projet retenu est une application desktop de gestion de taches developpee avec .NET et WPF.

## Objectif General

Concevoir une application de bureau permettant de centraliser et suivre des taches a effectuer dans un contexte simple de gestion d'activite.

## Objectifs Techniques

- prendre en main .NET et C#
- developper une interface graphique avec WPF
- structurer une application en couches simples : modele, service, acces aux donnees, interface
- manipuler une base de donnees locale avec Entity Framework Core et SQLite
- produire un projet presentable et executable

## Public Vise

- etudiant ou utilisateur souhaitant suivre des taches localement
- demonstration pedagogique d'une application de gestion

## Fonctionnalites Attendues

### Fonctionnalites principales

- ajouter une tache
- modifier une tache
- supprimer une tache
- consulter la liste des taches
- filtrer les taches par texte, statut, priorite et assignee
- visualiser les taches dans un tableau
- visualiser les taches dans une vue Kanban
- afficher des indicateurs de suivi

### Donnees gerees

Chaque tache doit pouvoir contenir :

- un identifiant
- un titre
- une description
- un assignee
- une priorite
- un statut
- une date d'echeance
- une date de creation

## Contraintes Techniques

- application desktop Windows
- utilisation de WPF pour l'interface
- utilisation de SQLite pour une base locale simple
- persistance des donnees entre les executions
- code maintenable et organise

## Ambition Du Projet

Le projet depasse un simple CRUD minimal, car il integre :

- une interface graphique complete
- une persistance locale avec ORM
- des filtres de recherche
- une double visualisation tableau / Kanban
- des indicateurs de suivi
- une initialisation automatique des donnees

Cela permet d'obtenir un rendu suffisamment riche pour une demonstration technique et pedagogique.

## Livrables Attendus

- le code source complet du projet
- une application executable depuis le projet .NET
- un README de presentation
- un document de choix technologique
- le present cahier des charges

## Criteres De Reussite

Le projet sera considere comme reussi si :

- l'application se lance correctement
- les operations principales sur les taches fonctionnent
- les donnees sont conservees localement
- l'interface est exploitable et claire
- la technologie choisie est correctement mise en oeuvre

## Evolutions Possibles

- ajout d'authentification
- ajout de commentaires sur les taches
- export CSV ou PDF
- notifications d'echeance
- architecture MVVM complete
- statistiques et graphiques avances