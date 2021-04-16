using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace SIRA_Notificaciones.el
{
    public class oracleParam
    {
        #region atributos
        private string m_id;
        private object m_valor;
        private OracleDbType m_tipo;
        private ParameterDirection m_direccion;
        #endregion

        #region constructor
            public oracleParam(string v, int pTipo, string pValor) { }
            public oracleParam(string id, object valor, OracleDbType tipo, ParameterDirection direccion) {
                m_id = id;
                m_valor = valor;
                m_tipo = tipo;
                m_direccion = direccion;
            }
        #endregion

        #region propiedades
        public string Id { get => m_id; set => m_id = value; }
        public object Valor { get => m_valor; set => m_valor = value; }
        public OracleDbType Tipo { get => m_tipo; set => m_tipo = value; }
        public ParameterDirection Direccion { get => m_direccion; set => m_direccion = value; }
        #endregion

    }
}
