namespace Supernova.Models;

public partial class SupportedDocument
{
    [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Description { get; set; }

    [Newtonsoft.Json.JsonProperty("DocumentType", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string DocumentType { get; set; }

    private global::System.Collections.Generic.IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new global::System.Collections.Generic.Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }
}
