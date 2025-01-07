using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeButton.Models
{
    public partial class PVSystem : System
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
    }

    public partial class PVArray
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
    }

    public partial class PVString
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
    }

    public partial class Device
    {
        [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Location>? Locations { get; set; }
        [Newtonsoft.Json.JsonProperty("Connections", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Connection>? Connections { get; set; }
    }
}
