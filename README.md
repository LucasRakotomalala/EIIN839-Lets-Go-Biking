# [Let's Go Biking !](https://lms.univ-cotedazur.fr/course/view.php?id=4334&amp;section=11)

## Auteur

- [Lucas Rakotomalala](https://github.com/LucasRakotomalala)

## Architecture

### Diagramme

<img src="resources/Diagramme d'architecture.png" alt="Diagramme d'architecture" style="margin: auto;"/>

### Implémentation

* [`HeavyClient`](HeavyClient)

* [`LightClient`](LightClient) : http://map.project-osrm.org/

* [`Proxy`](Proxy)

* [`ProxyHost`](ProxyHost) : projet dans lequel on expose une version dockerisé du proxy fonctionnel.

https://devblogs.microsoft.com/aspnet/lets-try-wcf-self-hosted-services-in-a-container/ + ajout de dns dans Docker Engine