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
    public partial class Class_page : Form
    {
        public Class_page()
        {
            InitializeComponent();
        }

        private void Class_page_Load(object sender, EventArgs e)
        {

        }

        private void btn_page_out_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_shop_page_go_Click(object sender, EventArgs e)
        {
            Shop_page
           Shop_page
          = new Shop_page();

            Shop_page.ShowDialog();
        }

        
    }
}
