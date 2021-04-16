using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRA_Notificaciones.el
{
    public class siraConfiguraciones
    {
        #region atributos
        private string m_id;
        private string m_etiqueta;
        private string m_tipo;
        private string m_valor;
        private DateTime m_fecha;
        #endregion

        #region constructor
        public siraConfiguraciones() { }
        #endregion

        #region propiedades
        public string Id { get => m_id; set => m_id = value; }
        public string Etiqueta { get => m_etiqueta; set => m_etiqueta = value; }
        public string Tipo { get => m_tipo; set => m_tipo = value; }
        public string Valor { get => m_valor; set => m_valor = value; }
        public DateTime Fecha { get => m_fecha; set => m_fecha = value; }        
        #endregion

    }
}
