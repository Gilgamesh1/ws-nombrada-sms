using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;
using APM.SENDSMS.SERVICE.Mensaje;

namespace APM.SENDSMS.SERVICE.DTO
{
    [DataContract(Name = "ContactoDTO")]
    public class ContactoDTO
    {
        [DataMember]
        public string NroBoleta { get; set; }
        
        [DataMember]
        public DateTime FechaNombrada { get; set; }

        [DataMember]
        public string Turno { get; set; }
        
        [DataMember]
        public int CodTrabajador { get; set; }

        [DataMember]
        public string NombreTrabajador { get; set; }

        [DataMember]
        public string Celular { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
    }
}
