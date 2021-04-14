using Proxy.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

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

        /* OpenStreetMap Search */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "position?address={address}")]
        Position GetPosition(string address);

        /* OpenRouteService Directions */
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "path")]
        string GetPath(Position[] positions);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "path")]
        void Options();

        /* Calculation */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestStartStation?lat={latitude}&lng={longitude}")]
        Station FindNearestStationFromStart(double latitude, double longitude);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestEndStation?lat={latitude}&lng={longitude}")]
        Station FindNearestStationFromEnd(double latitude, double longitude);
    }
}
