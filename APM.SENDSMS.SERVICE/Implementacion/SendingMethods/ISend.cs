using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APM.COM.UTIL;

namespace APM.SENDSMS.SERVICE.Implementacion.SendingMethods
{
    public interface ISend
    {
        Response SendSMS(string celular, string mensaje);
        
    }
}
