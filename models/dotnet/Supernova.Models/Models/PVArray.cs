using OrangeButton.Models;

namespace Supernova.Models;

public class PVArray : OrangeButton.Models.PVArray
{
    [Newtonsoft.Json.JsonProperty("Locations", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public IEnumerable<Location>? Locations { get; set; }
}
