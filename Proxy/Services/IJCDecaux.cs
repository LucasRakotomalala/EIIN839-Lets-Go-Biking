using Proxy.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Proxy
{
    [ServiceContract]
    public interface IJCDecaux
    {
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "stations?city={city}")]
        List<JCDecauxItem.Station> GetAllStationsFromCity(string city);

    }
}