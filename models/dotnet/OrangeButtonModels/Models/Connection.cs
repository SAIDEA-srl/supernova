using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeButton.Models
{
    public class Connection
    {
        public ConnectionId ConnectionId { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public FromDeviceId FromDeviceId { get; set; }
        public ToDeviceId ToDeviceId { get; set; }
    }

    public class ConnectionId : TaxonomyElementString { }
    public class ConnectionType : TaxonomyElementString { }
    public class FromDeviceId : TaxonomyElementString { }
    public class ToDeviceId : TaxonomyElementString { }
}
