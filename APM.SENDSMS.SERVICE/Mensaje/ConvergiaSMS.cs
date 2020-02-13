using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APM.SENDSMS.SERVICE.Mensaje
{
    public class ConvergiaSMS
    {
        public class Request
        {
            public string from { get; set; }
            public string to { get; set; }
            public string text { get; set; }
        }

        public class Response
        {
            public Message[] messages { get; set; }
            public string jsonResponse { get; set; }

            public class Message
            {
                public string to { get; set; }
                public Status status { get; set; }
                public int smsCount { get; set; }
                public string MessageId { get; set; }

                public class Status
                {
                    public int groupId { get; set; }
                    public string groupName { get; set; }
                    public int id { get; set; }
                    public string name { get; set; }
                    public string description { get; set; }
                }
            }
        }
    }
}
