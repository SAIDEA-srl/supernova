namespace Supernova.Models
{
    public class OMWorkPlan : OrangeButton.Models.OMWorkPlan
    {
        [Newtonsoft.Json.JsonProperty("Tasks", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public OrangeButton.Models.Tasks Tasks { get; set; }
    }
}
