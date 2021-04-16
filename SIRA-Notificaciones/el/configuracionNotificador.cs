using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SIRA_Notificaciones.el
{
   public static class configuracionNotificador
    {
        private static Boolean _timerActivo; //Indica si el Hilo esta activo
        private static Int32 _timeIntervalo; //Intervalo en Milisegundos de ejecucion del hilo
        private static Int32 _timeOutBD; //Time Out de procedures en la conexion con la BD
        private static String _urlWs; //Url del Servicio WEB de SIRA
        private static Int32 _timerOutWS; //Timpo de espera a que respondan los WS
        private static DataTable _timerMetodosWS;// Tabla en donde guardaremos los metodos registrados del WS con su estado

        public static Boolean timerActivo { get => _timerActivo; set => _timerActivo = value; }
        public static Int32 timerIntervalo { get => _timeIntervalo; set => _timeIntervalo = value; }
        public static Int32 timerOutBd { get => _timeOutBD; set => _timeOutBD = value; }
        public static String urlWs { get => _urlWs; set => _urlWs = value; }
        public static Int32 timerOutWS { get => _timerOutWS; set => _timerOutWS = value; }
        public static DataTable timerMetodosWS { get => _timerMetodosWS; set => _timerMetodosWS = value; }

    }
}
