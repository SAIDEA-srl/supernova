namespace Supernova.Models.GatewayHooks;

public partial class GatewayHookResult
{
    [Newtonsoft.Json.JsonProperty("ExecutionId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public Guid ExecutionId { get; set; }

    [Newtonsoft.Json.JsonProperty("IsSuccessful", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public bool IsSuccessful { get; set; }

    [Newtonsoft.Json.JsonProperty("ResponseUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string ResponseUrl { get; set; }

    [Newtonsoft.Json.JsonProperty("Message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Message { get; set; }

    [Newtonsoft.Json.JsonProperty("DateTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public DateTimeOffset DateTime { get; set; }

    [Newtonsoft.Json.JsonProperty("ExpirationTime", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public DateTimeOffset? ExpirationTime { get; set; }

    private IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }

}
