using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SIRA_Notificaciones.el;
using System.Xml;
using Oracle.ManagedDataAccess.Client;

namespace SIRA_Notificaciones.dal
{
    public class siraHandlerDAL
    {

        private OracleDAL oracleDAL;

        public siraHandlerDAL()
        {
            oracleDAL = new OracleDAL();
        }

        //Consulta Bitacora de eventos
        public DataTable ConsultaSiraNotificador(string pFechaInicio, string pFechaFin, Int32 pEstado)
        {
            try
            {
                #region ESTRUCTURA DE BD
                /*
                                CREATE TABLE SIRA_HANDLER
                                (
                                  SIHA_ID           NUMBER(10)
                                , SIHA_evento       VARCHAR2(50)
                                , SIHA_descripcion  VARCHAR2(2000)
                                , SIHA_xml          SYS.XMLTYPE 
                                , SIHA_fecha        DATE
                                , CONSTRAINT PK_SIRA PRIMARY KEY(SIHA_ID) ENABLE
                                );

                                COMMENT ON COLUMN SIRA_HANDLER.SIHA_ID              IS 'Identificador primario';
                                COMMENT ON COLUMN SIRA_HANDLER.SIHA_evento          IS 'Etiqueta o identificador de campo';
                                COMMENT ON COLUMN SIRA_HANDLER.SIHA_descripcion     IS 'Descripcion del Evento, en el caso de SW ira vacio';
                                COMMENT ON COLUMN SIRA_HANDLER.SIHA_xml             IS 'Valor del request7response de cada invocacion de sw';
                                COMMENT ON COLUMN SIRA_HANDLER.SIHA_fecha           IS 'Fecha de registro';
                */
                #endregion
                string sp       = "XXSIRA_PACK_NOTIFICADOR.consultaSiraNotificador";
                DataTable result  = null;

                List<oracleParam> list_params = new List<oracleParam>()
                {
                    new  oracleParam("pFechaInicio" ,pFechaInicio,  OracleDbType.Varchar2 ,ParameterDirection.Input ),
                    new  oracleParam("pFechaFin"    ,pFechaFin,     OracleDbType.Varchar2,ParameterDirection.Input ),
                    new  oracleParam("pStatus"    ,pEstado,     OracleDbType.Int32,ParameterDirection.Input ),
                    new  oracleParam("pHandler"   ,result,    OracleDbType.RefCursor, ParameterDirection.Output )
                };

                result = oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);

                return result;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        //Inserta evento
        public DataTable InsertaSiraNotificador(Int32 pIdOperacion,String pEvento, String pDescripcion, String pXmlRequest, String pXmlResponse, Int32 pStatus ) {

            try
            {
            
                string sp = "XXSIRA_PACK_NOTIFICADOR.insertaSiraNotificador";
                DataTable result = null;

                List<oracleParam> list_params = new List<oracleParam>()
                {
                    new  oracleParam("pEvento",pEvento,  OracleDbType.Varchar2 ,ParameterDirection.Input ),
                    new  oracleParam("pDescripcion" ,pDescripcion, OracleDbType.Varchar2,ParameterDirection.Input ),
                    new  oracleParam("pXMLRequest",pXmlRequest,   OracleDbType.Varchar2  ,ParameterDirection.Input ),
                    new  oracleParam("pIdOperacion",pIdOperacion,   OracleDbType.Int32  ,ParameterDirection.Input ),
                    new  oracleParam("pXMLResponse",pXmlResponse,   OracleDbType.Varchar2  ,ParameterDirection.Input),
                    new  oracleParam("pStatus",pStatus,   OracleDbType.Int32  ,ParameterDirection.Input),
                    new  oracleParam("pRespuesta"   ,result,    OracleDbType.RefCursor, ParameterDirection.Output )
                };

                result = oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);

                return result;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public DataTable ConsultaSiraNotificadorXId(Int32 pIdEvento)
        {
            try
            {                
                string sp = "XXSIRA_PACK_NOTIFICADOR.consultaSiraNotificadorXId";
                DataTable result = null;

                List<oracleParam> list_params = new List<oracleParam>()
                {                    
                    new  oracleParam("pConsecutivo",pIdEvento,     OracleDbType.Int32,ParameterDirection.Input ),
                    new  oracleParam("pHandler",result,    OracleDbType.RefCursor, ParameterDirection.Output )
                };

                result = oracleDAL.Execute_Reader_Sp_DataTable(sp, list_params);

                return result;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
    }
}   
