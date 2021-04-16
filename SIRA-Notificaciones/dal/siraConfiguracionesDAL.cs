using SIRA_Notificaciones.el;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace SIRA_Notificaciones.dal
{
    public class siraConfiguracionesDAL
    {

        private OracleDAL oracleDAL;


        public siraConfiguracionesDAL()
        {
            oracleDAL = new OracleDAL();
        }

        //Insertamos una nueva configuracion
        public DataTable InsertaSiraConfiguraciones(String pEtiqueta, Int32 pTipo, String pValor)
        {
            DataTable result = null;
            try
            {

                string sp = "XXSIRA_PACK_NOTIFICADOR.insertaSiraConfiguraciones";
               

                List<oracleParam> list_params = new List<oracleParam>()
                {
                    new  oracleParam("pEtiqueta",pEtiqueta,  OracleDbType.Varchar2 ,ParameterDirection.Input ),
                    new  oracleParam("pTipo" ,pTipo, OracleDbType.Int32 ,ParameterDirection.Input ),
                    new  oracleParam("pValor",pValor,   OracleDbType.Varchar2  ,ParameterDirection.Input ),
                    new  oracleParam("pRespuesta"   ,result,    OracleDbType.RefCursor, ParameterDirection.Output )
                };

                result = oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);

                
            }
            catch (Exception ex)
            {
                //throw new ApplicationException(e.Message);
                result = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.InsertaSiraConfiguraciones", ex.Message);
                #endregion
            }
            return result;
        }

        //Actulizamos alguna configuracion
        public DataTable ActualizaSiraConfiguraciones(Int32 pID, String pEtiqueta, Int32 pTipo, String pValor)
        {
            DataTable result = null;
            try
            {

                string sp = "XXSIRA_PACK_NOTIFICADOR.actualizaSiraConfiguraciones";
                

                List<oracleParam> list_params = new List<oracleParam>()
                {
                    new  oracleParam("pConsecutivo",pID,  OracleDbType.Varchar2 ,ParameterDirection.Input ),
                    new  oracleParam("pEtiqueta",pEtiqueta,  OracleDbType.Varchar2 ,ParameterDirection.Input ),
                    new  oracleParam("pTipo" ,pTipo, OracleDbType.Int32 ,ParameterDirection.Input ),
                    new  oracleParam("pValor",pValor,   OracleDbType.Varchar2  ,ParameterDirection.Input ),
                    new  oracleParam("pRespuesta"   ,result,    OracleDbType.RefCursor, ParameterDirection.Output )
                };

                result = oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);

               
            }
            catch (Exception ex)
            {
                //throw new ApplicationException(e.Message);
                result = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.ActualizaSiraConfiguraciones", ex.Message);
                #endregion
            }
            return result;
        }

        //Consulta en BD el catalgo de "tiposServicios" para llenar combo         
        public DataTable ConsultaSiraServicios()
        {
            DataTable result = null;
            try
            {
                string sp = "XXSIRA_PACK_NOTIFICADOR.consultaSiraServicios";

               
                List<oracleParam> list_params = new List<oracleParam>()
                {
                    new oracleParam("pTipoServicio",result, OracleDbType.RefCursor, ParameterDirection.Output )
                };

                return oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);
            }
            catch (Exception ex)
            {
                //throw new ApplicationException(e.Message);
                result = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.ConsultaSiraServicios", ex.Message);
                #endregion
            }
            return result;
        }

        //Peticion a la BD consultado tabla de configuraciones
        public DataTable ConsultaSiraConfiguraciones()
        {
            DataTable result = null;
            try
            {
                string sp = "XXSIRA_PACK_NOTIFICADOR.consultaSiraConfiguraciones";
                
                List<oracleParam> list_params = new List<oracleParam>()
                {
                    new oracleParam("pConfiguraciones",result, OracleDbType.RefCursor, ParameterDirection.Output )
                };

                result= oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);
            }
            catch (Exception ex)
            {
                //throw new ApplicationException(e.Message);
                result = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.ConsultaSiraConfiguraciones", ex.Message);
                #endregion
            }
            return result;
        }


        //Consultamos en BD la tabla de configuraciones, y los valores son guardados
        //En un objeto Global de tipo "configuracionNotificador.cs"
        public void LeerConfiguracionSira()
        {
            DataTable dtConfiguracion = new DataTable();

            Boolean vStatusNotificador = false;
            Int32 vTimeOutBD            = 0;
            Int32 vTimeOutWS            = 0;
            String vUrlWSRecinto        = string.Empty;
            String vUrlSira             = string.Empty;
            Int32 vTimerWS              = 0;

            try
            {
                //////////aqui activar
                //vStatusNotificador = Convert.ToInt32(CosultaValorConfiguracion(0)) == 0 ? false : true;
                //vTimeOutBD          = Int32.Parse(CosultaValorConfiguracion(1));
                //vTimeOutWS          = Int32.Parse(CosultaValorConfiguracion(2));
                //vUrlWSRecinto       = CosultaValorConfiguracion(3);
                //vUrlSira            = CosultaValorConfiguracion(4);
                //vTimerWS            = Int32.Parse(CosultaValorConfiguracion(5));

                vStatusNotificador =  true;
                vTimeOutBD      = 15;
                vTimeOutWS      = 15;
                vUrlWSRecinto   = "https://www.grupocice.com/serviciosgf2/Sira/OperacionEntradaService?wsdl";
                vUrlSira        = "https://201.151.252.116:8101/OperacionEntradaImpl/OperacionEntradaService?wsdl";
                vTimerWS        = 1;

                configuracionNotificador.timerActivo = vStatusNotificador;
                configuracionNotificador.timerOutBd = (vTimeOutBD * 1000);
                configuracionNotificador.timerOutWS = (vTimeOutWS * 1000);
                configuracionNotificador.timerIntervalo = (vTimerWS * 1000);
                configuracionNotificador.urlWs = vUrlSira;

                //////////aqui activar
                //configuracionNotificador.timerMetodosWS = ConsultaMetodosWSActivos();

                configuracionNotificador.timerMetodosWS = null;
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.LeerConfiguracionSira", ex.Message);
                #endregion
            }
        }

        //Metodo que recibe un "Id" de configuracion, consulta su valor 
        //y lo retorna como String
        public String CosultaValorConfiguracion(Int32 pIdConfiguracion)
        {
            String vRespuesta;
            DataTable dtConfiguracion = new DataTable();
            DataTable dtResultado = new DataTable();

            try
            {
                dtConfiguracion = ConsultaSiraConfiguraciones();

                DataView vwConfiguracion = new DataView(dtConfiguracion)
                {
                    RowFilter = "sico_id = " + pIdConfiguracion
                };
                dtResultado = vwConfiguracion.ToTable();

                vRespuesta = dtResultado.Rows[0]["SICO_VALOR"].ToString();

            }
            catch (Exception ex)
            {
                vRespuesta = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.CosultaValorConfiguracion", ex.Message);
                #endregion
            }

            return vRespuesta;
        }


        //Guardamos en el objeto Global de configuraciones, el listado de metodos del WS
        //con su respctivo estado "Activo" o "Inactivo"
        //ya que en cada peticion que se haga al WS, se validara antes que tengamos el metodo en estado "Activo"
        public DataTable ConsultaMetodosWSActivos() {            
            DataTable dtConfiguracion = new DataTable();
            DataTable dtResultado = new DataTable();

            try
            {
                dtConfiguracion = ConsultaSiraConfiguraciones();

                DataView vwConfiguracion = new DataView(dtConfiguracion)
                {
                    RowFilter = "sits_id = 2"
                };
                dtResultado = vwConfiguracion.ToTable();              

            }
            catch (Exception ex)
            {
                dtResultado = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraConfiguracionesDAL.ConsultaMetodosWSActivos", ex.Message);
                #endregion
            }

            return dtResultado;
        }
    }
}
