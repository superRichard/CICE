using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SIRA_Notificaciones.el;
using Oracle.ManagedDataAccess.Client;

namespace SIRA_Notificaciones.dal
{
    public class OracleDAL
    {
        OracleConnection cn;
        private int consTimeOut = configuracionNotificador.timerOutBd;

        public OracleDAL()        {

            string connectionString = ConfigurationManager.ConnectionStrings["ConectSIRA"].ConnectionString;
            cn = new OracleConnection(connectionString);            
        }

        public void Execute_Non_Query_Sp_Sin_Retorno(string nameSP, List<KeyValuePair<string, object>> listParams)
        {
            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection = cn;
                    cm.CommandTimeout = consTimeOut;
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = nameSP;

                    if (listParams != null && listParams.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> parameter in listParams)
                        {
                            cm.Parameters.Add(parameter.Key, parameter.Value);
                        }
                    }
                    cn.Open();
                    cm.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        public DataTable Execute_Reader_Sp_DataTable(string sql, List<oracleParam> listParams)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection       = cn;
                    cm.CommandTimeout   = consTimeOut;
                    cm.CommandType      = CommandType.StoredProcedure;
                    cm.CommandText      = sql;

                    if (listParams != null && listParams.Count > 0)
                    {
                        cm.Parameters.Clear();
                        foreach (oracleParam parameter in listParams)
                        {
                            if (parameter.Direccion == ParameterDirection.Output)
                                cm.Parameters.Add(parameter.Id, parameter.Tipo, parameter.Direccion);
                            else
                                cm.Parameters.Add(parameter.Id, parameter.Valor);
                        }
                    }

                    cn.Open();
                    using (OracleDataReader rd = cm.ExecuteReader())
                    {
                        dt.Load(rd);
                    }
                    cn.Close();
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }

            return dt;
        }

        public int Execute_Non_Query_Sp_Int(string nameSP, List<KeyValuePair<string, object>> list_params)
        {
            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection = cn;
                    cm.CommandTimeout = consTimeOut;
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = nameSP;

                    if (list_params != null && list_params.Count > 0)
                    {
                        cm.Parameters.Clear();

                        foreach (KeyValuePair<string, object> parameter in list_params)
                        {
                            cm.Parameters.Add(parameter.Key, parameter.Value);
                        }

                        if (cm.Parameters.Contains("result"))
                            cm.Parameters["result"].Direction = ParameterDirection.Output;
                    }

                    cn.Open();

                    int result = cm.ExecuteNonQuery();

                    if (cm.Parameters.Contains("result"))
                    {
                        result = (int)cm.Parameters["result"].Value;
                    }
                    cn.Close();
                    return result;

                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        public string Execute_Non_Query_Sp_String(string nameSP, List<KeyValuePair<string, object>> list_params)
        {
            #region
            try
            {
                object resultado = null;
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection = cn;
                    cm.CommandTimeout = consTimeOut;
                    cm.CommandType = CommandType.StoredProcedure; 
                    cm.CommandText = nameSP;

                    if (list_params != null && list_params.Count > 0)
                    {
                        cm.Parameters.Clear();

                        foreach (KeyValuePair<string, object> list in list_params)
                        {
                            cm.Parameters.Add(list.Key, list.Value);
                        }

                        if (cm.Parameters.Contains("result"))
                        {
                            cm.Parameters["result"].Direction = ParameterDirection.Output;
                        }
                    }

                    cn.Open();
                    cm.ExecuteNonQuery();

                    if (cm.Parameters.Contains("result"))
                    {
                        resultado = (String)cm.Parameters["result"].Value;
                    }

                }

                if (resultado == null)
                {
                    resultado = "Nulo";
                }
                cn.Close();
                return resultado.ToString();

            }
            catch (Exception e)
            {

                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }
            #endregion
        }

        public DataTable Data_Adapter_Fill_DataTable(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection = cn;
                    cm.CommandTimeout = consTimeOut;
                    cm.CommandType = CommandType.Text; //Indicas el tipo de comando a usar
                    cm.CommandText = sql;

                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = cm;
                    da.Fill(dt);
                }
                cn.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }
            return dt;
        }

        public DataSet Data_Adapter_Fill_DataSet(string sql)
        {
            DataSet ds = new DataSet();

            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection = cn;
                    cm.CommandTimeout = consTimeOut;
                    cm.CommandType = CommandType.Text; //Indicas el tipo de comando a usar
                    cm.CommandText = sql;

                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = cm;
                    da.Fill(ds, "TABLA");
                }
                cn.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }

            return ds;
        }


        public int Execute_Non_Query_Int(string sql)
        {
            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection       = cn;
                    cm.CommandTimeout   = consTimeOut;
                    cm.CommandType      = CommandType.Text; //Indicas el tipo de comando a usar
                    cm.CommandText      = sql;
                    cn.Open();
                    return cm.ExecuteNonQuery();
                }
                cn.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        public DataTable Execute_Reader_DataTable(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection       = cn;
                    cm.CommandTimeout   = consTimeOut;
                    cm.CommandType      = CommandType.StoredProcedure;
                    cm.CommandText      = sql;
                    cn.Open();
                    using (OracleDataReader dr = cm.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                    cn.Close();
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }

            return dt;
        }

        public string Execute_Scalar_String(string sql)
        {
            object resultado;
            try
            {
                using (OracleCommand cm = new OracleCommand())
                {
                    cm.Connection = cn;
                    cm.CommandTimeout = consTimeOut;
                    cm.CommandType = CommandType.Text;
                    cm.CommandText = sql;
                    cn.Open();
                    resultado = cm.ExecuteScalar();

                    if (resultado == null)
                        resultado = "Nulo";
                    cn.Close();
                    return resultado.ToString();
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                cn.Close();
            }
        }
    }

}