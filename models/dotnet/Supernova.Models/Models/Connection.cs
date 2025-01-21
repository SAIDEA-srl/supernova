using OrangeButton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supernova.Models;

public class Connection
{
    public ConnectionId ConnectionId { get; set; }
    public ConnectionType ConnectionType { get; set; }
    public FromDeviceId FromDeviceId { get; set; }
    public ToDeviceId ToDeviceId { get; set; }

    private global::System.Collections.Generic.IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new global::System.Collections.Generic.Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }
}

public class ConnectionId : TaxonomyElementString { }
public class ConnectionType : TaxonomyElementString { }
public class FromDeviceId : TaxonomyElementString { }
public class ToDeviceId : TaxonomyElementString { }
