using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace APM.COM.UTIL
{
    public class Response
    {
        public string _Codigo { set; get; }
        public string _Mensaje { set; get; }
        public DataTable _Data { set; get; }
        public string _Xml { set; get; }
        [DefaultValue(0)]
        public int _RowAffected { set; get; }
    }
}
