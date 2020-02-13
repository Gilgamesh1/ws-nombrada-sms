using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APM.COM.UTIL;
using System.Net;
using APM.SENDSMS.SERVICE.Mensaje;
using System.IO;
using APM.COM.UTIL.Enum;

namespace APM.SENDSMS.SERVICE.Implementacion.SendingMethods
{
    public class Convergia: ISend
    {
        public Response SendSMS(string celular, string mensaje)
        {
            string actionUrl = Util.GetAppSettings("convergia.actionUrl");
            string supportedHttpMethod = Util.GetAppSettings("convergia.supportedHttpMethod");
            string httpAccept = Util.GetAppSettings("convergia.httpAccept");
            string contentType = Util.GetAppSettings("convergia.contentType");
            string authorizationHeaderKey = Util.GetAppSettings("convergia.authorizationHeaderKey");
            string username = Util.GetAppSettings("convergia.username");
            string password = Util.GetAppSettings("convergia.password");
            string proxy = Util.GetAppSettings("proxy");

            Response response = null;

            //SE VERIFICA SI EL CELULAR YA CONTIENE EL "51" AL INICIO
            celular = celular.StartsWith("51") ? celular : "51" + celular;

            ConvergiaSMS.Request convergiaRequest = new ConvergiaSMS.Request() { from = "APMTERMINALS", to = celular, text = mensaje };
            ConvergiaSMS.Response convergiaResponse = new ConvergiaSMS.Response();

            try
            {
                Uri address = new Uri(actionUrl);
                HttpWebRequest webRequest = WebRequest.Create(address) as HttpWebRequest;
                string jsonBody = Util.Stringify(convergiaRequest);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonBody);
                webRequest.Method = supportedHttpMethod;
                webRequest.Accept = httpAccept;
                webRequest.ContentType = contentType;
                webRequest.Headers.Add(authorizationHeaderKey, GetAuthorization(username, password));
                webRequest.Proxy = GetProxy(proxy);
                webRequest.ContentLength = buffer.Length;
                webRequest.GetRequestStream().Write(buffer, 0, buffer.Length);
                using (HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                    string jsonResponse = reader.ReadToEnd();
                    convergiaResponse = Util.Parse<ConvergiaSMS.Response>(jsonResponse);
                    convergiaResponse.jsonResponse = jsonResponse;
                    switch (convergiaResponse.messages[0].status.groupName)
                    {
                        case "REJECTED":
                            response = new Response() { _Codigo = CodigoError.ERROR, _Mensaje = jsonResponse };
                            break;
                        default:
                            response = new Response() { _Codigo = CodigoError.OK, _Mensaje = jsonResponse.Replace(Environment.NewLine, "") };
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response = new Response() { _Codigo = CodigoError.ERROR, _Mensaje = ex.Message };
            }

            return response;
        }

        private string GetAuthorization(string username, string password)
        {
            string userJoinPassword = username + ":" + password;
            byte[] bytes = Encoding.UTF8.GetBytes(userJoinPassword);
            return "Basic " + Convert.ToBase64String(bytes);
        }

        private WebProxy GetProxy(string proxy)
        {
            return new WebProxy()
            {
                Address = new Uri(proxy)
            };
        }
    }
}
