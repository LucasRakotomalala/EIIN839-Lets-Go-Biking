# [Let's Go Biking !](https://lms.univ-cotedazur.fr/course/view.php?id=4334&amp;section=11)

## Auteur

- [Lucas Rakotomalala](https://github.com/LucasRakotomalala)

## Architecture

### Diagramme

<img src="resources/Diagramme d'architecture.png" alt="Diagramme d'architecture" style="margin: auto;"/>

### Implémentation

* [`HeavyClient`](HeavyClient)
    * Client console en `.Net Framework 4.7.2` pour démontrer certains fonctionnalités du `Routing`
    * Communication `SOAP` avec `Routing`
    * Exécutable de sortie utilisé pour créer l'[*image Docker*](#avec-une-version-dockerisée-de-la-solution-net)
    * **Fonctionnalités :**
        * Permet d'avoir la trajet entre un point *A* et un point *B* avec la prise de vélos lorsqu'utile
        * Permet d'afficher les statistiques d'utilisation des stations des *x* derniers jours
        * Permet d'exporter dans un fichier Excel (`stats.xls`) les statistiques d'utilisation des stations des *x* derniers jours

* [`LightClient`](LightClient)
    * Client web en `HTML5/CSS3/JS`
        * pour démontrer certains fonctionnalités du `Routing`
        * avec une interface *responsive*
            * et mise en place d'une [`PWA`](https://web.dev/progressive-web-apps/) pour les utilisateurs mobiles
    * Communication `REST` avec `Routing`
    * **Fonctionnalités :**
        * Permet d'avoir la trajet entre un point *A* et un point *B* avec la prise de vélos lorsqu'utile
        * Permet d'obtenir le trajet entre la position de l'utilisateur et une station
        * Permet d'afficher des informations relatives à une station

* [`Proxy`](Proxy)
    * Bibliothèque de services `WCF`
    * Exposition `SOAP` et `REST` des méthodes de son interface [`IJCDecaux`](Proxy/Services/IJCDecaux.cs)
    * Présence d'un [cache générique](Proxy/Caches/Cache.cs)
        * utilisé pour stocker les informations, récupérées avec l'***API JCDecaux***, d'une station sous la forme de [`JCDecauxItem`](Proxy/Models/JCDecauxItem.cs)

* [`Routing`](Routing)
    * Bibliothèque de services `WCF`
    * Communication en `REST` 
        * avec `Proxy` pour récupérer des données récentes d'une station spécifique
        * avec l'***API JCDecaux*** en `REST` pour récupérer, à l'initialisation, la liste de toutes les stations
    * Exposition `SOAP` et `REST` des méthodes de son interface [`IRouting`](Routing/Services/IRouting.cs)

* [`Host`](Host)
    * Exposition d'une version *console* des projets `Routing` et `Proxy`
    * Affichage des traces d'exécutions de `Routing` et de `Proxy`
    * Exécutable de sortie utilisé pour créer l'[*image Docker*](#avec-une-version-dockerisée-de-la-solution-net)

## Récupération des sources

* Depuis l'invité de commandes :
```bash
$ git clone https://github.com/LucasRakotomalala/EIIN839-Lets-Go-Biking.git "Let's Go Biking"
$ cd "Let's Go Biking"
```

**OU**

* En téléchargeant les sources puis en extrayant l'archive

## Exécution du projet

### Exécution de la solution .Net

#### Avec le script fourni

* Pré-requis :
    * [BuildTools for Visual Studio 2019](https://www.visualstudio.com/downloads/) (`>= 16.0`)

* Exécution du script batch [`build.bat`](build.bat) (depuis un invité de commande) :

    ```bash
    $ build
    ```

*Note: Dans le cas où le script ne fonctionne pas, un dossier [`build.backup`](build.backup) est disponible pour exécuter l'étape suivante.*

* Exécuter `Host.exe` (**en tant qu'Administrateur**) et `HeavyClient.exe` présents dans le dossier `build` *(ou [`build.backup`](build.backup))*

#### Avec une version Dockerisée de la solution .Net

* Pré-requis :

    * [Docker](https://www.docker.com)
    * [Docker Compose](https://www.docker.com)

* Lancement du serveur et du client :

    ```bash
    $ build # Les images n'étant pas encore sur le Hub Docker à cause de la taille (+ de 6Go)
    $ docker-compose up -d
    ```

* Accéder à la CLI du `Client` :

    ```bash
    $ docker attach Client # ^P ^Q pour se détacher
    ```

* Afficher les logs du `Host` :

    ```bash
    $ docker logs Host
    ```

#### Depuis Visual Studio 2019 (en mode *Administrateur*)

* Définition des projets à lancer :
<img src="resources/Projets de lancement.png" alt="Projets de lancement" style="margin: auto;"/>

* Lancement des projets :
<img src="resources/Lancement des projets.png" alt="Lancement des projets" style="margin: auto;"/>

### Exécution du `LightClient`

#### Avec la `PWA`

* Pré-requis :

    * [NodeJS](https://nodejs.org/en/)
    * [http-server](https://www.npmjs.com/package/http-server)

```bash
$ cd LightClient
$ http-server -p 80 # ^C pour arrêter le serveur HTTP local
```

* URL pour accéder au site internet : [`http://localhost/`](http://localhost/)

#### Sans la `PWA`

* Se rendre dans le dossier `LightClient` depuis votre *Explorateur Windows*
* Ouvrir `index.html` avec votre navigateur favori

## Liens utiles

* [Leaflet](https://leafletjs.com/)
* [Dockeriser un hôte de service WCF](https://devblogs.microsoft.com/aspnet/lets-try-wcf-self-hosted-services-in-a-container/)
* [Docker Compose - Client WCF et WCF Self-Hosted](https://github.com/microsoft/dotnet-framework-docker/tree/main/samples/wcfapp)
