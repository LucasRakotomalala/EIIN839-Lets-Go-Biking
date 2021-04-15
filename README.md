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

* [`LightClient`](LightClient)
    * Client web
        * pour démontrer certains fonctionnalités du `Routing`
        * avec une interface *responsive*
            * et la présence d'une [`PWA`](https://web.dev/progressive-web-apps/) pour les utilisateurs mobiles
    * Communication `REST` avec `Routing`

* [`Proxy`](Proxy)
    * Bibliothèque de services `WCF`
    * Exposition `SOAP` et `REST` des méthodes de son interface [`IJCDecaux`](Proxy/Services/IJCDecaux.cs)
    * Présence d'un [cache générique](Proxy/Cache/Cache.cs)
        * utilisé pour stocker les informations, récupérées avec l'***API de JCDecaux***, d'une station

* [`Routing`](Routing)
    * Bibliothèque de services `WCF`
    * Communication en `REST` 
        * avec `Proxy` pour récupérer des données récentes d'une station spécifique
        * avec l'***API de JCDecaux*** en `REST` pour récupérer, à l'initialisation, la liste de toutes les stations
    * Exposition `SOAP` et `REST` des méthodes de son interface [`IRouting`](Routing/IRouting.cs)

* [`Host`](Host)
    * Projet qui expose une version *console* du projet `Routing`
    * Possibilité d'afficher les traces d'exécutions de `Routing`

### Liens utiles

* [Dockeriser un hôte de service WCF](https://devblogs.microsoft.com/aspnet/lets-try-wcf-self-hosted-services-in-a-container/)
