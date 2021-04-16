using SIRA_Notificaciones.dal;
using SIRA_Notificaciones.el;
using SIRA_Notificaciones.forms;
using System;
using System.Data;
using System.Windows.Forms;

namespace SIRA_Notificaciones
{
    public partial class frmConfiguraciones : Form
    {
        public delegate void DelegadoConfiguraciones();
        public event DelegadoConfiguraciones MiEventoConfiguraciones;

        siraConfiguracionesDAL siraConfiguracion = new siraConfiguracionesDAL();
        frmDetalleConfiguracion ventanaConfiguracion = new frmDetalleConfiguracion();

        public frmConfiguraciones()
        {
            InitializeComponent();            
            ventanaConfiguracion.MiEventoConfiguracion += new frmDetalleConfiguracion.DelegadoConfiguracion(ConsultaSiraConfiguraciones);
        }

        #region Eventos
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmConfiguraciones_Load(object sender, EventArgs e)
        {         
            ConsultaSiraConfiguraciones();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            ventanaConfiguracion.InicializaValores("", "", -1, "", "", "");
            ventanaConfiguracion.ShowDialog(this);
        }

        private void gridDatos_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            String pId          = string.Empty; 
            String pEtiqueta    = string.Empty; 
            String pTipo        = string.Empty; 
            String pValor       = string.Empty; 
            Int32 pIdTipo       = 0 ;
            String pDecValor    = string.Empty; 
            String pFecha       = string.Empty; 

            try
            {
                pId             = this.gridDatos.Rows[e.RowIndex].Cells[0].Value.ToString();
                pEtiqueta       = this.gridDatos.Rows[e.RowIndex].Cells[1].Value.ToString();
                pTipo           = this.gridDatos.Rows[e.RowIndex].Cells[2].Value.ToString();
                pValor          = this.gridDatos.Rows[e.RowIndex].Cells[3].Value.ToString();
                pIdTipo         = Int32.Parse(this.gridDatos.Rows[e.RowIndex].Cells[4].Value.ToString());
                pDecValor       = this.gridDatos.Rows[e.RowIndex].Cells[5].Value.ToString();
                pFecha          = this.gridDatos.Rows[e.RowIndex].Cells[6].Value.ToString();

                ventanaConfiguracion.InicializaValores(pId, pEtiqueta, pIdTipo, pValor, pDecValor, pFecha);
                ventanaConfiguracion.ShowDialog(this);
            }
            catch (Exception ex) {     
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("gridDatos_CellMouseDoubleClick", ex.Message);
                #endregion
                MessageBox.Show("Error al cargar el evento", "..::: SIRA :::...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridWS_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            String pId          = string.Empty;
            String pEtiqueta    = string.Empty; 
            String pTipo        = string.Empty;
            String pValor       = string.Empty;
            Int32  pIdTipo      = 0;
            String pDescValor   = string.Empty;
            String pFecha       = string.Empty;


            try
            {
                pId         = this.gridWS.Rows[e.RowIndex].Cells[1].Value.ToString();
                pEtiqueta   = this.gridWS.Rows[e.RowIndex].Cells[2].Value.ToString();
                pTipo       = this.gridWS.Rows[e.RowIndex].Cells[3].Value.ToString();
                pValor      = this.gridWS.Rows[e.RowIndex].Cells[4].Value.ToString();
                pIdTipo     = Int32.Parse(this.gridWS.Rows[e.RowIndex].Cells[5].Value.ToString());
                pDescValor  = this.gridWS.Rows[e.RowIndex].Cells[6].Value.ToString();
                pFecha      = this.gridWS.Rows[e.RowIndex].Cells[7].Value.ToString();

                ventanaConfiguracion.InicializaValores(pId, pEtiqueta, pIdTipo, pValor, pDescValor, pFecha);
                ventanaConfiguracion.ShowDialog(this);
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("gridWS_CellMouseDoubleClick", ex.Message);
                #endregion
                MessageBox.Show("Error al cargar el evento", "..::: SIRA :::...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion;
        
        #region Metodos
        //Consulta catalogo de configuraciones 
        void ConsultaSiraConfiguraciones()
        {
            DataTable dtRespuesta = new DataTable();
            DataTable dtGenerales = new DataTable();
            DataTable dtWS = new DataTable();

            try
            {

                //Consulta en BD listado de configuraciones
                dtRespuesta = siraConfiguracion.ConsultaSiraConfiguraciones();

                //Filtra las que no son metodos de WS
                DataView viewGeneral = new DataView(dtRespuesta)
                {
                    RowFilter = "sits_id <> 2"
                };
                dtGenerales = viewGeneral.ToTable();
                this.gridDatos.DataSource = dtGenerales;


                //Filta las que son metodos de WS
                DataView viewWS = new DataView(dtRespuesta)
                {
                    RowFilter = "sits_id = 2"
                };
                dtWS = viewWS.ToTable();
                this.gridWS.DataSource = dtWS;

            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("ConsultaSiraConfiguraciones", ex.Message);
                #endregion
            }

            siraConfiguracion.LeerConfiguracionSira();

            try
            {
                frmMain.InicializaValores();
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("ConsultaSiraConfiguraciones", ex.Message);
                #endregion
            }

        }
        #endregion;
    }
}
