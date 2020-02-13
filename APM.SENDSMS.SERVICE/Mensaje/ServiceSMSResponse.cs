using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using APM.SENDSMS.SERVICE.DTO;

namespace APM.SENDSMS.SERVICE.Mensaje
{
    [DataContract(Namespace = "http://www.apmterminals.com")]
    public class ServiceSMSResponse
    {
        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

    }
}
