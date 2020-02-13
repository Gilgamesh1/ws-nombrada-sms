using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APM.SENDSMS.SERVICE.Mensaje
{
    public class CMovilSMS
    {
        public class Request
        {
            public int apikey { get; set; }
            public string country { get; set; }
            public int dial { get; set; }
            public string messaje { get; set; }
            public long[] msisdns { get; set; }
            public string tag { get; set; }
            /*
            public string carrier { get; set; }//Opcional
            public string mask { get; set; }//Opcional
            public int msgClass { get; set; }//Opcional
            public string schedule { get; set; }//Opcional dd/mm/YYYYTHH:MM:ss
            */
            public override String ToString()
            {
                StringBuilder _msisdns = new StringBuilder();
                if (msisdns != null)
                {
                    _msisdns.Append("[");
                    foreach (long valor in msisdns)
                    {
                        _msisdns.Append(valor);
                        _msisdns.Append(", ");
                    }
                    if (_msisdns.Length > 1)
                    {
                        _msisdns = _msisdns.Remove(_msisdns.Length - 2, 1);
                    }
                    _msisdns.Append("]");
                }
                else
                    _msisdns.Append("[]");
                return "SmsRequest{" +
                        "apikey=" + apikey +
                        ", country='" + country + '\'' +
                        ", dial=" + dial +
                        ", messaje='" + messaje + '\'' +
                        ", msisdns=" + _msisdns +
                        ", tag='" + tag + '\'' +
                        /* ", carrier='" + carrier + '\'' +
                         ", mask='" + mask + '\'' +
                         ", msgClass=" + msgClass +
                         ", schedule='" + schedule + '\'' +*/
                        '}';
            }
        }

        public class Response
        {
            public int code { get; set; }
            public int mailingId { get; set; }
            public string result { get; set; }
            public string hint { get; set; }
            public string message { get; set; }

            public override string ToString()
            {
                return "SmsResponse{" +
                        "code=" + code +
                        ", mailingId=" + mailingId +
                        ", result='" + result + '\'' +
                        ", hint='" + hint + '\'' +
                        ", message='" + message + '\'' +
                        '}';
            }
        }
    }
}
