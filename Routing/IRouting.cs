using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using static Proxy.Models.JCDecauxItem;

namespace Routing
{
    [ServiceContract]
    public interface IRouting
    {
        /* JCDecaux */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations")]
        List<Station> GetAllStations();

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations?city={city}")]
        List<Station> GetAllStationsFromCity(string city);

        /* OpenStreetMap Reverse GeoCode */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "reverse?lat={latitude}&lng={longitude}")]
        string GetCityName(double latitude, double longitude);

        /* OpenRouteService Directions */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "path?startLat={latitudeStart}&startLng={longitudeStart}&endLat={latitudeEnd}&endLng={longitudeEnd}")]
        string GetPath(double latitudeStart, double longitudeStart, double latitudeEnd, double longitudeEnd);

        /* Calculation */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestStation?lat={latitude}&lng={longitude}")]
        Station FindNearestStation(double latitude, double longitude);
    }
}
