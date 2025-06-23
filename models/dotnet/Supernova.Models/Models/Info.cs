using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supernova.Models;

public partial class Info
{
    public string Name { get; set; }
    public string Source { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public List<SupportedDocument> SupportedDocuments { get; set; } = new List<SupportedDocument>();

    private global::System.Collections.Generic.IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new global::System.Collections.Generic.Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }
}
