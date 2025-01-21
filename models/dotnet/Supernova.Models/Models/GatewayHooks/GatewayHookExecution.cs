namespace Supernova.Models.GatewayHooks;

public partial class GatewayHookExecution
{
    [Newtonsoft.Json.JsonProperty("Id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public Guid Id { get; set; }

    [Newtonsoft.Json.JsonProperty("HookName", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string HookName { get; set; }

    [Newtonsoft.Json.JsonProperty("Paramenters", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public IDictionary<string, object> Paramenters { get; set; }

    [Newtonsoft.Json.JsonProperty("DateTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public DateTimeOffset DateTime { get; set; }

    [Newtonsoft.Json.JsonProperty("Status", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public HookStatus Status { get; set; }

    [Newtonsoft.Json.JsonProperty("Result", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public GatewayHookResult Result { get; set; }

    private IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }

}
