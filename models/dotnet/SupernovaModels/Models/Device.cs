using OrangeButton.Models;

namespace Supernova.Models;

public class Device : OrangeButton.Models.Device
{
    [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public IEnumerable<Location>? Locations { get; set; }
    [Newtonsoft.Json.JsonProperty("Connections", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public IEnumerable<Connection>? Connections { get; set; }
}
