using OrangeButton.Models;

namespace Supernova.Models
{
    public class OMIssue : OrangeButton.Models.OMIssue
    {
        public AlarmTypeId? AlarmTypeId { get; set; }
    }

    public class AlarmTypeId : TaxonomyElementString { }
}
