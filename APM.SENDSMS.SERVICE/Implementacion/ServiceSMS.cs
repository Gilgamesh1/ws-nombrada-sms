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

        private Response EnvioSMSBatch(ServiceSMSRequest request)
        {
            int j = 0;
            string dataContactos = string.Empty;

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

            foreach (var item in request.ListaEnvio)
            {
                DataRow fila = tabla.NewRow();
                fila["Aplicacion"] = request.Aplicacion;
                fila["UsuarioEnvio"] = request.Usuario;
                fila["NroBoleta"] = item.NroBoleta;
                fila["FechaNombrada"] = item.FechaNombrada;
                fila["Turno"] = item.Turno;
                fila["CodTrabajador"] = item.CodTrabajador;
                fila["NombreTrabajador"] = item.NombreTrabajador;
                fila["Celular"] = item.Celular;
                fila["Mensaje"] = item.Mensaje;
                tabla.Rows.Add(fila);
                dataContactos = String.Concat(dataContactos, String.Format("{0},{1},{2}\n", j, item.Celular, item.Mensaje));
                j++;
            }

            Response responseSMS = SMSBatchBase(dataContactos);
            responseSMS._Xml =  Util.getXMLData(tabla);
            responseSMS._RowAffected = j;
            return responseSMS;
        }

        /* Envio Batch SMS*/
        private Response SMSBatchBase(string dataContactos)
        {
            Response response = null;
            try
            {
                string userSMS = Util.GetAppSettings(ParametroSMS.UserSMS);
                string passSMS = Util.GetAppSettings(ParametroSMS.PassSMS);
                string curl = Util.GetAppSettings(ParametroSMS.urlMasivo);
                string method = Util.GetAppSettings(ParametroSMS.Method);
                string contentType = Util.GetAppSettings(ParametroSMS.ContentType);
                string proxy = Util.GetAppSettings(ParametroSMS.Proxy);
                string data = String.Format("usuario={0}&clave={1}&bloque=%0d{2}", userSMS, passSMS, dataContactos);
                string datafinal = Uri.EscapeUriString(data);

                WebRequest url = WebRequest.Create(curl);
                url.Method = method;
                if (proxy != "")
                {
                    WebProxy prox = new WebProxy() { Address = new Uri(proxy) };
                    url.Proxy = prox;
                }
                byte[] byteArray = Encoding.UTF8.GetBytes(datafinal);
                url.ContentType = contentType;
                url.ContentLength = byteArray.Length;
                Stream dataStream = url.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                Stream objStream = url.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                string responseWebSMS = objReader.ReadLine();
                switch (responseWebSMS)
                {
                    case "OK": Console.WriteLine("Enviado");
                        response = new Response() { _Codigo = CodigoError.OK, _Mensaje = MensajeError.ERROR_OK };
                        break;
                    default:
                        response = new Response() { _Codigo = CodigoError.ERROR, _Mensaje = responseWebSMS };
                        break;
                }
            }
            catch (Exception ex)
            {
                response = new Response() { _Codigo = CodigoError.ERROR, _Mensaje = ex.Message };
            }

            return response;
        }

        /* Envio Simple SMS*/
        private Response SMSSimpleBase(string numero, string texto)
        {
            ISend sendOver;
            switch( Util.GetAppSettings("sendOver") ){
                case "MASIVO":
                    sendOver = new Masivo();
                    break;
                case "CONVERGIA":
                    sendOver = new Convergia();
                    break;
                case "CMOVIL":
                    sendOver = new CMovil();
                    break;
                default:
                    sendOver = new Masivo();
                    break;
            }
            return sendOver.SendSMS(numero, texto);
        }
    }
}
