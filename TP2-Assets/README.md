# Modelisation Geometrique
## TP2
> Le code se trouve à l'intérieur du répertoire TP2-Assets, dans lequel se trouve 1 scripts C# correspondant à l'attendu du TP
  sous le nom "MyCustomMesh.cs"

> Il suffit d'attacher ce script à un GameObject possédant un MeshFilter et un MeshRenderer, et d'insérer les 
  paramètres attendus dans les SerializeField, puis de lancer l'exécution car les fonctions sont éxécutés au 
  evenement Awake et Start

> Deux paramètres serialized sont utilisables 'input_filepath' (path vers le fichier .off à charger) + 'output_filepath' (path vers le
  fichier .off dans lequel on export la shape)

> A chaque exécution on charge le 'input_filepath', puis les transformations du TP sont appliqués (gravity center, normalized, normals)
  et ensuite on export le fichier dans 'output_filepath'

> Il semblerait que le calcul de normal comporte des erreurs, visibles en chargeant buddha.off (trou lumineux).
  Je n'ai pas su identifier d'où cela venait à temps, j'émets la supposition qu'il pourrait charger d'erreurs
  de calcul de flottant, mais je n'en suis pas sûr

> Le code est supposément suffisamenent commenté pour qu'aucune explication supplémentaire soit nécessaire

