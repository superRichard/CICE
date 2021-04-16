using System;
using System.Windows.Forms;

namespace SIRA_Notificaciones.forms
{
    public partial class frmDetalleNotificacion : Form
    {
        public frmDetalleNotificacion(String pId, String pEvento, String pDescripcion, String pXmlRequest, String pXmlResponse, String pFecha, String pEstado, Int32 pIdEstado)
        {
            InitializeComponent();

            

            this.txtId.Text = pId;
            this.txtEvento.Text = pEvento;
            this.txtDescripcion.Text = pDescripcion;
            this.txtFecha.Text = pFecha;
            this.txtXmlRequest.Text = System.Xml.Linq.XDocument.Parse(pXmlRequest).ToString();
            this.txtXmalResponse.Text = System.Xml.Linq.XDocument.Parse(pXmlResponse).ToString();
            this.lblEstado.Text = pEstado;

            if (pIdEstado > 0)
            {
                lblEstado.BackColor = System.Drawing.Color.Green;
            }
            else {
                lblEstado.BackColor = System.Drawing.Color.Red;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDetalleNotificacion_Load(object sender, EventArgs e)
        {
            this.btnSalir.Select();
        }
    }
}
