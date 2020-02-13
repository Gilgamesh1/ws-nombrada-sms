using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;
using APM.SENDSMS.SERVICE.Mensaje;

namespace APM.SENDSMS.SERVICE.Contrato
{

    [ServiceContract]
    public interface IServiceSMS
    {
        [OperationContract]
        ServiceSMSResponse EnviarMensajeBatch(ServiceSMSRequest request);
    }

}