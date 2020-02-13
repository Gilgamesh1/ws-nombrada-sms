using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using APM.SENDSMS.SERVICE.DTO;

namespace APM.SENDSMS.SERVICE.Mensaje
{
    [DataContract(Namespace = "http://www.apmterminals.com")]
    public class ServiceSMSRequest
    {
        [DataMember]
        public string Aplicacion { get; set; }

        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public List<ContactoDTO> ListaEnvio { get; set; }
    }
}
