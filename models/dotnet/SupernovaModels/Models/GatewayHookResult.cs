namespace Supernova.Models;

public partial class GatewayHookResult
{
    [Newtonsoft.Json.JsonProperty("ExecutionId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid ExecutionId { get; set; }

    [Newtonsoft.Json.JsonProperty("IsSuccessful", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public bool IsSuccessful { get; set; }

    [Newtonsoft.Json.JsonProperty("ResponseUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string ResponseUrl { get; set; }

    [Newtonsoft.Json.JsonProperty("DateTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset DateTime { get; set; }

    [Newtonsoft.Json.JsonProperty("ExpirationTime", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset? ExpirationTime { get; set; }

    private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }

}
