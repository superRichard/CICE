using SIRA_Notificaciones.dal;
using SIRA_Notificaciones.el;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/*+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+--+-+-+-+--+-+-+-+-+-+-+-+-+-+-+-+-+
RICHARD PERALES
isc.rperales@gmail.com
2299043001 
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+--+-+-+-+--+-+-+-+-+-+-+-+-+-+-+-+-+*/
namespace SIRA_Notificaciones
{
    public partial class frmMain : Form
    {
        public static Int32 contador = 0;
        siraConfiguracionesDAL dtSiraConfiguracion = new siraConfiguracionesDAL();        
        private static System.Timers.Timer timer;


        public frmMain()
        {
            InitializeComponent();           
        }

        #region Funcionalidades del formulario      

        private void frmMain_Load(object sender, EventArgs e)
        {           
            LeerConfiguracion();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        private void btnNotificaciones_Click(object sender, EventArgs e)
        {
            AbrirFormulario<frmNotificaciones>();
            btnNotificaciones.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            AbrirFormulario<frmLogs>();
            btnConfiguraciones.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnConfiguraciones_Click(object sender, EventArgs e)
        {
            AbrirFormulario<frmConfiguraciones>();
            btnLogs.BackColor = Color.FromArgb(12, 61, 92);
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void AbrirFormulario<MiForm>() where MiForm : Form, new()
        {
            Form formulario;
            formulario = panelformularios.Controls.OfType<MiForm>().FirstOrDefault();//Busca en la colecion el formulario
            //si el formulario/instancia no existe
            if (formulario == null)
            {
                formulario = new MiForm();
                formulario.TopLevel = false;
                formulario.FormBorderStyle = FormBorderStyle.None;
                formulario.Dock = DockStyle.Fill;
                panelformularios.Controls.Add(formulario);
                panelformularios.Tag = formulario;
                formulario.Show();
                formulario.BringToFront();
                formulario.FormClosed += new FormClosedEventHandler(CloseForms);
            }
            //si el formulario/instancia existe
            else
            {
                formulario.BringToFront();
            }
        }
        private void CloseForms(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms["frmNotificaciones"] == null)
                btnNotificaciones.BackColor = Color.FromArgb(4, 41, 68);
            if (Application.OpenForms["frmLogs"] == null)
                btnConfiguraciones.BackColor = Color.FromArgb(4, 41, 68);
            if (Application.OpenForms["frmConfiguraciones"] == null)
                btnLogs.BackColor = Color.FromArgb(4, 41, 68);
        }

        #endregion

        #region Metodos funcionalidad
        //Consultamos de la BD las configuraciones para los controles
        //Este metodo tambien puede ser invocado desde el formulario frmConfiguraciones.cs, 
        //esto por que toda vez que cambien alguna configuracion se apliquen de forma automatica
        //sin reinicier el .EXE
          public void LeerConfiguracion()
          {

            //Consultamos configuracion en BD y se llena objeto GLOBAL con los valores configurados
            //el objeto es de la clase "configuracionNotificador.cs"
            dtSiraConfiguracion.LeerConfiguracionSira();
                
            //Asigna configuracion a controles
            InicializaValores();
           }
        
        
        //Asignamos valores configurados al TIMER
        public static void InicializaValores()
            {

            //Console.WriteLine("timerIntervalo -> " + configuracionNotificador.timerIntervalo.ToString());
            //Console.WriteLine("timerActivo -> " + configuracionNotificador.timerActivo.ToString());

            //Antes de asignar valores, si existe el objeto lo detenemos y matamos
            //StopTimer();

            //Creamos y asiganamos valores al TIMER
            timer = new System.Timers.Timer(configuracionNotificador.timerIntervalo);
            // aqui activar //////////////timer.Elapsed += EnviaPeticionesSira;
            timer.AutoReset = true;
            timer.Enabled = configuracionNotificador.timerActivo;
            siraPeticiones s = new siraPeticiones();
            //s.pruebaSimple(); // prueba de ingreso simple
            s.IngresoSinID();
            s.IngresoParcial();
        }

        public static void StopTimer() {
            try
            {
                timer.Stop();
                timer.Dispose();
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("StopTimer Main", ex.Message);
                #endregion
            }
        }
                
        private static void EnviaPeticionesSira(Object source, ElapsedEventArgs e)
        {                
            siraPeticiones siraPeticion = new siraPeticiones();            
            DataTable dtMetodosActivos = new DataTable();
            
            //Cramos un DataView con el dataSet que tenemos en objeto Global 
            //y filtramos los metodos que estan activos
            DataView vwMetodos = new DataView(configuracionNotificador.timerMetodosWS);
            vwMetodos.RowFilter = "sico_valor = 1";
            dtMetodosActivos = vwMetodos.ToTable();

            Console.WriteLine("------------------------- [ Ejecucion No -> " + contador.ToString() + " ] --------------------------" );

            //Recorremos datatable donde guardamos metodos activos 
            //y ejecutamos peticion para cada uno de ellos
            for (int i = 0; i < dtMetodosActivos.Rows.Count; i++) {

                String nombreMetodo = dtMetodosActivos.Rows[i]["sico_etiqueta"].ToString();                

                switch (nombreMetodo)
                {
                    case "ingresoSimple":
                        siraPeticion.IngresoSimple();
                        Console.WriteLine("---> Ejecutó ingresoSimple");
                        break;
                        //case "ingresoParcial":
                        //    siraPeticion.IngresoParcial();
                        //    Console.WriteLine("---> Ejecutó ingresoParcial");
                        //    break;
                        //case "ingresoSinID":
                        //    siraPeticion.IngresoSinID();
                        //    Console.WriteLine("---> Ejecutó ingresoSinID");
                        //    break;
                        //case "ingresoFlujoAlterno":
                        //    siraPeticion.IngresoFlujoAlterno();
                        //    Console.WriteLine("---> Ejecutó ingresoFlujoAlterno");
                        //    break;
                        //case "rechazo":
                        //    siraPeticion.Rechazo();
                        //    Console.WriteLine("---> Ejecutó rechazo");
                        //break;                        
                    default:
                        break;
                }
                
            } 
            
            contador += 1;
        }
        #endregion;
        
    }
}
