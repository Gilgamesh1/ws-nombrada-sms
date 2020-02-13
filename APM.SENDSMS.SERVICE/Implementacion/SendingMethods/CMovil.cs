using System;
using System.Text;
using APM.COM.UTIL;
using APM.SENDSMS.SERVICE.Mensaje;
using System.Net;
using System.IO;
using APM.COM.UTIL.Enum;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APM.SENDSMS.SERVICE.Implementacion.SendingMethods
{
    public class CMovil : ISend
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public Response SendSMS(string nroCelular, string mensaje)
        {
            Response response = null;
            try
            {
                var countryx = "PE";
                var msisdnsx = nroCelular.StartsWith("51") ? nroCelular : "51" + nroCelular;
                var tagx = "APM";
                string json = $@"
                   {{
                       ""apiKey"":{Util.GetAppSettings("cmovil.apikey")},
                       ""country"":""{countryx}"",
                       ""dial"":{Util.GetAppSettings("cmovil.dial")},
                       ""message"":""{mensaje}"",
                       ""msisdns"":[" + msisdnsx + $@"],
                       ""tag"":""{tagx}""
                   }}";
                Logger.Info("CMovil - smsRequest : {}", json);

                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                using (var httpClient = new HttpClient())
                {
                    using (var rqmsg = new HttpRequestMessage(new HttpMethod("POST"), Util.GetAppSettings("cmovil.actionUrl")))
                    {
                        rqmsg.Headers.TryAddWithoutValidation("Authorization", Util.GetAppSettings("cmovil.token"));
                        rqmsg.Headers.TryAddWithoutValidation("cache-control", "no-cache");

                        rqmsg.Content = new StringContent(json);
                        rqmsg.Content.Headers.ContentType = new MediaTypeHeaderValue(Util.GetAppSettings("cmovil.contentType"));
                        using (var httpResponseMessage = httpClient.SendAsync(rqmsg, HttpCompletionOption.ResponseHeadersRead))
                        {
                            var dataObjects = httpResponseMessage.Result.Content.ReadAsStringAsync();

                            CMovilSMS.Response smsResponse = Util.Parse<CMovilSMS.Response>(dataObjects.Result.ToString());
                            Logger.Info("CMovil - smsResponse : {}", smsResponse.ToString());
                            if (smsResponse.code == 0)
                            {
                                response = new Response() { _Codigo = CodigoError.OK, _Mensaje = smsResponse.result };
                            }
                            else
                            {
                                response = new Response() { _Codigo = CodigoError.ERROR, _Mensaje = smsResponse.message };
                            }
                            return response;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al consulta la web service de Concepto Movil");
                Logger.Error("{} \n {}", ex.Message, ex.StackTrace);
                return new Response() { _Codigo = CodigoError.ERROR, _Mensaje = ex.Message };
            }
        }
    }
}
