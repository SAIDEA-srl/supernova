using OrangeButton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supernova.Models;

public class Document
{
    public DocumentId DocumentId { get; set; }
    public DocumentName DocumentName { get; set; }
    //public DocumentProvider DocumentProvider { get; set; }
    public DocumentType DocumentType { get; set; }
    public DocumentUrl DocumentUrl { get; set; }
    public DocumentDate DocumentDate { get; set; }

    private global::System.Collections.Generic.IDictionary<string, object> _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new global::System.Collections.Generic.Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }
}

public class DocumentId : TaxonomyElementString { }
public class DocumentName : TaxonomyElementString { }
//public class DocumentProvider : TaxonomyElementString { }
public class DocumentType : TaxonomyElementString { }
public class DocumentUrl : TaxonomyElementString { }
public class DocumentDate : TaxonomyElementString { }

