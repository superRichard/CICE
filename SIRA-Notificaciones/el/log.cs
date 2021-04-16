using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SIRA_Notificaciones.el
{
    public class log
    {
        #region atributos
        private string m_id;
        private string m_momento;
        #endregion

        #region constructor
            public log() { }
        #endregion

        #region propiedades
        public string Id { get => m_id; set => m_id = value; }
        public string Momento { get => m_momento; set => m_momento = value; }

        public void Guardar(string pId, string pMomento) {
            string bitacora = string.Empty;
            string pathLog = Directory.GetCurrentDirectory() + "\\log.txt";
            bitacora = pId +   "- " + pMomento + DateTime.Now + Environment.NewLine;
            File.AppendAllText(pathLog, bitacora);
        }
        #endregion
    }
}
