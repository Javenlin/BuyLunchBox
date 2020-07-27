using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace 期末專題_便當訂購系統
{
    public partial class Admin_page : Form
    {
        SqlConnectionStringBuilder scsb; //資料庫連線字串

        //宣告Login_page導入變數
        public string str_user_login_confirm = "";  /*使用者登入提示字串*/
        // public string str_Admin_page_Today_shop=""; /*本日供餐店家*/
       

        public Admin_page()
        {
            InitializeComponent();
        }

        private void Admin_page_Load(object sender, EventArgs e)
        {
            MessageBox.Show(str_user_login_confirm);
           

            //建立資料庫連線字串物件scsb
            scsb = new SqlConnectionStringBuilder();//儲存SQL連線資訊
            scsb.DataSource = @"."; //伺服器名稱
            scsb.InitialCatalog = "lunch";//資料庫名稱
            scsb.IntegratedSecurity = true; //集成驗證

            user_page user_page = new user_page();
            user_page.str_Admin_page_Today_shop = cbox_today_shop.Text;//存入公用登入姓名;
        }

        //跳至店家
        private void btn_shop_go_Click(object sender, EventArgs e)
        {
            Shop_page
              Shop_page
             = new Shop_page();

            Shop_page.ShowDialog();
        }

        //跳至班級
        private void btn_class_go_Click(object sender, EventArgs e)
        {
            Class_page
                Class_page
               = new Class_page();

            Class_page.ShowDialog();
        }

        //跳至訂購單生成
        private void btn_order_go_Click(object sender, EventArgs e)
        {
            Orders_page
                Orders_page
               = new Orders_page();

            Orders_page.ShowDialog();
        }

        private void btn_new_shop_Click(object sender, EventArgs e)
        {
            

        }

        private void btn_new_class_Click(object sender, EventArgs e)
        {
           
        }

           private void btn_page_out_Click(object sender, EventArgs e)
        {
            Close();
            user_page user_page = new user_page();
            user_page.str_Admin_page_Today_shop = cbox_today_shop.Text;//存入公用本日供餐店家;
        }

     
        private void cbox_today_shop_SelectedIndexChanged(object sender, EventArgs e)
        {
            user_page user_page = new user_page();
            user_page.str_Admin_page_Today_shop = cbox_today_shop.SelectedItem.ToString();//存入公用本日供餐店家已變選項
        }
    }
}
