using SIRA_Notificaciones.el;
using SIRA_Notificaciones.dal;
using SIRA_Notificaciones.forms;
using System;
using System.Data;
using System.Windows.Forms;

namespace SIRA_Notificaciones
{
    public partial class frmNotificaciones : Form
    {
        siraHandlerDAL dtSiraHandler = new siraHandlerDAL();
        public frmNotificaciones()
        {
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        public void ConsultaSiraHandler() {
            try
            {
                DataTable dtResultado = new DataTable();
                String pFechaInicio     = this.dateInicio.Value.ToShortDateString();
                String pFechaFin        = this.dateFinal.Value.ToShortDateString();
                Int32  pEstado           = Int32.Parse(this.ddlTipoServicio.SelectedValue.ToString());

                dtResultado = dtSiraHandler.ConsultaSiraNotificador(pFechaInicio, pFechaFin, pEstado);

                this.gridDatos.DataSource = dtResultado;
            }
            catch (Exception ex) {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar(" ConsultaSiraHandler ", ex.Message);
                #endregion                
            }

        }

        private void frmNotificaciones_Load(object sender, EventArgs e)
        {
            
            this.dateInicio.Value = DateTime.Now.AddDays(-1);
            this.dateFinal.Value = DateTime.Now;

            llenaEstadoPeticion();
            ConsultaSiraHandler();
        }

        private void gridDatos_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Int32 pId;
            
            try
            {
                pId = Convert.ToInt32(this.gridDatos.Rows[e.RowIndex].Cells[0].Value.ToString());
                ConsultaSiraHandlerxId(pId);
            }
            catch (Exception ex) {
                MessageBox.Show("Error al cargar el evento","..::: SIRA :::...",MessageBoxButtons.OK,MessageBoxIcon.Error);
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar( "gridDatos_CellMouseDoubleClick", ex.Message);
                #endregion   
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            ConsultaSiraHandler();
        }
        
        public void llenaEstadoPeticion() {
            DataTable dtEstado = new DataTable();
            dtEstado.Columns.Add("ID");
            dtEstado.Columns.Add("DESCRIPCION");

            dtEstado.Rows.Add("-1", "Todas");
            dtEstado.Rows.Add("1", "Correctas");
            dtEstado.Rows.Add("0", "Inocrrectas");

            this.ddlTipoServicio.DataSource     = dtEstado;
            this.ddlTipoServicio.DisplayMember  = "DESCRIPCION";
            this.ddlTipoServicio.ValueMember    = "ID";            
        }

        private void ddlTipoServicio_MouseClick(object sender, MouseEventArgs e)
        {
            ConsultaSiraHandler();
        }

        public void ConsultaSiraHandlerxId(Int32 pIdEvento) {

            String pEvento          = string.Empty;
            String pDescripcion     = string.Empty;
            String pXmlRequest      = string.Empty;
            String pXmlResponse     = string.Empty;
            String pFecha           = string.Empty;
            String pEstado          = string.Empty;
            Int32 pIdEstado         = 0;

            try
            {
                DataTable dtResultado = new DataTable();

                dtResultado = dtSiraHandler.ConsultaSiraNotificadorXId(pIdEvento);

                pEvento         = dtResultado.Rows[0]["SINO_EVENTO"].ToString();
                pDescripcion    = dtResultado.Rows[0]["SINO_DESCRIPCION"].ToString();
                pXmlRequest     = dtResultado.Rows[0]["SINO_XML_REQUEST"].ToString();
                pXmlResponse    = dtResultado.Rows[0]["SINO_XML_RESPONSE"].ToString();
                pFecha          = dtResultado.Rows[0]["SINO_FECHA"].ToString();
                pEstado         = dtResultado.Rows[0]["SINO_STATUS"].ToString();
                pIdEstado       = Int32.Parse(dtResultado.Rows[0]["SINO_STATUS_ID"].ToString());

                frmDetalleNotificacion ventanaDetalle = new frmDetalleNotificacion(pIdEvento.ToString(),pEvento, pDescripcion, pXmlRequest, pXmlResponse, pFecha, pEstado, pIdEstado);
                ventanaDetalle.ShowDialog(this);
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("ConsultaSiraHandlerxId", ex.Message);
                #endregion 
            }
        }

        private void gridDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
