using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    public class JCDecauxItem
    {
        [DataMember]
        public List<Station> stations { get; set; }
    }
}