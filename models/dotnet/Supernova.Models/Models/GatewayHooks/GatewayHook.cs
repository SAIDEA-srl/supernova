namespace Supernova.Models.GatewayHooks;

public partial class GatewayHook
{
    [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Name { get; set; }

    [Newtonsoft.Json.JsonProperty("Parameters", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public IDictionary<string, string> Parameters { get; set; }

    private IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }

}
