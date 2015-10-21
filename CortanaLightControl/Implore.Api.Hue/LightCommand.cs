using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Implore.Api.Hue
{
    [DataContract]
    class LightCommand
    {
        [DataMember(Name = "on")]
        public bool? On { get; set; }

        [DataMember(Name = "bri")]
        public byte? Brightness { get; set; }

        [DataMember(Name = "hue")]
        public int? Hue { get; set; }

        [DataMember(Name = "sat")]
        public int? Saturation { get; set; }

        [DataMember(Name = "xy")]
        public double[] ColorCoordinates { get; set; }

        [DataMember(Name = "ct")]
        public int? ColorTemperature { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember(Name = "alert")]
        public Alert? Alert { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember(Name = "effect")]
        public Effect? Effect { get; set; }


    }
    public enum Effect
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "colorloop")]
        ColorLoop
    }

    public enum Alert
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "select")]
        Once,
        [EnumMember(Value = "lselect")]
        Multiple

    }

}
