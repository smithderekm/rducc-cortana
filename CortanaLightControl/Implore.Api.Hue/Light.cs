using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
namespace Implore.Api.Hue
{
    [DataContract]
    public class Light
    {
        [DataMember(Name="id") ]
        public string Id { get; set; }
        
        [DataMember(Name ="state")]
        public State State { get; set; }

        [DataMember(Name ="type")]
        public string Type { get; set; }

        [DataMember(Name ="name")]
        public string Name { get; set; }

        [DataMember(Name ="modelid")]
        public string ModelId { get; set; }

        [DataMember(Name ="uniqueid")]
        public string UniqueId { get; set; }

        [DataMember(Name ="manufacturername")]
        public string ManufacturerName { get; set; }

        [DataMember(Name ="swversion")]
        public string SoftwareVersion { get; set; }

        [DataMember(Name ="pointsymbol")]
        public Dictionary<string, string> PointSymbol { get; set; }

    }
}
