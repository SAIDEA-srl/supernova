using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeButton.Models
{
    public class Document
    {
        public DocumentId DocumentId { get; set; }
        public DocumentName DocumentName { get; set; }
        //public DocumentProvider DocumentProvider { get; set; }
        public DocumentType DocumentType { get; set; }
        public DocumentUrl DocumentUrl { get; set; }
        public DocumentDate DocumentDate { get; set; }
    }

    public class DocumentId : TaxonomyElementString { }
    public class DocumentName : TaxonomyElementString { }
    //public class DocumentProvider : TaxonomyElementString { }
    public class DocumentType : TaxonomyElementString { }
    public class DocumentUrl : TaxonomyElementString { }
    public class DocumentDate : TaxonomyElementString { }
}
