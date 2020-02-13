using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.ServiceReference1;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceSMSClient cliente = new ServiceSMSClient();
            ServiceSMSRequest request = new ServiceSMSRequest();
            ContactoDTO[] lista = { 
            new ContactoDTO { NroBoleta = "NOM-047322", FechaNombrada = DateTime.Now, Turno = "15:00-23:00", CodTrabajador = 101, NombreTrabajador = "Luis Soto", Celular = "969772149", Mensaje = "APM - Feliz Navidad" }};
        //new ContactoDTO { NroBoleta = "NOM-047321", FechaNombrada = DateTime.Now, Turno = "15:00-23:00", CodTrabajador = 100, NombreTrabajador="Jonathan Ascencio", Celular = "956747351", Mensaje = "Mensajin" } };
        request.Aplicacion = "Nombrada";
            request.Usuario = "user_admin";
            request.ListaEnvio = lista;
            ServiceSMSResponse response = cliente.EnviarMensajeBatch(request);
            Console.WriteLine(response.Codigo);
            Console.WriteLine(response.Mensaje);
        }
    }
}
