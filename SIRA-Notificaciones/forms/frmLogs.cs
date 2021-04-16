using System;
using System.Windows.Forms;

namespace SIRA_Notificaciones
{
    public partial class frmLogs : Form
    {
        public frmLogs()
        {
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void Init() {
            //siraHandlerDAL SiraHandlerDAL = new siraHandlerDAL();
            //DataTable dt = SiraHandlerDAL.Select_handler(DateTime.Now.AddDays (-1).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            //if (dt != null) {
            //    if (dt.Rows.Count > 0) {
            //        gvBitacora.DataSource = dt;
            //    }
            //}
        }

        private void frmLogs_Load(object sender, EventArgs e)
        {
            Init();
        }
    }
}
