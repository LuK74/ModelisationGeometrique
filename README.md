# Modelisation Geometrique
## TP1
> Le code se trouve � l'int�rieur du r�pertoire TP1-Assets, dans lequel se trouve 4 scripts C# correspondant respectivement � une "Shape"
  et donc � ajouter en tant que component � un objet sur Unity (incluant MeshRendered, etc..)
> Les param�tres de la "Shape" sont expos�s en tant que [SerializeField] permettant de configurer dans l'�diteur de unity

### Triangle
> Triangle n'est pas tr�s explicite mais il s'agit en r�alite d'un rectangle compos� de 'nbLignes' et 'nbColonnes' donc chaque sous rectangle est compos� de 2 triangles
[SerializeField]:
- nbLignes: Nombre de lignes (int)
- nbColonnes: Nombre de colonnes (int)

### Cylindre
> Utilis� pour repr�senter un cylindre
[SerializeField]:
- m_rayon: Rayon du cylindre (float) 
- m_height: Hauteur du cylindre (float)
- m_nmeridiens: Nombre de m�ridiens (int)
- m_truncatedAngle: Angle de la partie tronqu� (float) [fonctionne que si m_isTruncated == true]
- m_isTruncated: D�cides si on tronque ou non l'objet (bool)

### Sphere
> Utilis� pour repr�senter un Sphere
[SerializeField]:
- m_rayon: Rayon du sphere (float) 
- m_nmeridiens: Nombre de m�ridiens (int)
- m_nparallels: Nombre de parall�le (int)_
- m_truncatedAngle: Angle de la partie tronqu� (float) [fonctionne que si m_isTruncated == true]
- m_isTruncated: D�cides si on tronque ou non l'objet (bool)

### Cone
> Utilis� pour repr�senter un cone
[SerializeField]:
- m_rayon: Rayon du cone (float) 
- m_height: Hauteur du cone (float)
- m_truncatedHeight: Diff�rence entre le sommet math�matique du cone, et le point qui sera utilis� pour sa repr�sentation dans Unity pour repr�senter la face haute (float)_
- m_nmeridiens: Nombre de m�ridiens (int)
- m_truncatedAngle: Angle de la partie tronqu� (float) [fonctionne que si m_isTruncated == true]
- m_isTruncated: D�cides si on tronque ou non l'objet (bool)