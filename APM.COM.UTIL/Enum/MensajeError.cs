using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APM.COM.UTIL.Enum
{
    public static class MensajeError
    {
        public const string ERROR_OK = "Success";
        public const string ERROR_0 = "No se puede conectar al Servidor";
        public const string ERROR_1045 = "Credenciales invalidas, intentelo de nuevo";
        public const string ERROR_CONEXION = "No se pudo conectar a la base de datos";
    }
}
