using Proxy.Models;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Proxy
{
    [ServiceContract]
    public interface IJCDecaux
    {
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "station?city={city}&number={stationNumber}")]
        JCDecauxItem GetStationDefault(string city, string stationNumber);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stationCache?city={city}&number={stationNumber}&cache={duration}")]
        JCDecauxItem GetStation(string city, string stationNumber, double duration);
    }
}