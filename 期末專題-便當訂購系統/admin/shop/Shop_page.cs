using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 期末專題_便當訂購系統
{
    public partial class Shop_page : Form
    {
        public Shop_page()
        {
            InitializeComponent();
        }

        private void Shop_page_Load(object sender, EventArgs e)
        {

        }

        private void btn_page_out_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_class_page_go_Click(object sender, EventArgs e)
        {
            Class_page
           Class_page
          = new Class_page();

            Class_page.ShowDialog();
        }

        
    }
}
