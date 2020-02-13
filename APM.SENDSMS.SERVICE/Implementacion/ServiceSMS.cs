using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using APM.COM.DALC;
using APM.COM.UTIL;
using APM.COM.UTIL.Enum;
using APM.SENDSMS.SERVICE.Contrato;
using APM.SENDSMS.SERVICE.Mensaje;
using APM.SENDSMS.SERVICE.Implementacion.SendingMethods;
using NLog;

namespace APM.SENDSMS.SERVICE.Implementacion
{
    public class ServiceSMS : IServiceSMS
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ServiceSMSResponse EnviarMensajeBatch(ServiceSMSRequest request)
        {

            var config = new NLog.Config.LoggingConfiguration();
            Logger.Info(Util.GetAppSettings("url.nlog"));
            var vary = Util.GetAppSettings("url.nlog");
            // Targets where to log to: File and Console 
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = vary,
                Layout = "${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}"
            };
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            // Apply config           
            NLog.LogManager.Configuration = config;
            Logger.Info("ServiceSMS - EnviarMensajeBatch");

            try
            {
                int delay = int.Parse(Util.GetAppSettings(ParametroSMS.Delay));
                int totalSMS = request.ListaEnvio.Count;
                int smsEnviados = 0;
                int smsFallidos = 0;
                string proceso = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                const string mensaje = "Proceso: {0}\nTotal de Mensajes: {1}\nMensajes Enviados: {2}\nMensajes Fallidos: {3}";

                DataTable tabla = new DataTable();
                tabla.Columns.Add("Aplicacion");
                tabla.Columns.Add("UsuarioEnvio");
                tabla.Columns.Add("NroBoleta");
                tabla.Columns.Add("FechaNombrada");
                tabla.Columns.Add("Turno");
                tabla.Columns.Add("CodTrabajador");
                tabla.Columns.Add("NombreTrabajador");
                tabla.Columns.Add("Celular");
                tabla.Columns.Add("Mensaje");
                tabla.Columns.Add("Proceso");
                tabla.Columns.Add("Respuesta");
                tabla.Columns.Add("FechaEnvio");
                tabla.Columns.Add("Enviado");

                foreach (var item in request.ListaEnvio)
                {
                    Response responseSMS = EnvioSMSSimple(item.Celular, item.Mensaje, delay);
                    DataRow fila = tabla.NewRow();
                    fila["Aplicacion"] = request.Aplicacion;
                    fila["UsuarioEnvio"] = request.Usuario;
                    fila["NroBoleta"] = item.NroBoleta;
                    fila["FechaNombrada"] = item.FechaNombrada.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);  
                    fila["Turno"] = item.Turno;
                    fila["CodTrabajador"] = item.CodTrabajador;
                    fila["NombreTrabajador"] = item.NombreTrabajador;
                    fila["Celular"] = item.Celular;
                    fila["Mensaje"] = item.Mensaje;
                    fila["Proceso"] = proceso;
                    fila["Respuesta"] = responseSMS._Mensaje;
                    fila["FechaEnvio"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);                    

                    if (responseSMS._Codigo == CodigoError.OK)
                    {
                        fila["Enviado"] = "1";
                        smsEnviados++;
                    }
                    else
                    {
                        fila["Enviado"] = "0";
                        smsFallidos++;
                    }
                    tabla.Rows.Add(fila);
                }

                string xml = Util.getXMLData(tabla);
                object[,] paramSP = new object[,] { { "@p_XML", xml } };
                string mensajeFinal = string.Format(mensaje, proceso, totalSMS, smsEnviados, smsFallidos);

                Response responseBD = AccesoDatos.BaseData("UP_REGISTROSMS_BATCH_INSERT", paramSP, Util.GetConnectionStrings(ConexionBD.NOMBRADA));
                if (responseBD._Codigo != CodigoError.OK)
                {
                    return new ServiceSMSResponse() { Codigo = responseBD._Codigo, Mensaje = responseBD._Mensaje };
                }

                return new ServiceSMSResponse() { Codigo = CodigoError.OK, Mensaje = mensajeFinal };
            }
            catch (Exception ex)
            {
                Logger.Error("ServiceSMS EnviarMensajeBatch: {}", ex.Message);
                return new ServiceSMSResponse() { Codigo = CodigoError.ERROR, Mensaje = ex.Message };
            }
        }

        private Response EnvioSMSSimple(string contacto, string mensaje, int delay)
        {
            System.Threading.Thread.Sleep(delay);
            return SMSSimpleBase(contacto, mensaje);
        }

        /* Envio Simple SMS*/
        private Response SMSSimpleBase(string numero, string texto)
        {
            ISend sendOver;
            switch( Util.GetAppSettings("sendOver") ){
                case "CONVERGIA":
                    sendOver = new Convergia();
                    break;
                case "CMOVIL":
                    sendOver = new CMovil();
                    break;
                default:
                    sendOver = new CMovil();
                    break;
            }
            return sendOver.SendSMS(numero, texto);
        }
    }
}
