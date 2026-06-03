# TestMauiFaisabilite-KronoGeo

############################ Programmation MAUI pour Android ##############################################

Pour la géolocation,  utilisation des APi d'android directement, ne pas prendre le toolkit de la communauté - le point GPS n'est pas précis - marche bien pour les véhicules - occureny de 100m pour le best et entre 0 et 10 m pour IOs.
Programmer directement avec les API d'Android voir dans le répertoire Platforms/Android :
- AndroidLocationService.cs
- LocationListener.cs

Utilisation de la prise des photos :
- utilisation de CommunityToolkit.Maui.Camera pour la visualisation de la prise de vue (nuget)
  attention avec les dépendances des autres packages (une horreur - je me suis bien amusé merci Github copilot)
- utilisation de l'API Android pour la prise de vue, beaucoup plus simple
  
Programmation, installation de toolkit.CameraView et voir le répertoire Platforms/Android:
- TakePhoto.cs
- Au niveau de la prise de vue pour les autres options par exemple zoom ou autre prendre CommunityToolkit.Maui.Camera pour le faire.
- 
##########################################################################################################

########################### Api mise en place des roles au niveau d'identity ###########################

-  utiliser Identity
-  créer la base
-  mettre en place JWT bearer
-  le configurer et rajouter les rôles dans le programme de création du token
-  ajouter l'Authorize ave role au niveau des action ou controler  

########################################################################################################

