namespace Supernova.Models
{
    public class OMIssue : OrangeButton.Models.OMIssue
    {
        public string? AlarmTypeId { get; set; }

        public new Scope Scope { get; set; }
    }
}
