using Proxy.Models;
using Routing.Models;
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
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "station?city={city}&number={stationNumber}")]
        Station GetStationInformations(string city, string stationNumber);

        /* OpenStreetMap Search */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "position?address={address}")]
        Position GetPosition(string address);

        /* OpenRouteService Directions */
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "path")]
        GeoJson GetPath(Position[] positions);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "path")]
        void PathOptions();

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "goToStation")]
        GeoJson GoToStation(Position[] positions);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "goToStation")]
        void GoToStationOptions();

        /* Calculation */
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestStartStation?lat={latitude}&lng={longitude}")]
        Station FindNearestStationFromStart(double latitude, double longitude);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestEndStation?lat={latitude}&lng={longitude}")]
        Station FindNearestStationFromEnd(double latitude, double longitude);

        /* Logs */
        [OperationContract]
        Dictionary<string, string> GetLogs();
    }
}
