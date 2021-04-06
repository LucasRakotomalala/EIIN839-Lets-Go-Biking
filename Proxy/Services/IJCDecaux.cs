using Proxy.Models;
using System.ServiceModel;
using System.ServiceModel.Web;
using static Proxy.Models.JCDecauxItem;

namespace Proxy
{
    [ServiceContract]
    public interface IJCDecaux
    {
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations")]
        JCDecauxItem GetAllStations();

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations?city={city}")]
        JCDecauxItem GetAllStationsFromCity(string city);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "nearestStation?lat={latitude}&lng={longitude}")] 
        Station FindNearestStation(double latitude, double longitude);

    }
}