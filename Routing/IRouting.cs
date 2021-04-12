using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using static Proxy.Models.JCDecauxItem;

namespace Routing
{
    [ServiceContract]
    public interface IRouting
    {
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations")]
        List<Station> GetAllStations();

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations?city={city}")]
        List<Station> GetAllStationsFromCity(string city);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestStation?lat={latitude}&lng={longitude}")]
        Station FindNearestStation(double latitude, double longitude);
    }
}
