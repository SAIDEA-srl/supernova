using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeButton.Models.Extensions
{
    public class PVSystem : OrangeButton.Models.PVSystem
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
    }

    public class PVArray : OrangeButton.Models.PVArray
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
    }

    public class PVString : OrangeButton.Models.PVString
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
    }

    public class Device : OrangeButton.Models.Device
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
        [Newtonsoft.Json.JsonProperty("Connections", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Connection>? Connections { get; set; }
    }
}
