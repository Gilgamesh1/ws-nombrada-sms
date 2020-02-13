using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Script.Serialization;
using APM.COM.UTIL;
using APM.COM.UTIL.Enum;

namespace APM.COM.DALC
{
    public static class AccesoDatos
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #region Method-Invoke

        //Adexus: Method for getting a data with conection string custom
        public static Response BaseData(string spName, object[,] paramSP, string conexString)
        {
            return Base(spName, paramSP, conexString); ;
        }

        #endregion

        #region BASE

        //Adexus: Method for getting a DataTable object
        private static Response Base(string spName, object[,] paramSP, string conexString)
        {
            using (DataTable DT = new DataTable())
            {
                Response response = null;
                using (SqlConnection Connection_ = new SqlConnection(conexString))
                {
                    using (SqlCommand SqlCommand_ = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = spName, Connection = Connection_ })
                    {
                        using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommand_))
                        {
                            try
                            {
                                if (paramSP != null)
                                {
                                    for (int i = 0; i < paramSP.GetLength(0); i++)
                                    {
                                        string param= (string)paramSP[i, 0];
                                        if (param!= null && param!= "")
                                            SqlCommand_.Parameters.AddWithValue(param, paramSP[i, 1]);
                                    }
                                }
                                DA.Fill(DT);
                                response = new Response { _Codigo = CodigoError.OK, _Mensaje = MensajeError.ERROR_OK, _Data = DT };
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("Error AccesoDatos {}", ex.Message);
                                Logger.Error("Error AccesoDatos {}", ex.StackTrace);
                                response = new Response { _Codigo = CodigoError.ERROR, _Mensaje = ex.Message };
                            }
                        }
                    }
                }

                return response;
            }
        }

        #endregion
    }
}
