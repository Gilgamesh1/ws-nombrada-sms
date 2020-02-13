using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APM.SENDSMS.SERVICE.Implementacion.SendingMethods;

namespace APM.SENDSMS.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            string celular = "972395720";
            string mensaje = "ESTO ES UNA PRUEBA";
            ISend sendMethod = new Convergia();
            sendMethod.SendSMS(celular, mensaje);
        }
    }
}
