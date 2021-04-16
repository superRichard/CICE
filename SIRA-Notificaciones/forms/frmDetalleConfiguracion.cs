using SIRA_Notificaciones.dal;
using SIRA_Notificaciones.el;
using System;
using System.Data;
using System.Windows.Forms;

namespace SIRA_Notificaciones.forms
{
    public partial class frmDetalleConfiguracion : Form
    {
        siraConfiguracionesDAL siraConfiguracion = new siraConfiguracionesDAL();
        public delegate void DelegadoConfiguracion();
        public event DelegadoConfiguracion MiEventoConfiguracion;

        public Int32 error = 0;

        public Int32 notificador = 0;
        public Int32 sistema = 1;
        public Int32 servicioWeb = 2;
        public Int32 temporizador = 3;


        public frmDetalleConfiguracion()
        {
            InitializeComponent();            
        }

        #region eventos
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDetalleConfiguracion_Load(object sender, EventArgs e)
        {
            //cargaTipoServicios();
            // nuevo();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidaCampos())
            {
                if (String.IsNullOrEmpty(this.txtId.Text))
                {
                    Guardar();
                }
                else
                {
                    Actualiza();
                }
            }
            else
            {
                Mensaje(error, "No puede continuar, llene todos los campos...!!!");
            }

        }

        private void txtTemporizador_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void ddlTipoServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivarCampo();
        }

        private void btnSubir_Click(object sender, EventArgs e)
        {
            this.txtTemporizador.Text = (Int32.Parse(this.txtTemporizador.Text) + 1).ToString();
        }

        private void btnBajar_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(this.txtTemporizador.Text) > 1)
            {
                this.txtTemporizador.Text = (Int32.Parse(this.txtTemporizador.Text) - 1).ToString();
            }
        }

        #endregion;

        #region metodos

        public void InicializaValores(String pId, String pEtiqueta, Int32 pTipo, String pValor, String pDecValor, String pFecha)
        {
            CargaTipoServicios();
            ActivarCampo();


            this.txtId.Text = pId;
            this.txtEtiqueta.Text = pEtiqueta;
            this.ddlTipoServicio.SelectedValue = pTipo;
            this.txtFecha.Text = pFecha;

            if (pTipo == sistema)
            {
                this.txtValor.Text = pValor;
            }
            else if (pTipo == servicioWeb || pTipo == notificador)
            {
                if (pValor.Equals("1") || pValor.ToUpper().Equals("ACTIVO"))
                    rdActivo.Checked = true;
                else
                    rdInactivo.Checked = true;
            }
            else
            {
                this.txtTemporizador.Text = pValor;                
            }
        }

        public void CargaTipoServicios()
        {
            DataTable dtTipoServicio = new DataTable();

            dtTipoServicio = siraConfiguracion.ConsultaSiraServicios();

            this.ddlTipoServicio.DataSource = dtTipoServicio;
            this.ddlTipoServicio.DisplayMember = "SITS_DESCRIPCION";
            this.ddlTipoServicio.ValueMember = "SITS_ID";

        }

        public void nuevo()
        {
            LimpiarCampos();

            this.txtId.Text = "0";
            this.txtEtiqueta.Select();
        }

        public void LimpiarCampos()
        {
            this.txtId.Text = "";
            this.txtEtiqueta.Text = "";
            this.txtValor.Text = "";
        }

        public void Guardar()
        {
            DataTable dtRespuesta = new DataTable();
            Int32 vTransaccion;
            String vMensaje;

            try
            {
                String vEtiqueta = this.txtEtiqueta.Text;
                Int32 vTipoServicio = Int32.Parse(this.ddlTipoServicio.SelectedValue.ToString());
                String vValor;

                if (vTipoServicio == sistema)
                    vValor = this.txtValor.Text;
                else if (vTipoServicio == servicioWeb || vTipoServicio == notificador)
                    if (rdActivo.Checked)
                        vValor = "1";
                    else
                        vValor = "0";
                else
                    vValor = this.txtTemporizador.Text;


                dtRespuesta = siraConfiguracion.InsertaSiraConfiguraciones (vEtiqueta, vTipoServicio, vValor);

                vTransaccion = Int32.Parse(dtRespuesta.Rows[0]["transaccion"].ToString());
                vMensaje = dtRespuesta.Rows[0]["mensaje"].ToString();

                Mensaje(vTransaccion, vMensaje);

                this.MiEventoConfiguracion();
                this.Close();
            }
            catch (Exception ex)            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("Guardar configuraciones", ex.Message);
                #endregion
                //throw new ApplicationException(ex.Message);
            }
        }

        public void Actualiza()
        {
            DataTable dtRespuesta = new DataTable();
            Int32 vTransaccion;
            String vMensaje;

            try
            {
                Int32 vId = Int32.Parse(this.txtId.Text);
                String vEtiqueta = this.txtEtiqueta.Text;
                Int32 vTipoServicio = Int32.Parse(this.ddlTipoServicio.SelectedValue.ToString());
                String vValor;

                if (vTipoServicio == sistema)
                    vValor = this.txtValor.Text;
                else if (vTipoServicio == servicioWeb || vTipoServicio == notificador)
                    if (rdActivo.Checked)
                        vValor = "1";
                    else
                        vValor = "0";
                else
                    vValor = this.txtTemporizador.Text;


                dtRespuesta = siraConfiguracion.ActualizaSiraConfiguraciones(vId, vEtiqueta, vTipoServicio, vValor);

                vTransaccion = Int32.Parse(dtRespuesta.Rows[0]["transaccion"].ToString());
                vMensaje = dtRespuesta.Rows[0]["mensaje"].ToString();

                Mensaje(vTransaccion, vMensaje);

                this.MiEventoConfiguracion();

                this.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public void Mensaje(Int32 pTransaccion, String pMensaje)
        {
            if (pTransaccion == 0)
            {
                MessageBox.Show(pMensaje, "...::: SIRA :::...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(pMensaje, "...::: SIRA :::...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void ActivarCampo()
        {

            try
            {
                Int32 pTipo = Int32.Parse(ddlTipoServicio.SelectedValue.ToString());

                if (pTipo == sistema)
                {
                    this.rdActivo.Visible = false;
                    this.rdInactivo.Visible = false;
                    this.txtValor.Visible = true;
                    this.lblValor.Visible = true;
                    this.txtTemporizador.Visible = false;
                    this.btnSubir.Visible = false;
                    this.btnBajar.Visible = false;
                    this.lblTemporizador.Visible = false;
                }
                else if (pTipo == servicioWeb || pTipo == notificador)
                {
                    this.rdActivo.Visible = true;
                    this.rdInactivo.Visible = true;
                    this.txtValor.Visible = false;
                    this.lblValor.Visible = false;
                    this.txtTemporizador.Visible = false;

                    this.rdActivo.Checked = true;
                    this.btnSubir.Visible = false;
                    this.btnBajar.Visible = false;
                    this.lblTemporizador.Visible = false;
                }
                else
                {
                    this.rdActivo.Visible = false;
                    this.rdInactivo.Visible = false;
                    this.txtValor.Visible = false;
                    this.lblValor.Visible = true;

                    this.txtTemporizador.Visible = true;
                    this.btnSubir.Visible = true;
                    this.btnBajar.Visible = true;
                    this.lblTemporizador.Visible = true;

                    //this.txtEtiqueta.Select();
                }
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("ActivarCampo configuraciones", ex.Message);
                #endregion
            }
        }

        public Boolean ValidaCampos()
        {
            Boolean vRespuesta = true;
            try
            {
                Int32 vTipoServicio = Int32.Parse(this.ddlTipoServicio.SelectedValue.ToString());

                if (vTipoServicio == sistema)
                {
                    if (String.IsNullOrEmpty(this.txtValor.Text))
                    {
                        vRespuesta = false;
                    }
                }
                else if (vTipoServicio == servicioWeb || vTipoServicio == notificador)
                {
                    if (rdActivo.Checked == false && rdInactivo.Checked == false)
                    {
                        vRespuesta = false;
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(this.txtTemporizador.Text))
                    {
                        vRespuesta = false;
                    }
                }

            }
            catch (Exception ex)
            {
                vRespuesta = false;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("ValidaCampos configuraciones", ex.Message);
                #endregion
            }

            return vRespuesta;
        }


        #endregion;


    }
}
