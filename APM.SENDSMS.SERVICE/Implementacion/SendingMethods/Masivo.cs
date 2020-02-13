using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APM.COM.UTIL;
using APM.COM.UTIL.Enum;
using System.Net;
using System.IO;

namespace APM.SENDSMS.SERVICE.Implementacion.SendingMethods
{
    public class Masivo : ISend
    {
        public Response SendSMS(string celular, string mensaje){
            Response response = null;
            try
            {
                string userSMS = Util.GetAppSettings(ParametroSMS.UserSMS);
                string passSMS = Util.GetAppSettings(ParametroSMS.PassSMS);
                string curl = Util.GetAppSettings(ParametroSMS.UrlSimple);
                string encoding = Util.GetAppSettings(ParametroSMS.Encoding);
                string contentType = Util.GetAppSettings(ParametroSMS.ContentTypeSimple);
                string proxy = Util.GetAppSettings(ParametroSMS.Proxy);
                string data = String.Format("{0}?API=1&USUARIO={1}&CLAVE={2}&TOS={3}&TEXTO={4}", curl, userSMS, passSMS, celular, mensaje);
                string responseWebSMS = string.Empty;

                Uri uri = new Uri(data);
                HttpWebRequest requestFile = (HttpWebRequest)WebRequest.Create(uri);
                requestFile.ContentType = contentType;

                if (proxy != "")
                {
                    WebProxy prox = new WebProxy() { Address = new Uri(proxy) };
                    requestFile.Proxy = prox;
                }

                HttpWebResponse webResp = requestFile.GetResponse() as HttpWebResponse;
                if (requestFile.HaveResponse)
                {
                    if (webResp.StatusCode == HttpStatusCode.OK || webResp.StatusCode == HttpStatusCode.Accepted)
                    {
                        StreamReader respReader = new StreamReader(webResp.GetResponseStream(), Encoding.GetEncoding(encoding));
                        responseWebSMS = respReader.ReadToEnd();
                    }
                }

                switch (responseWebSMS)
                {
                    case "OK\r\n":
                        response = new Response() { _Codigo = CodigoError.OK, _Mensaje = responseWebSMS.Replace(Environment.NewLine, "") };
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

    }
}
