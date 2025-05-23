using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supernova.Models
{
    public class OMTask : OrangeButton.Models.OMTask
    {
        [Newtonsoft.Json.JsonProperty("Devices", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Device>? Devices { get; set; }

        [Newtonsoft.Json.JsonProperty("Documents", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<Document>? Documents { get; set; }

        [Newtonsoft.Json.JsonProperty("RelatedOMTasks", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IEnumerable<OMTask>? RelatedOMTasks { get; set; }
        public string? AlarmTypeId { get; set; }

        public new Scope Scope { get; set; }
    }
}
