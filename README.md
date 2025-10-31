# Modelisation Geometrique
## TP1
> Le code se trouve à l'intérieur du répertoire TP1-Assets, dans lequel se trouve 4 scripts C# correspondant respectivement à une "Shape"
  et donc à ajouter en tant que component à un objet sur Unity (incluant MeshRendered, etc..)

> Les paramètres de la "Shape" sont exposés en tant que [SerializeField] permettant de configurer dans l'éditeur de unity

> Sur le GameObject j'utilisais le material suivant: "SpatialMappingWireframe"

### Triangle
> Triangle n'est pas très explicite mais il s'agit en réalite d'un rectangle composé de 'nbLignes' et 'nbColonnes' donc chaque sous rectangle est composé de 2 triangles

[SerializeField]:
- nbLignes: Nombre de lignes (int)
- nbColonnes: Nombre de colonnes (int)

### Cylindre
> Utilisé pour représenter un cylindre

[SerializeField]:
- m_rayon: Rayon du cylindre (float) 
- m_height: Hauteur du cylindre (float)
- m_nmeridiens: Nombre de méridiens (int)
- m_truncatedAngle: Angle de la partie tronqué (float) [fonctionne que si m_isTruncated == true]
- m_isTruncated: Décides si on tronque ou non l'objet (bool)

### Sphere
> Utilisé pour représenter un Sphere

[SerializeField]:
- m_rayon: Rayon du sphere (float) 
- m_nmeridiens: Nombre de méridiens (int)
- m_nparallels: Nombre de parallèle (int)_
- m_truncatedAngle: Angle de la partie tronqué (float) [fonctionne que si m_isTruncated == true]
- m_isTruncated: Décides si on tronque ou non l'objet (bool)

### Cone
> Utilisé pour représenter un cone

[SerializeField]:
- m_rayon: Rayon du cone (float) 
- m_height: Hauteur du cone (float)
- m_truncatedHeight: Différence entre le sommet mathématique du cone, et le point qui sera utilisé pour sa représentation dans Unity pour représenter la face haute (float)_
- m_nmeridiens: Nombre de méridiens (int)
- m_truncatedAngle: Angle de la partie tronqué (float) [fonctionne que si m_isTruncated == true]
- m_isTruncated: Décides si on tronque ou non l'objet (bool)
