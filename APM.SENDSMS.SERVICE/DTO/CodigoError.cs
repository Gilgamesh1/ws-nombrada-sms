using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;
using APM.SENDSMS.SERVICE.Mensaje;

namespace APM.SENDSMS.SERVICE.DTO
{
    [DataContract(Name = "CodigoError")]
    public static class CodigoError
    {
        [DataMember]
        public const string OK = "200";

        [DataMember]
        public const string ERROR = "500";

        [DataMember]
        public const string VALIDACION = "300";
    }
}
