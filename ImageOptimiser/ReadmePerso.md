# Optimisation des images

## Résultats des performances

- **Version séquentielle**: 0.10 secondes
- **Version parallèle**: 0.01 secondes
- **Gain de performance**: 14.01x plus rapide

## Observations

- La version parallèle utilise Parallel.ForEachAsync pour traiter les images en parallèle
- Les performances dépendent du nombre de coeurs CPU disponibles
- La version parallèle est généralement plus rapide pour un grand nombre d'images
- Les gains peuvent varier selon le matériel et la taille des images

## Ma Configuration sur PC Portable

- Processeur : I7 2.80 Ghz
- RAM : 16 Go
- Nombre de cœurs : 4 (8 avec hyperthreading)
- Système d'exploitation : Windows 11
