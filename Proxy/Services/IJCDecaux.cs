using Proxy.Models;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Proxy
{
    [ServiceContract]
    public interface IJCDecaux
    {
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "station?city={city}&number={number}")]
        JCDecauxItem GetStation(string city, string number);
    }
}