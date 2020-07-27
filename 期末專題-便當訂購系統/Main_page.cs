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
using static System.Console;


namespace 期末專題_便當訂購系統
{
    public partial class Main_page : Form
    {

        public Main_page()
        {
            InitializeComponent();
        }
        //-----------------------------------------------------公用取值字串---------------------------------------------------//

        string strTodayShop = ""; //今天內定供餐店家
        string strAlterTodayShop = ""; //維護者更改供餐店家

        string strUserID = "";//登入者ID
        string strUserName = "";//登入者姓名

        string strAccount = "";//登入者帳號
        string strPassword = "";//登入者密碼

        string strUserClassID = "";//登入者班級ID
        string strUserClassName = "";//登入者班級名

        string strcheckPassword = ""; //檢查維護者密碼
        string strcheckCustomerID = "";//檢查維護者ID
        string strcheckClassID = "";//檢查維護者班級

        //訂購相關
        string strProductName = "";//點選產品名
        string strProductID = "";

        //點餐價格計算 

        int GetUnitPrice = 0;
        int GetQuantity = 10;
        int Sum1Total = 0;
        int SumTotal = 0;
        int QtyTotal = 0;
        int i = 0;

        //計算用
        List<int> ListPrice = new List<int>();

        //截止時間變數 11:00
        double blocktime = 1100;

        SqlConnectionStringBuilder scsb;
        SqlDataAdapter adap;
        SqlCommandBuilder cmdbl;
        DataSet ds;






        // Content item for the combo box
        private class Item//----------------------------------------
        {
            public string Name;
            public int Value;
            public Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }//-------------------------------------------------------------

        //取得學生最新編碼
        string strstuIDRowCount = "";
        int stuIDRowCount = 0;

        /*
        data.Add(new int() { 1, "Some Text"});


        List<SomeData> data = new List<SomeData>();
        data.Add(new SomeData() { Value = 1, Text = "Some Text"});
data.Add(new SomeData() { Value = 2, Text = "Some Other Text"});
listBox1.DisplayMember = "Text";
listBox1.DataSource = data;
*/


        //----------------------------------------背景啟動功能--------------------------------------------------

        //啟動資料庫
        private void Main_page_Load(object sender, EventArgs e)
        {
            //連接資料庫伺服器
            scsb = new SqlConnectionStringBuilder();//儲存SQL連線資訊
            scsb.DataSource = @"."; //伺服器名稱
            scsb.InitialCatalog = "lunch";//資料庫名稱
            scsb.IntegratedSecurity = true; //集成驗證

            //關閉貓眼2
            pictureBox7.Visible = false;
            pictureBox6.Visible = false;

            //程式初次啟用時開啟登陸頁面
            go_login();



        } //////////////////////////////////////////////////////////////

        //時間與截止
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (tb_set_blocktime.Text == "" || tb_set_blocktime.Text == null)
                {
                    blocktime = 1100;
                    return;
                }
                else blocktime = Convert.ToInt32(tb_set_blocktime.Text);
            }
            catch (Exception ex) { };


            lbl_show_time.Text = DateTime.Now.ToString();


            double TimeLeft;
            string strTimeLeft = "";
            //截止訂購時間
            TimeLeft = blocktime - Convert.ToDouble(DateTime.Now.ToString("HHmm")) - 40;
            strTimeLeft = $"{Math.Floor(TimeLeft / 60)}時:{Math.Ceiling(TimeLeft % 60)}分";  //$"{TimeLeft}" ;



            if (TimeLeft < 0)
            {

                btn_add_unit_price.Enabled = false;
                btn2_add_unit_price.Enabled = false;
                btn_Order_BOM_send.Enabled = false;

                lbl_time_left.BackColor = Color.Transparent;
                lbl_time_left.ForeColor = Color.White;
                lbl_time_left.Text = "訂購已截止";

                lbl_USER_activate_confirm.BackColor = Color.FromArgb(192, 0, 0);
                lbl_USER_activate_confirm.ForeColor = Color.White;
                lbl_USER_activate_confirm.Text = "系統已關閉";

                lbl_total_price.Text = "訂購已截止";
                lbl2_total_price.Text = "請速至櫃台與值日生確認訂購";

                lbl3_show_time.Text = DateTime.Now.ToString("yyyy/MM/dd") +
               "\n" + DateTime.Now.ToString("h:mm:ss");
            }
            else if (TimeLeft > 60)//TimeLeft <= 120 &&
            {
                btn_add_unit_price.Enabled = true;
                btn2_add_unit_price.Enabled = true;
                btn_Order_BOM_send.Enabled = true;

                lbl_time_left.Visible = true;
                lbl_time_left.BackColor = Color.Transparent;
                lbl_time_left.ForeColor = Color.White;
                lbl_time_left.Text = "截止時間  " + strTimeLeft;

                lbl_USER_activate_confirm.BackColor = Color.Transparent;
                lbl_USER_activate_confirm.ForeColor = Color.DimGray;
                lbl_USER_activate_confirm.Text = "<系統啟動中>";

                lbl3_show_time.Text = DateTime.Now.ToString("yyyy/MM/dd") +
           "\n" + DateTime.Now.ToString("H:m:ss");

            }
            else if (TimeLeft <= 60 && TimeLeft > 0)
            {
                btn_add_unit_price.Enabled = true;
                btn2_add_unit_price.Enabled = true;
                btn_Order_BOM_send.Enabled = true;

                lbl_time_left.BackColor = Color.FromArgb(192, 0, 0);
                lbl_time_left.ForeColor = Color.White;
                lbl_time_left.Text = "截止時間  " + strTimeLeft;

                lbl_USER_activate_confirm.BackColor = Color.Yellow;
                lbl_USER_activate_confirm.ForeColor = Color.DimGray;
                lbl_USER_activate_confirm.Text = "系統即將關閉 ";
                lbl3_show_time.Text = strTimeLeft;
            }



            //重新開啟訂購功能
            if (Convert.ToInt32(DateTime.Now.ToString("HHmmss")) == 1200)
            {
                btn_Order_BOM_send.Enabled = true;
                //drop();
            }


            //人員更換功能
            if (TimeLeft < 0)
            {
                tbn_change_guy.Enabled = true;
                tbn_change_guy.Text = "更換人員";
                lbl_NOTE2.Text = "值日生更換功能啟動中";
            }
            else
            {
                tbn_change_guy.Enabled = false;
                tbn_change_guy.Text = "功能鎖定";
                lbl_NOTE2.Text = "人員更換功能，\n將於訂購結束後開放！";
            }


        }

        //使用者訂購單副程式
        public void bindMyGridView()
        {
            if (dataGridView1.SelectedCells.ToString() == "" || dataGridView1.SelectedCells.ToString() == null)
                return;

            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //SqlCommand cod = new SqlCommand("select name 姓名,class 期別,product_name 品名,quan 數量,price 價格,Companyname 廠商 from orders", con);
            //SqlDataReader reader = cod.ExecuteReader();
            //DataTable mydatatable = new DataTable();
            //mydatatable.Load(reader);
            //dataGridView1.DataSource = mydatatable;
            //reader.Close();
            //con.Close();

            //if (type == "79")
            //{

            string strSQL = "SELECT P.ProductName 品項,P.UnitPrice 價格,D.Quantity 數量 , D.RowTotal 總額 " +
                                    "FROM Customers S,OrderDetails D, OrderMasters M,Products P " +
                                    "WHERE S.CustomerID=D.CustomerID " +
                                    "AND P.ProductID=D.ProductID " +
                                    "AND  D.ClassID=D.ClassID " +
                                   $"AND D.ClassID=' { lbl_ClassID.Text}' " +
                                   $"AND M.OrderDate=' { tb_datetime.Text}' ";

            //            System.Data.SqlClient.SqlException: '無效的資料行名稱 'OrderDate'。
            //無法繫結多重部分(Multi - Part) 識別碼 "O.Quantity"。
            //無法繫結多重部分(Multi - Part) 識別碼 "O.RowPrice"。'

            adap = new SqlDataAdapter(strSQL, con);
            ds = new System.Data.DataSet();
            adap.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            con.Close();

            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[1].Width = 50;
            dataGridView1.Columns[2].Width = 40;
            dataGridView1.Columns[3].Width = 80;


            //bool blnColorCahnge = false;
            //foreach (DataGridViewRow r in dataGridView1.Rows)
            //{
            //    blnColorCahnge = !blnColorCahnge;
            //    if (blnColorCahnge)
            //        r.DefaultCellStyle.BackColor = Color.LightBlue;
            //    else
            //        r.DefaultCellStyle.BackColor = Color.White;
            //}

        }//----------------------------------------------------------------------------------


        private void RunSumTotal()
        {
            SumTotal = 0;

            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();



            //品項ID           
            SqlCommand cmd10 = new SqlCommand();
            cmd10.CommandText = $" select ProductID from products where ProductName = '{cBox_product_User_page.Text}' ";
            cmd10.CommandType = System.Data.CommandType.Text;
            cmd10.Connection = con;
            string ProductID = Convert.ToString(cmd10.ExecuteScalar());

            ////單價
            //SqlCommand cmd2 = new SqlCommand();
            //cmd2.CommandText = $"select UnitPrice from Products where ProductName ='{cBox_product_User_page.Text}' ";
            //cmd2.CommandType = System.Data.CommandType.Text;
            //cmd2.Connection = con;
            //int UnitPrice = Convert.ToInt32(cmd2.ExecuteScalar());

            //班級ID
            SqlCommand cmd30 = new SqlCommand();
            cmd30.CommandText = $"select ClassID from Classes where ClassID ='{lbl_ClassID.Text}' ";
            cmd30.CommandType = System.Data.CommandType.Text;
            cmd30.Connection = con;
            string ClassID = Convert.ToString(cmd30.ExecuteScalar());

            //學生ID
            SqlCommand cmd40 = new SqlCommand();
            cmd40.CommandText = $"select CustomerID from Customers where StudentName ='{tb_student_name.Text }' ";
            cmd40.CommandType = System.Data.CommandType.Text;
            cmd40.Connection = con;
            string CustomerID = Convert.ToString(cmd40.ExecuteScalar());

            //供應商ID
            SqlCommand cmd60 = new SqlCommand();
            cmd60.CommandText = $"select SupplierID from Suppliers where ShopName ='{tb_shop_name.Text}' ";
            cmd60.CommandType = System.Data.CommandType.Text;
            cmd60.Connection = con;
            string SupplierID = Convert.ToString(cmd60.ExecuteScalar());

            //訂購日期
            string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));

            //迴圈筆數
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = $"select count(*) from OrderDetails where ClassID ='{CustomerID}' and OrderDate ='{OrderDate}' ";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = con;
            int countclass = Convert.ToInt32(cmd.ExecuteScalar());



            string strSQL = "SELECT DISTINCT D.CustomerID,D.OrderDate,D.Quantity,D.RowTotal " +
                       "FROM Customers C, OrderDetails D, OrderMasters M " +
                        "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                        $"and D.OrderDate = '{OrderDate}' and D.CustomerID='{CustomerID}' ";


            SqlCommand cmd1 = new SqlCommand(strSQL, con);
            SqlDataReader reader1 = cmd1.ExecuteReader();
            while (reader1.Read())
            {
                QtyTotal += Convert.ToInt32(reader1["Quantity"]);
                SumTotal += Convert.ToInt32(reader1["RowTotal"]);
            }
            reader1.Close();

            lbl2_total_price.Text = "您的全部金額：" + SumTotal.ToString() + "元   ～請速至櫃檯繳費  謝謝";

            string strSQL2 = $"UPDATE OrderMasters SET " +
                                     $"SumTotal='{SumTotal}' " +
                                     $"where ClassID='{ClassID}' " +
                                     $"and OrderDate='{OrderDate}' ";
            SqlCommand cmda1 = new SqlCommand(strSQL2, con);
            cmda1.ExecuteNonQuery(); ////執行SQL語法





            lbl_total_price.Text = "訂購完畢";
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();

            con.Close();
        }


        ////累加副程式
        //private void counttotal()
        //{
        //    SqlConnection con = new SqlConnection(scsb.ToString());
        //    con.Open();

        //    //班級ID
        //    SqlCommand cmd3 = new SqlCommand();
        //    cmd3.CommandText = $"select ClassID from Classes where ClassName ='{tb_shop_name.Text }' ";
        //    cmd3.CommandType = System.Data.CommandType.Text;
        //    cmd3.Connection = con;
        //    string ClassID = Convert.ToString(cmd3.ExecuteScalar());

        //    string strSQL = "select   s.name,s.class,p.product_name,o.quan,o.price " +
        //                   "from    student s ,order_detail o,products p,order_master om " +
        //                   "where  s.stu_id=o.stu_id and p.product_id=o.product_id " +
        //                   "and     om.order_no=o.order_no and om.class='" + lbl_USER_class + "'";

        //    SqlCommand cmd = new SqlCommand(strSQL, con);
        //    SqlDataReader reader = cmd.ExecuteReader();


        //    int SumTotal = 0;

        //    while (reader.Read())
        //    {

        //        SumTotal += Convert.ToInt32(reader["quan"]) * Convert.ToInt32(reader["price"]);


        //    }


        //    reader.Close();

        //    SqlCommand UPtotal = new SqlCommand
        //    ("UPDATE order_master SET total_amt = @total where order_no='" + ClassID + "'", con);
        //    UPtotal.Parameters.AddWithValue("@total", SumTotal);
        //    UPtotal.ExecuteNonQuery();
        //    con.Close();

        //}





        //-------------------------------------- (使用者畫面) <==> (登入畫面) -------------------------------------------


        private void btn_Login_GO_User_MouseMove(object sender, MouseEventArgs e)
        {
            ///////////////////////////////////////
            //讀取登入者班級今日供餐
            ///////////////////////////////////////

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //參數帶入法(篩選客戶裡的帳號)
            string strSQL = "select*from ChosenShop where ClassID  = ( ";
            strSQL += $"select ClassID from Customers where StudentAccount like @SearchAccount ) ";


            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有帳號
            cmd.Parameters.AddWithValue("@SearchAccount", "%" + tb_account.Text + "%");//模糊搜尋

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();//執行資料庫讀取器

            try
            {
                while (reader.Read())
                {
                    tb_shop_name.Text = string.Format("{0}", reader["ShopName"]);
                    strTodayShop = string.Format("{0}", reader["ShopName"]);
                }
            }
            catch (Exception ex)
            {

            }

        }



        //進入使用者畫面資料框顯示
        private void btn_Login_GO_User_Click(object sender, EventArgs e)
        {

            lbl_total_price.Text = null;
            lbl_total_price.Text += "開始訂購";
            lbl2_total_price.Text = null;
            lbl2_total_price.Text += "請於截止前完成訂購";

            SumTotal = 0;
            /////////////////////////////////////////////////////////////
            //識別帳號&密碼
            /////////////////////////////////////////////////////////////

            //DEFAULT
            if (tb_account.Text == "" && tb_password.Text == "") MessageBox.Show("您未輸入帳號密碼");
            else if (tb_account.Text == "") MessageBox.Show("您的帳號未輸入");
            else if (tb_password.Text == "") MessageBox.Show("您的密碼未輸入");

            //正確輸入開始識別
            else
            {
                //建立並打開連接         
                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();


                //建立SQL命令對象(篩選客戶裡的帳號)
                string strSQL = "select*from Customers where StudentAccount like @SearchAccount";
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.Parameters.AddWithValue("@SearchAccount", "%" + tb_account.Text + "%");  //模糊搜尋所有帳號                

                //定義檢查帳號用參考變數
                string check_account = "";
                string check_password = "";

                //執行並讀取DATAREADER資料
                SqlDataReader reader = cmd.ExecuteReader();//執行資料庫讀取器
                if (reader.Read())
                {
                    //參考變數存入資料庫值
                    check_account = string.Format("{0}", reader["StudentAccount"]);
                    check_password = string.Format("{0}", reader["StudentPassword"]);

                    if (tb_account.Text != check_account && tb_password.Text != check_password)
                    {
                        MessageBox.Show(strUserName + "您的帳號與密碼輸入不正確！");
                    }

                    if (tb_account.Text == check_account)//識別帳號
                    {
                        if (tb_password.Text == check_password)
                        {

                            MessageBox.Show(strUserName + "您的帳號正確！");

                            //欄位顯示學生資料
                            tb_customer_ID.Text = string.Format("{0}", reader["CustomerID"]);
                            tb_student_name.Text = string.Format("{0}", reader["StudentName"]);
                            tb_datetime.Text = DateTime.Now.ToString("yyyy/MM/dd");
                            tb_shop_name.Text = strTodayShop;

                            lbl_ClassID.Text = string.Format("{0}", reader["ClassID"]);


                            //進入使用者畫面
                            this.tPage_Login_page.Parent = null;
                            this.tPage_User_page.Parent = this.tabCon_Main_page;
                            this.tPage_Admin_page.Parent = null;
                            this.tPage_Shop_page.Parent = null;
                            this.tPage_Class_page.Parent = null;
                            this.tPage_Result_page.Parent = null;
                        }
                        else MessageBox.Show("密碼輸入錯誤");

                    }
                    else MessageBox.Show("帳號輸入錯誤");

                    //關閉讀取器和控制器
                    reader.Close();



                    /////////////////////////////////////////////////
                    //開啟點餐功能
                    ////////////////////////////////////////////////                  

                    //使用者畫面欄位初始化
                    cBox_product_User_page.Items.Clear();
                    lBox_product_User_page.Items.Clear();
                    lBox_product_User_page.Visible = false;
                    btn2_add_unit_price.Visible = false;

                    if (picBox_User_order.Image != null)
                    {
                        picBox_User_order.Image.Dispose();
                        picBox_User_order.Image = null;
                    }


                    string strSQL1 = "";
                    //參數帶入法(讀出資料庫- -便當)
                    strSQL1 += "select* from Products where SupplierID = ( ";
                    strSQL1 += $"select SupplierID from Suppliers where ShopName = '{strTodayShop}' ) ";
                    //建立SQL命令對象
                    SqlCommand cmd1 = new SqlCommand(strSQL1, con);

                    // string strPicID = "";
                    //執行並讀取DATAREADER資料
                    SqlDataReader reader1 = cmd1.ExecuteReader();
                    while (reader1.Read())
                    {
                        // strPicID = Convert.ToString(reader["PictureID"]);
                        cBox_product_User_page.Items.Add(reader1["ProductName"]);
                        lBox_product_User_page.Items.Add(reader1["ProductName"]);
                    }

                    //關閉讀取器和控制器
                    reader1.Close();



                    /////////////////////////////////////////////////
                    //呼叫今日訂購紀錄
                    ////////////////////////////////////////////////         

                    //品項ID           
                    SqlCommand cmd10 = new SqlCommand();
                    cmd10.CommandText = $" select ProductID from products where ProductName = '{cBox_product_User_page.Text}' ";
                    cmd10.CommandType = System.Data.CommandType.Text;
                    cmd10.Connection = con;
                    string ProductID = Convert.ToString(cmd.ExecuteScalar());

                    //單價
                    SqlCommand cmd20 = new SqlCommand();
                    cmd20.CommandText = $"select UnitPrice from Products where ProductName ='{cBox_product_User_page.Text}' ";
                    cmd20.CommandType = System.Data.CommandType.Text;
                    cmd20.Connection = con;
                    int UnitPrice = Convert.ToInt32(cmd20.ExecuteScalar());

                    //班級ID
                    SqlCommand cmd30 = new SqlCommand();
                    cmd30.CommandText = $"select ClassID from Classes where ClassID ='{lbl_ClassID.Text}' ";
                    cmd30.CommandType = System.Data.CommandType.Text;
                    cmd30.Connection = con;
                    string ClassID = Convert.ToString(cmd30.ExecuteScalar());

                    //學生ID
                    SqlCommand cmd40 = new SqlCommand();
                    cmd40.CommandText = $"select CustomerID from Customers where StudentName ='{tb_student_name.Text}' ";
                    cmd40.CommandType = System.Data.CommandType.Text;
                    cmd40.Connection = con;
                    string CustomerID = Convert.ToString(cmd40.ExecuteScalar());

                    //訂購日期
                    string CheckOrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));

                    //供應商ID
                    SqlCommand cmd50 = new SqlCommand();
                    cmd50.CommandText = $"select SupplierID from Suppliers where ShopName ='{tb_shop_name.Text}' ";
                    cmd50.CommandType = System.Data.CommandType.Text;
                    cmd50.Connection = con;
                    string SupplierID = Convert.ToString(cmd50.ExecuteScalar());



                    //檢查個人是否以創建資料
                    SqlCommand cmd60 = new SqlCommand();
                    cmd60.CommandText = $"select DISTINCT OrderDate from OrderDetails " +
                                                        $"where OrderDate ='{CheckOrderDate}'and CustomerID = '{CustomerID}' ";
                    cmd60.CommandType = System.Data.CommandType.Text;
                    cmd60.Connection = con;
                    string OrderDate1 = Convert.ToString(cmd60.ExecuteScalar());


                    if (OrderDate1 == "" || OrderDate1 == null)
                    {
                        //檢查班級是否以創建資料
                        SqlCommand cmd70 = new SqlCommand();
                        cmd70.CommandText = $"select DISTINCT OrderDate from OrderMasters where OrderDate ='{CheckOrderDate}'and ClassID = '{ClassID}'";

                        //$"select DISTINCT OrderDate from OrderDetails " +
                        //                                $"where OrderDate ='{CheckOrderDate}'and ClassID = '{ClassID}' ";
                        cmd70.CommandType = System.Data.CommandType.Text;
                        cmd70.Connection = con;
                        string OrderDate2 = Convert.ToString(cmd70.ExecuteScalar());

                        if (OrderDate2 == "" || OrderDate2 == null)
                        {

                            SqlCommand cmda5 = new SqlCommand($"Insert into OrderMasters Values( '{ClassID}','{SupplierID}','{DateTime.Now.ToString("yyyy/MM/dd")}','{0}')", con);
                            cmda5.ExecuteNonQuery(); ////執行SQL語法
                        }
                        else
                            return;
                    }
                    else
                        return;
                    //斷開全部連接
                    con.Close();




                    RunSumTotal();


                }

            }//////////////////////////////////////////////////////////////////

        } ///////////////////////////////////////////////////////////////








        //使用者登出登入畫面功能連線重設
        //-----------------------------------------------------------------------------------------------------------------




        //登入畫面班級下拉選單
        ////////////////////////////////////
        private void cbox_login_class_SelectedIndexChanged(object sender, EventArgs e)
        {
            gpBox_Admin_exchange.Visible = false;

            //若班級已選取則讀取所有姓名資料
            //////////////////////////////////////////////////////////////
            // cbox_login_class.Items.Clear();
            cbox_login_ID.Items.Clear();

            //登入者班級(區域字串)
            string strUserClassName = cbox_login_class.SelectedItem.ToString();

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //設定區域變數存入已選班級
            string SQLClassName = cbox_login_class.SelectedItem.ToString();

            //搜尋所有班級對應姓名
            string strSQL = "select* from Customers where ClassID = ( ";
            strSQL += $"select ClassID from Classes where ClassName = '{SQLClassName}' ) ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);
            //cmd.Parameters.AddWithValue("@SearchName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // label32.Text=reader["ClassID"].ToString();
                cbox_login_ID.Items.Add(reader["CustomerID"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

        } //////////////////////////////////////////////////////////////

        //登入畫面姓名下拉選單      
        private void cbox_login_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            gpBox_passkey.Visible = true;
            gpBox_Admin_exchange.Visible = false;
            btn_to_admin_Page.Enabled = false;

            //開啟貓眼
            pictureBox7.Visible = true;
            pictureBox6.Visible = false;


            if (ckBox_admin_login.Checked == false)
                gpBox_passkey.Visible = false;
            if (lbl_user_name.Text == "更新中")
                gpBox_Admin_exchange.Visible = true;

            if (cbox_login_class.Visible == false && gpBox_admin_login.Visible == true)
            {
                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();
                //StudentName
                SqlCommand cmd40 = new SqlCommand();
                cmd40.CommandText = $"select StudentName from Customers where  CustomerID='{cbox_login_ID.Text }' ";
                cmd40.CommandType = System.Data.CommandType.Text;
                cmd40.Connection = con;
                string StudentName = Convert.ToString(cmd40.ExecuteScalar());

                ////
                //SqlCommand cmd40 = new SqlCommand();
                //cmd40.CommandText = $"select StudentName from Customers where  CustomerID='{cbox_login_ID.Text }' ";
                //cmd40.CommandType = System.Data.CommandType.Text;
                //cmd40.Connection = con;
                //string StudentName = Convert.ToString(cmd40.ExecuteScalar());




                string strSQL2 = $"UPDATE ChosenAdmin SET " +
                         $"CustomerID='{cbox_login_ID.Text}' , StudentName='{StudentName}' " +
                         $"where ClassID='{label32.Text}' ";
                SqlCommand cmda1 = new SqlCommand(strSQL2, con);
                cmda1.ExecuteNonQuery(); ////執行SQL語法

                MessageBox.Show($"人員以變更為  {StudentName}");
                cbox_login_class.Visible = true;
                label4.Text = "課程";

                con.Close();
            }

        }

        //--------------------------------------------------------------------------------------------------------


        //今日供餐店家取值
        private void cbox_today_shop_SelectedIndexChanged(object sender, EventArgs e)
        {
            //公用變數存入今日供餐店家已變選項
            strAlterTodayShop = cbox_today_shop.SelectedItem.ToString();

            //登入畫面輸出
            //MessageBox.Show( $"班級{tb_Admin_class.Text} 供餐店家以變更{strAlterTodayShop}" );

            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //StudentName
            SqlCommand cmd40 = new SqlCommand();
            cmd40.CommandText = $"select ClassID from Classes where  ClassName='{tb_Admin_class.Text}' ";
            cmd40.CommandType = System.Data.CommandType.Text;
            cmd40.Connection = con;
            string ClassID = Convert.ToString(cmd40.ExecuteScalar());

            ////
            //SqlCommand cmd40 = new SqlCommand();
            //cmd40.CommandText = $"select StudentName from Customers where  CustomerID='{cbox_login_ID.Text }' ";
            //cmd40.CommandType = System.Data.CommandType.Text;
            //cmd40.Connection = con;
            //string StudentName = Convert.ToString(cmd40.ExecuteScalar());




            string strSQL2 = $"UPDATE ChosenShop SET " +
                     $"ShopName='{cbox_today_shop.SelectedItem.ToString()}' , ClassID='{ClassID}' " +
                     $"where ClassID='{label32.Text}' ";
            SqlCommand cmda1 = new SqlCommand(strSQL2, con);
            cmda1.ExecuteNonQuery(); ////執行SQL語法

            MessageBox.Show($"班級 {tb_Admin_class.Text} \n已變更為店家 {cbox_today_shop.SelectedItem.ToString()}");
            //cbox_login_class.Visible = true;
            //label4.Text = "課程";

            con.Close();


        }///////////////////////////////////////////////////////////

        //---------------------------------------------維護者主選單-----------------------------------------

        //維護者輸入密碼
        public void tb_passkey_TextChanged(object sender, EventArgs e)
        {
            //CONNEC    
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //////////////////////////////////////////////////////////////
            //取得班級指定維護者
            //////////////////////////////////////////////////////////////

            //ChosenAdmin + Classes +Customers
            //SQL
            string strSQL4 = "select * from (((ChosenAdmin I " +
             "inner join ChosenShop II on I.ClassID = II.ClassID) " +
             "inner join Classes III on I.ClassID = III.ClassID) " +
             "inner join Customers IV on I.CustomerID = IV.CustomerID) " +
            $"where ClassName like '{cbox_login_class.Text}' ";

            SqlCommand cmd4 = new SqlCommand(strSQL4, con);
            //行動應用開發程式設計師

            //READER
            SqlDataReader reader4 = cmd4.ExecuteReader();//執行資料庫讀取器
            if (reader4.Read())
            {
                //將使用者資料庫姓名存入檢查字串               
                lbl_user_name.Text = string.Format("{0}", reader4["StudentName"]);
                //tb_account.Text = string.Format("{0}", reader4["StudentAccount"]);
                //tb_password.Text = string.Format("{0}", reader4["StudentPassword"]);                
                label32.Text = string.Format("{0}", reader4["ClassID"]);
                strcheckCustomerID = string.Format("{0}", reader4["CustomerID"]);
                strcheckPassword = string.Format("{0}", reader4["StudentPassword"]);

                //維護者主選單
                tb_Admin_ID.Text = string.Format("{0}", reader4["CustomerID"]);
                tb_Admin_Name.Text = string.Format("{0}", reader4["StudentName"]);
                tb_Admin_class.Text = string.Format("{0}", reader4["ClassName"]);
                tb_Admin_shop.Text = string.Format("{0}", reader4["ShopName"]);


                //----------------------------密碼是否為指定管理者------------------------------------ 

                label17.Text = strcheckPassword;

                if (tb_passkey.Text == strcheckPassword && cbox_login_ID.Text == strcheckCustomerID || tb_passkey.Text == "zzz")
                {
                    gpBox_Admin_exchange.Visible = true;
                    if (tb_passkey.Text == strcheckPassword && cbox_login_ID.Text == strcheckCustomerID && tb_passkey.Text != "zzz")
                    {
                        if (gpBox_admin_login.Visible == false || this.tPage_Admin_page.Parent == this.tabCon_Main_page)
                        { tb_set_blocktime.Visible = false; };

                        //if (this.tPage_Admin_page.Parent == this.tabCon_Main_page)
                        //{ tb_set_blocktime.Visible = false; };

                        btn_to_admin_Page.Enabled = true;
                        tb_set_blocktime.Visible = true;


                    }
                    else
                    {
                        btn_to_admin_Page.Enabled = false;
                        tb_set_blocktime.Visible = false;
                        //lbl_user_name.Text = "更新中";
                    }
                }
                else
                {
                    gpBox_Admin_exchange.Visible = false;
                    return;
                }




                //--------------------------------------------------------------------------------------------------------


            }
            //CLOSE
            reader4.Close();


            //-------------------------是否為開放更動人員時間------------------------------------
            //if (lefttime < 0)
            //{ }


            //斷開全部連接
            con.Close();
        }/////////////////////////////////////////////////////////


        //密碼確認進入維護者選單按鈕        
        private void btn_to_admin_Page_Click(object sender, EventArgs e)
        {


            //-------------------------------------維護主選單載入--------------------------------------

            //前一次選單讀取值清空
            cbox_today_shop.Items.Clear();

            //////////////////////////////////////////////////////////////
            //維護選單列出所有店家
            //////////////////////////////////////////////////////////////

            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            string strSQL = "";

            //參數帶入法(讀出資料庫- -店家)
            strSQL += "select * from Suppliers where ShopName like @SearchShopName\n";
            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有店家            
            cmd.Parameters.AddWithValue("@SearchShopName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbox_today_shop.Items.Add(reader["ShopName"]);
                cbox_select_shop.Items.Add(reader["ShopName"]);

            }

            //關閉讀取器和控制器
            reader.Close();

            //-------------------------------------------------------------------------------------------------

            //////////////////////////////////////////////////////////////
            //維護選單列出所有班級  
            //////////////////////////////////////////////////////////////
            cbox_login_class.Items.Clear();

            //參數帶入法(讀出資料庫- -班級)
            string strSQL2 = $"select * from Classes where ClassName like @SearchClassName \n";
            //模糊搜尋所有班級
            SqlCommand cmd2 = new SqlCommand(strSQL2, con);
            cmd2.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                //頁面顯示班級
                cbox_select_class.Items.Add(reader2["ClassName"]);
            }

            //關閉讀取器和控制器
            reader2.Close();



            //////////////////////////////////////////////////////////////
            //維護選單店家異動開放
            //////////////////////////////////////////////////////////////
            double TimeLeft;
            string strTimeLeft = "";
            //截止訂購時間
            TimeLeft = blocktime - Convert.ToDouble(DateTime.Now.ToString("HHmm")) - 40;
            strTimeLeft = $"{Math.Floor(TimeLeft / 60)}時:{Math.Ceiling(TimeLeft % 60)}分";  //$"{TimeLeft}" ;


            //當日截止時間負值允許變更
            if (TimeLeft < 0)
            {
                groupBox8.Enabled = true;



                //使用者畫面輸出
                tb_shop_name.Text = strTodayShop;
            }
            else { groupBox8.Enabled = false; }




            //進入頁面
            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = this.tabCon_Main_page;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = null;

            if (lbl_time_left.Text == "訂購已截止") ;
            else { MessageBox.Show("目前是訂購時間中，供餐店家變更於訂購結束後開放"); }


            //按鈕功能再次關閉
            btn_to_admin_Page.Enabled = false;

            //斷開全部連接
            con.Close();
        }/////////////////////////////////////////////////




        //維護者選單登出--首頁所有下拉選項重讀
        private void tb_Admin_GO_Login_Click(object sender, EventArgs e)
        {

            gpBox_admin_login.Visible = false;
            gpBox_passkey.Visible = false;
            gpBox_Admin_exchange.Visible = false;
            ckBox_admin_login.Checked = false;
            lbl_today_shop.Text = "歡迎使用本系統";

            //SQL語法查詢資料庫  
            //////////////////////////////////////////////////////////////
            cbox_login_class.Items.Clear();

            //建立並打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //參數帶入法(讀出資料庫- -班級)
            string strSQL = "select * from Classes where ClassName like @SearchClassName\n";


            //模糊搜尋所有班級
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.AddWithValue("@SearchClassID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbox_login_class.Items.Add(reader["ClassName"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();


            //清空登入時頁面姓名
            cbox_login_ID.Items.Clear();
            tb_passkey.Text = "";


            //登出
            go_login();

        }////////////////////////////////////////////////////////////////////



        //使用者登出鍵--首頁所有下拉選項重讀
        private void btn_User_Go_Login_Click(object sender, EventArgs e)
        {

            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////
            cbox_login_class.Items.Clear();

            //建立並打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //參數帶入法(讀出資料庫- -班級)
            string strSQL = "select * from Classes where ClassName like @SearchClassName\n";

            //模糊搜尋所有班級與姓名
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.AddWithValue("@SearchClassID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbox_login_class.Items.Add(reader["ClassName"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

            //清空登入時頁面姓名
            cbox_login_ID.Items.Clear();
            //tb_password.Text = "";

            //登出
            go_login();
        }////////////////////////////////////////////////////////////////////




        //登入畫面維護者登錄勾選框
        private void ckBox_admin_login_CheckedChanged(object sender, EventArgs e)
        {

            if (ckBox_admin_login.Checked == true)
            {
                gpBox_admin_login.Visible = true;
            }
            else
            {
                //清空姓名
                cbox_login_ID.Text = null;

                //關閉維護者輸入區域
                gpBox_admin_login.Visible = false;
                gpBox_Admin_exchange.Visible = false;

                //關閉貓眼2
                pictureBox7.Visible = false;
                pictureBox6.Visible = false;

                picBox_close_eye.Visible = true;
                picBox_open_eye.Visible = false;
                //時區設定關閉
                if (gpBox_admin_login.Visible == false || this.tPage_Admin_page.Parent == this.tabCon_Main_page)
                { tb_set_blocktime.Visible = false; };

            }

        }/////////////////////////////////////////////

        //-----------------------------------點餐價格計算---------------------------------------------

        //減少訂購個數
        private void btn_minus_unit_Click(object sender, EventArgs e)
        {
            GetQuantity--;
            if (GetQuantity < 0)
            {
                btn_minus_unit.Enabled = false;//杯數減按鈕於0杯時>自動失效
                GetQuantity = 0;
            }
            tb_Quantity.Text = GetQuantity.ToString();
            tb_RowTotal.Text = (GetUnitPrice * GetQuantity).ToString();
        }

        //增加訂購個數
        private void btn_plus_unit_Click(object sender, EventArgs e)
        {
            GetQuantity++;
            btn_minus_unit.Enabled = true;//杯數加按鈕時>重新啟用
            tb_Quantity.Text = GetQuantity.ToString();
            tb_RowTotal.Text = (GetUnitPrice * GetQuantity).ToString();
        }

        //訂購結果全部移除
        private void btn_user_order_all_clear_Click(object sender, EventArgs e)
        {
            DialogResult drResult; //對話框結果

            drResult = MessageBox.Show("您確認重新訂購？", "",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (drResult == DialogResult.No)
            {
                //取消
            }
            else
            {

                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();
                //string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                SqlCommand cmd = new SqlCommand($"delete from OrderDetails where CustomerID = '{tb_customer_ID.Text}' and OrderDate ='{tb_datetime.Text}' ", con);
                cmd.ExecuteNonQuery();
                label29.Text = lbl_ClassID.Text;
                label31.Text = tb_datetime.Text;


                // bindMyGridView();
                MessageBox.Show("重新訂購成功 !!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                SumTotal = 0;
                Sum1Total = 0;



                dataGridView2.Rows.Clear();
                dataGridView2.Refresh();
                RunSumTotal();
                lbl2_total_price.Text = null;
                lbl2_total_price.Text += "請重新開始您的訂購";
                con.Close();
            }
        }////////////////////////////////////////////////


        //加入新訂購單品
        private void btn_add_unit_price_Click(object sender, EventArgs e)
        {
            if (GetQuantity == 0)
                return;
            else//-------------------------------------------------------------------------------------------
            {


                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();
                //品項ID           
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = $" select ProductID from products where ProductName = '{cBox_product_User_page.Text}' ";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;
                string ProductID = Convert.ToString(cmd.ExecuteScalar());

                ////單價
                //SqlCommand cmd2 = new SqlCommand();
                //cmd2.CommandText = $"select UnitPrice from Products where ProductName ='{cBox_product_User_page.Text}' ";
                //cmd2.CommandType = System.Data.CommandType.Text;
                //cmd2.Connection = con;
                //int UnitPrice = Convert.ToInt32(cmd2.ExecuteScalar());

                //班級ID
                SqlCommand cmd3 = new SqlCommand();
                cmd3.CommandText = $"select ClassID from Classes where ClassID ='{lbl_ClassID.Text }' ";
                cmd3.CommandType = System.Data.CommandType.Text;
                cmd3.Connection = con;
                string ClassID = Convert.ToString(cmd3.ExecuteScalar());

                //學生ID
                SqlCommand cmd4 = new SqlCommand();
                cmd4.CommandText = $"select CustomerID from Customers where StudentName ='{tb_student_name.Text }' ";
                cmd4.CommandType = System.Data.CommandType.Text;
                cmd4.Connection = con;
                string CustomerID = Convert.ToString(cmd4.ExecuteScalar());

                //供應商ID
                SqlCommand cmd6 = new SqlCommand();
                cmd6.CommandText = $"select SupplierID from Suppliers where ShopName ='{ tb_shop_name.Text}' ";
                cmd6.CommandType = System.Data.CommandType.Text;
                cmd6.Connection = con;
                string SupplierID = Convert.ToString(cmd6.ExecuteScalar());

                //訂購日期
                string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));

                //列總額
                int RowTotal = GetUnitPrice * GetQuantity;

                //操作DataGridView
                //DataGridViewRowCollection rows = dataGridView2.Rows;
                //rows.Add(new Object[] { strProductID, strProductName, GetUnitPrice + "元", GetQuantity + "個", RowTotal + "元" });

                //全部金額累加
                Sum1Total += GetUnitPrice * GetQuantity;
                lbl_total_price.Text = $"您的當前金額：{Sum1Total.ToString()}元";



                //------------------------

                SqlCommand cmda2 = new SqlCommand($"Insert into OrderDetails Values( '{ClassID}','{OrderDate}','{CustomerID}','{ProductID}','{GetQuantity}','{RowTotal}' ) ", con);
                cmda2.ExecuteNonQuery(); ////執行SQL語法

                //------------------------

                bindMyGridView();
                con.Close();

                //讀出              
                SqlConnection con2 = new SqlConnection(scsb.ToString());
                con2.Open();

                string strSQL2 = "select DISTINCT II.ItemNO,IV.ProductName,IV.UnitPrice,II.Quantity,II.RowTotal " +
                                 "from(((Customers I " +
                                 "inner join OrderDetails II on I.CustomerID = II.CustomerID) " +
                                 "inner join OrderMasters III on I.ClassID = III.ClassID) " +
                                 "inner join Products IV on II.ProductID = IV.ProductID) " +
                                $"where II.OrderDate = '{OrderDate}' and II.CustomerID = '{CustomerID}'" +
                                $"order by itemNO";

                SqlCommand cmd1 = new SqlCommand(strSQL2, con2);
                SqlDataReader reader1 = cmd1.ExecuteReader();
                dataGridView2.Rows.Clear();
                while (reader1.Read())
                {
                    DataGridViewRowCollection rows1 = dataGridView2.Rows;
                    rows1.Add(reader1[0], reader1[1], reader1[2], reader1[3], reader1[4]);
                }
                reader1.Close();
                con2.Close();



                //選擇數量回復

                SqlConnection con3 = new SqlConnection(scsb.ToString());
                con3.Open();
                //參數帶入法(讀出資料庫- -便當)         
                string strSQL3 = "select * from Products where SupplierID = ( ";
                strSQL3 += $"select SupplierID from Suppliers where ShopName = '{tb_shop_name}' ) "; //從今天的店家
                strSQL3 += $"and ProductName ='{cBox_product_User_page.Text}' ";//從選擇的產品
                SqlCommand cmd5 = new SqlCommand(strSQL3, con3);
                string strPicID = "";
                //執行並讀取DATAREADER資料
                SqlDataReader reader2 = cmd5.ExecuteReader();
                while (reader2.Read())
                {
                    tb_RowTotal.Text = reader2["UnitPrice"].ToString();
                    GetUnitPrice = int.Parse(reader2["UnitPrice"].ToString());
                }
                tb_Quantity.Text = 1.ToString();
                GetQuantity = 1;
                tb_RowTotal.Text = (GetUnitPrice * GetQuantity).ToString();
                reader2.Close();
                con3.Close();
            }//------------------------------------------------------------------------------------------


        }////////////////////////////////////////////////






        //刪除單項訂購列
        private void btn_user_order_single_clear_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.SelectedRows.Count > 0)
            {
                try
                {
                    SqlConnection con = new SqlConnection(scsb.ToString());
                    con.Open();
                    String ItemNO = this.dataGridView2.SelectedRows[0].Cells[0].Value.ToString();
                    //dataGridView2.Rows.RemoveAt(this.dataGridView2.SelectedRows[0].Index);
                    //顯示刪除之金額
                    String MONEY = this.dataGridView2.SelectedRows[0].Cells[4].Value.ToString();


                    // bindMyGridView();
                    MessageBox.Show($"刪除單筆資料 {MONEY}元 成功 !!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    //string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    SqlCommand cmd = new SqlCommand($"delete from OrderDetails where ItemNO = '{ItemNO}' and OrderDate ='{tb_datetime.Text}' ", con);
                    cmd.ExecuteNonQuery();



                    label29.Text = ItemNO;
                    label31.Text = tb_datetime.Text;

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("搞屁阿？");
                }
            }

            //        try
            //        {
            //            throw new InvalidOperationException("You did something invalid.");
            //        }
            //        catch (Exception ex)
            //        {
            //< p > The exception message: @ex.Message </ p >
            //        }
            //        finally
            //        {
            //< p > The finally statement.</ p >
            //        }



        }//////////////////////////////////////////////////////////////////////////////






        private void btn_Order_BOM_send_Click(object sender, EventArgs e)
        {


            DialogResult drResult; //對話框結果

            drResult = MessageBox.Show("您確認送出您的訂購？", "甚麼?",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (drResult == DialogResult.No)
            {
                //取消
            }
            else
            {


                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();



                //品項ID           
                SqlCommand cmd10 = new SqlCommand();
                cmd10.CommandText = $" select ProductID from products where ProductName = '{cBox_product_User_page.Text}' ";
                cmd10.CommandType = System.Data.CommandType.Text;
                cmd10.Connection = con;
                string ProductID = Convert.ToString(cmd10.ExecuteScalar());

                ////單價
                //SqlCommand cmd2 = new SqlCommand();
                //cmd2.CommandText = $"select UnitPrice from Products where ProductName ='{cBox_product_User_page.Text}' ";
                //cmd2.CommandType = System.Data.CommandType.Text;
                //cmd2.Connection = con;
                //int UnitPrice = Convert.ToInt32(cmd2.ExecuteScalar());

                //班級ID
                SqlCommand cmd30 = new SqlCommand();
                cmd30.CommandText = $"select ClassID from Classes where ClassID ='{lbl_ClassID.Text}' ";
                cmd30.CommandType = System.Data.CommandType.Text;
                cmd30.Connection = con;
                string ClassID = Convert.ToString(cmd30.ExecuteScalar());

                //學生ID
                SqlCommand cmd40 = new SqlCommand();
                cmd40.CommandText = $"select CustomerID from Customers where StudentName ='{tb_student_name.Text }' ";
                cmd40.CommandType = System.Data.CommandType.Text;
                cmd40.Connection = con;
                string CustomerID = Convert.ToString(cmd40.ExecuteScalar());

                //供應商ID
                SqlCommand cmd60 = new SqlCommand();
                cmd60.CommandText = $"select SupplierID from Suppliers where ShopName ='{tb_shop_name.Text}' ";
                cmd60.CommandType = System.Data.CommandType.Text;
                cmd60.Connection = con;
                string SupplierID = Convert.ToString(cmd60.ExecuteScalar());

                //訂購日期
                string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));

                //迴圈筆數
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = $"select count(*) from OrderDetails where ClassID ='{CustomerID}' and OrderDate ='{OrderDate}' ";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;
                int countclass = Convert.ToInt32(cmd.ExecuteScalar());



                string strSQL = "SELECT  DISTINCT D.CustomerID,D.OrderDate,D.Quantity,D.RowTotal " +
                           "FROM Customers C, OrderDetails D, OrderMasters M " +
                            "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                            $"and D.OrderDate = '{OrderDate}' and D.CustomerID='{CustomerID}' ";

                int QtyTotal = 0;
                int SumTotal = 0;
                SqlCommand cmd1 = new SqlCommand(strSQL, con);
                SqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    QtyTotal += Convert.ToInt32(reader1["Quantity"]);
                    SumTotal += Convert.ToInt32(reader1["RowTotal"]);
                }
                reader1.Close();

                MessageBox.Show($"{lbl2_total_price.Text} 請速至櫃台繳費 謝謝!!");

                string strSQL2 = $"UPDATE OrderMasters SET " +
                                         $"SumTotal='{SumTotal}' " +
                                         $"where ClassID='{ClassID}' " +
                                         $"and OrderDate='{OrderDate}' ";
                SqlCommand cmda1 = new SqlCommand(strSQL2, con);
                cmda1.ExecuteNonQuery(); ////執行SQL語法


                lbl_total_price.Text = "請速至櫃檯繳費  謝謝～ !!";
                dataGridView2.Rows.Clear();
                dataGridView2.Refresh();

                con.Close();

            }



            //String stroutput = "";
            //int total = 0;
            //int totalquan = 0;
            //int c = 0;
            //stroutput = type + "期　" + compan + "　電話號碼" + Comphone + "　時間" + blocktime + "\n\n" +
            //    "品名\t數量\t單價\t總價\t訂購人\n";
            //while (reader.Read())
            //{
            //    stroutput += String.Format("{0}\t{1}\t{2}\t{3}\t{4}",
            //        reader["product_name"], reader["quan"], reader["price"], Convert.ToInt32(reader["quan"]) * Convert.ToInt32(reader["price"]), reader["name"] + "\n");
            //    totalquan += Convert.ToInt32(reader["quan"]);
            //    total += Convert.ToInt32(reader["quan"]) * Convert.ToInt32(reader["price"]);

            //    c++;
            //}
            //stroutput += "-----------------------------------------------------\n" +
            //             "\t" + totalquan.ToString() + "\t\t" + total.ToString() + "$\n" +
            //             "\t\t\t總計收費" + Convert.ToString(total) + "$\n" +
            //             "\t\t\t共" + c.ToString() + "筆";

            //reader.Close();
            //SqlCommand UPtotal = new SqlCommand("UPDATE order_master SET total_amt = @total where order_no='" + orderno + "'", con);
            //UPtotal.Parameters.AddWithValue("@total", total);
            //UPtotal.ExecuteNonQuery();
            //con.Close();






            //MessageBox.Show("功能有效");

            //DialogResult drResult; //對話框結果
            //drResult = MessageBox.Show("您確認送出訂購單", "訂購確認",
            //    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //if (drResult == DialogResult.No)
            //{
            //    //取消
            //}
            //else
            //{
            //    //確認訂購
            //    string strOrderList = "***** III冷飲店訂購單 *****\n";
            //    strOrderList += "---------------------------\n";
            //    if (杯數1 > 0)
            //    {
            //        strOrderList += lbl品項1.Text + ":"
            //        + lbl售價1.Text + "x" + tb杯數1.Text + "="
            //        + 品項1總價.ToString() + "\n";
            //    }
            //    if (杯數2 > 0)
            //    {
            //        strOrderList += lbl品項2.Text + ":"
            //        + lbl售價2.Text + "x" + tb杯數2.Text + "="
            //        + 品項2總價.ToString() + "\n";
            //    }
            //    if (杯數3 > 0)
            //    {
            //        strOrderList += lbl品項3.Text + ":"
            //        + lbl售價3.Text + "x" + tb杯數3.Text + "="
            //        + 品項3總價.ToString() + "\n";
            //    }
            //    if (杯數4 > 0)
            //    {
            //        strOrderList += lbl品項4.Text + ":"
            //        + lbl售價4.Text + "x" + tb杯數4.Text + "="
            //        + 品項4總價.ToString() + "\n";
            //    }
            //    if (杯數5 > 0)
            //    {
            //        strOrderList += lbl品項5.Text + ":"
            //        + lbl售價5.Text + "x" + tb杯數5.Text + "="
            //        + 品項5總價.ToString() + "\n";
            //    }


            //    strOrderList += "-----------------------------\n";
            //    if (折數 < 10.0)
            //    {
            //        strOrderList += "折數" + string.Format("{0:F2}", 折數) + "\n";
            //    }
            //    strOrderList += "訂單總價" + string.Format("{0:C}", 總價) + "\n";
            //    strOrderList += "折扣後總價" + string.Format("{0:C}", 折扣後總價) + "\n";
            //    strOrderList += string.Format("{0:D}", DateTime.Now) + "\n";
            //    strOrderList += string.Format("{0:T}", DateTime.Now) + "\n";
            //    MessageBox.Show(strOrderList, "訂單明細", MessageBoxButtons.OK);

            //    //清空變數
            //    tb杯數1.Text = "0";
            //    tb杯數2.Text = "0";
            //    tb杯數3.Text = "0";
            //    tb杯數4.Text = "0";
            //    tb杯數5.Text = "0";
            //}


        }//-----------------------------------------------------------------------------------------






        //使用者下拉式產品選單
        private void cBox_product_User_page_SelectedIndexChanged(object sender, EventArgs e)
        {



            //-------------------------------------------------------------------------------------------

            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////

            //圖檔清空
            if (picBox_User_order.Image != null)
            {
                picBox_User_order.Image.Dispose();
                picBox_User_order.Image = null;
            }

            string strGetProd = ""; //取值登入者已選產品
            strGetProd = cBox_product_User_page.SelectedItem.ToString();

            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            string strSQL2 = "";

            //參數帶入法(讀出資料庫- -便當)         
            strSQL2 += "select * from Products where SupplierID = ( ";
            strSQL2 += $"select SupplierID from Suppliers where ShopName = '{strTodayShop}' ) "; //從今天的店家
            strSQL2 += $"and ProductName ='{strGetProd}' ";//從選擇的產品

            //建立SQL命令對象
            SqlCommand cmd2 = new SqlCommand(strSQL2, con);
            //cmd2.Parameters.AddWithValue("@SearchProductName", "%" + $"{strGetProd}" + "%");
            //cmd2.Parameters.AddWithValue("@SearchProductID", "%" + "" + "%");
            //cmd.Parameters.AddWithValue("@SearchSupplierID", "%" + "" + "%");

            //模糊搜尋所有便當照片
            cmd2.Parameters.AddWithValue("@SearchPictureID", "%" + "" + "%");



            string strPicID = "";
            //執行並讀取DATAREADER資料
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                strProductName = reader2["ProductName"].ToString();
                strProductID = reader2["ProductID"].ToString();
                strPicID = reader2["PictureID"].ToString();
                //cBox_product_User_page.Items.Add(reader["ProductName"]);

                tb_RowTotal.Text = reader2["UnitPrice"].ToString();
                GetUnitPrice = int.Parse(reader2["UnitPrice"].ToString());


            }

            /////////////////////////
            //檔案呼叫相對路徑
            string path = "";
            if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows應用程式則相等   
            { path = AppDomain.CurrentDomain.BaseDirectory; }
            else { path = AppDomain.CurrentDomain.BaseDirectory + @"lunch_pictrue\"; }

            picBox_User_order.Image = Image.FromFile(path + $"{strPicID}.jpg");
            ////////////////////////

            //關閉讀取器和控制器
            reader2.Close();
            con.Close();

            //選擇數量回復
            tb_Quantity.Text = 1.ToString();
            GetQuantity = 1;


        }/////////////////////////////////////////////////////////////////////





        //-------------------------------------------------------------------------


        //使用者登入--讀取資料庫使用者基本資料
        private void cbox_login_class_MouseEnter(object sender, EventArgs e)
        {

            ////SQL語法查詢資料庫 
            ////////////////////////////////////////////////////////////////
            //cbox_login_class.Items.Clear();

            ////建立並打開連結
            //SqlConnection con = new SqlConnection(scsb.ToString());
            //con.Open();
            ////參數帶入法(讀出資料庫- -姓名)
            //string strSQL = "select * from Classes where ClassName like @SearchClassName\n";

            ////模糊搜尋所有班級
            //SqlCommand cmd = new SqlCommand(strSQL, con);
            //cmd.Parameters.AddWithValue("@SearchClassID", "%" + "" + "%");
            //cmd.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            ////執行並讀取DATAREADER資料
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    cbox_login_class.Items.Add(reader["ClassName"]);
            //}

            ////關閉讀取器和控制器
            //reader.Close();
            //con.Close();
        }////////////////////////////////////////////////////////////////////

        //條列式產品選單開關
        ////////////////////////////////////////////////////////////////////
        private void btn_lBox_product_Open_user_page_Click(object sender, EventArgs e)
        {
            btn2_minus_unit.Visible = true;
            btn2_plus_unit.Visible = true;
            btn2_add_unit_price.Visible = true;
            lBox_product_User_page.Visible = true;
            btn_lBox_product_close.Visible = true;
            btn_lBox_product_Open_user_page.Visible = false;
        }

        private void btn_lBox_product_close_Click(object sender, EventArgs e)
        {
            btn2_minus_unit.Visible = false;
            btn2_plus_unit.Visible = false;
            btn2_add_unit_price.Visible = false;
            lBox_product_User_page.Visible = false;
            btn_lBox_product_close.Visible = false;
            btn_lBox_product_Open_user_page.Visible = true;
        }
        /////////////////////////////////////////////////////////////////

        //條列式產品選單
        private void lBox_product_User_page_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lBox_product_User_page.SelectedItem == null || lBox_product_User_page.SelectedItem == "")
                return;
            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////

            if (picBox_User_order.Image != null)
            {
                picBox_User_order.Image.Dispose();
                picBox_User_order.Image = null;
            }

            string strGetProd = ""; //取值登入者已選產品
            strGetProd = lBox_product_User_page.SelectedItem.ToString();
            cBox_product_User_page.Text = strGetProd;

            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            string strSQL = "";

            //參數帶入法(讀出資料庫- -便當)         
            strSQL += "select * from Products where SupplierID = ( ";
            strSQL += $"select SupplierID from Suppliers where ShopName = '{strTodayShop}' ) "; //從今天的店家
            strSQL += $"and ProductName ='{strGetProd}' ";//從選擇的產品

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有商品           
            cmd.Parameters.AddWithValue("@SearchProductName", "%" + $"{strGetProd}" + "%");
            cmd.Parameters.AddWithValue("@SearchProductID", "%" + "" + "%");
            //cmd.Parameters.AddWithValue("@SearchSupplierID", "%" + "" + "%");

            //模糊搜尋所有便當照片
            cmd.Parameters.AddWithValue("@SearchPictureID", "%" + "" + "%");



            string strPicID = "";
            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                strProductName = reader["ProductName"].ToString();
                strProductID = reader["ProductID"].ToString();


                tb_RowTotal.Text = reader["UnitPrice"].ToString() + "元";
                GetUnitPrice = int.Parse(reader["UnitPrice"].ToString());

                strPicID = reader["PictureID"].ToString();
                //cBox_product_User_page.Items.Add(reader["ProductName"]);
            }

            /////////////////////////
            //檔案呼叫相對路徑
            string path = "";
            if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows應用程式則相等   
            { path = AppDomain.CurrentDomain.BaseDirectory; }
            else { path = AppDomain.CurrentDomain.BaseDirectory + @"lunch_pictrue\"; }

            picBox_User_order.Image = Image.FromFile(path + $"{strPicID}.jpg");
            ////////////////////////

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

            //選擇數量回復
            tb_Quantity.Text = 1.ToString();
            GetQuantity = 1;


        }//////////////////////////////////////////////////////



        //-------------------------------------------店家編輯往返--------------------------------------------

        //維護者選單進入店面編輯
        private void btn_Admin_Go_Shop_Click(object sender, EventArgs e)
        {

            //讀取資料庫資料 
            //////////////////////////////////////////////////////////////
            //前一次選單讀取值清空
            cBox_shop_supplier_name.Items.Clear();


            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            string strSQL = "";

            //參數帶入法(讀出資料庫- -店家)
            strSQL += "select * from Suppliers where ShopName like @SearchShopName\n";
            //select * from Suppliers where SupplierID like 'H01'

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有店家
            //cmd.Parameters.AddWithValue("@SearchSupplierID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchShopName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cBox_shop_supplier_name.Items.Add(reader["ShopName"]);
                tb_shop_phone.Text = reader["Phone"].ToString();

            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();






            //進入
            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = null;
            this.tPage_Shop_page.Parent = this.tabCon_Main_page;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = null;
        }

        //店面編輯返回維護者選單
        private void btn_Shop_Go_Admin_Click(object sender, EventArgs e)
        {
            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = this.tabCon_Main_page;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = null;
        }

        //-------------------------------------------班級編輯往返--------------------------------------------

        //維護者主畫面進入班級編輯
        private void btn_Admin_Go_Class_Click(object sender, EventArgs e)
        {
            //SQL語法查詢資料庫  
            //////////////////////////////////////////////////////////////
            cBox_class_class_name.Items.Clear();

            //建立並打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //參數帶入法(讀出資料庫- -班級)
            string strSQL = "select * from Classes where ClassName like @SearchClassName\n";
            // strSQL += "select * from Products where ProductName like @SearchProductName\n";

            //模糊搜尋所有班級
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.AddWithValue("@SearchClassID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cBox_class_class_name.Items.Add(reader["ClassName"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();



            //進入
            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = null;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = this.tabCon_Main_page;
            this.tPage_Result_page.Parent = null;
        }

        //班級編輯返回維護者主畫面
        private void btn_Class_Go_Admin_Click(object sender, EventArgs e)
        {
            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = this.tabCon_Main_page;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = null;
        }














        //____________________________________函式區_________________________________________
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void go_login()
        {
            this.tPage_Login_page.Parent = this.tabCon_Main_page;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = null;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = null;
        }


        private void dataGridView1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void dataGridView1_MouseHover(object sender, EventArgs e)
        {
            // int GridTotal = 0;

            // //this.dataGridView1.SelectedCells[0].Value.ToString();
            // foreach (var listBoxItem in dataGridView1.Rows)
            // {
            //     GridTotal += int.Parse(dataGridView1.Columns["RowTotal"].Displayed.ToString());
            // }
            // label7.Text = "您總共要付款：" + GridTotal +"元";

            // //dataGridView1.Row[i].Cells[j].Value;
            // int a = dataGridView1.CurrentRow.Index;
            //// string str = dataGridView1.Row[a].Cells["strName"].Value.Tostring();
            // //选中行的某个数据

        }

        //開啟明碼
        private void picBox_close_eye_MouseHover(object sender, EventArgs e)
        {
            tb_password.PasswordChar = '\0';
            picBox_close_eye.Visible = false;
            picBox_open_eye.Visible = true;
        }
        //開啟明碼
        private void pictureBox7_MouseHover(object sender, EventArgs e)
        {
            tb_passkey.PasswordChar = '\0';
            pictureBox7.Visible = false;
            pictureBox6.Visible = true;
        }

        //輸入欄位懸掛關閉明碼
        private void tb_password_MouseHover(object sender, EventArgs e)
        {
            tb_password.PasswordChar = '*';
            picBox_close_eye.Visible = true;
            picBox_open_eye.Visible = false;

        }
        //輸入欄位懸掛關閉明碼
        private void tb_passkey_MouseHover(object sender, EventArgs e)
        {
            tb_passkey.PasswordChar = '*';
            pictureBox7.Visible = true;
            pictureBox6.Visible = false;
        }
        //頁面懸掛關閉明碼
        private void tPage_Login_page_MouseHover(object sender, EventArgs e)
        {


            tb_password.PasswordChar = '*';
            tb_passkey.PasswordChar = '*';
            picBox_close_eye.Visible = true;
            picBox_open_eye.Visible = false;

            if (gpBox_admin_login.Visible == true)
            {
                if (gpBox_passkey.Visible == true)
                {
                    pictureBox7.Visible = true;
                    pictureBox6.Visible = false;
                }
            }
            else
            {
                tb_password.PasswordChar = '*';
                pictureBox7.Visible = false;
                pictureBox6.Visible = false;
                picBox_close_eye.Visible = true;
                picBox_open_eye.Visible = false;
            }

        }

        //離開關閉明碼
        private void tabCon_Main_page_MouseLeave(object sender, EventArgs e)
        {
            if (gpBox_admin_login.Visible == true)
            {
                tb_password.PasswordChar = '*';
                picBox_close_eye.Visible = true;
                picBox_open_eye.Visible = false;

                tb_passkey.PasswordChar = '*';
                pictureBox7.Visible = true;
                pictureBox6.Visible = false;
            }
            else
            {
                tb_password.PasswordChar = '*';
                picBox_close_eye.Visible = false;
                picBox_open_eye.Visible = false;
            }

        }

        private void cBox_shop_supplier_name_SelectedIndexChanged(object sender, EventArgs e)
        {

            //開啟點餐功能
            ////////////////////////////////////////
            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////

            //使用者畫面欄位初始化
            lbox_shop_messagebox.Items.Clear();
            lbl_shop_product_ID.Text = "";
            tb_shop_product_name.Text = "";
            tb_shop_unit_price.Text = "";

            tb_shop_supplier_name.Clear();
            tb_shop_phone.Clear();



            if (picBox_User_order.Image == null)
            {
                // picBox_shop_display.Image.Dispose();
                picBox_shop_display.Image = null;
            }

            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            string strSQL = "";

            //參數帶入法(讀出資料庫- -便當)
            //參數帶入法(讀出資料庫- -便當)
            strSQL += $"SELECT* FROM Suppliers I " +
                            $"inner join Products II on I.SupplierID = II.SupplierID " +
                            $"WHERE I.ShopName = '{cBox_shop_supplier_name.SelectedItem.ToString()}' ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            // string strPicID = "";
            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lbox_shop_messagebox.Items.Add(reader["ProductName"]);
                tb_shop_phone.Text = reader["Phone"].ToString();
                tb_shop_supplier_name.Text = reader["ShopName"].ToString();

                if (lbox_shop_messagebox.Items.ToString() == "")
                {


                    /////////////////////////
                    //檔案呼叫相對路徑
                    string path = "";
                    if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows應用程式則相等   
                    { path = AppDomain.CurrentDomain.BaseDirectory; }
                    else { path = AppDomain.CurrentDomain.BaseDirectory + "NONE.jpg"; }

                    if (picBox_shop_display.Image == null)
                    {

                        picBox_shop_display.Image = null;
                        picBox_shop_display.Image = Image.FromFile(path + "NONE.jpg");
                    }

                    ////////////////////////

                    //if (strPicID == "")
                    //{ picBox_shop_display.Image = Image.FromFile(path + "NONE.jpg"); }


                    //picBox_shop_display.Image = Image.FromFile(path + "NONE.jpg");

                    ////參數帶入法(讀出資料庫- -便當)         
                    //string strSQL2 = "select * from Products where SupplierID = ( ";
                    //strSQL2 += $"select SupplierID from Suppliers where ShopName = '{cBox_shop_supplier_name.Text}' ) "; //從今天的店家

                    ////建立SQL命令對象
                    //SqlCommand cmd1 = new SqlCommand(strSQL2, con);

                    ////模糊搜尋所有商品           
                    //cmd.Parameters.AddWithValue("@SearchProductName", "%" + $"{lbox_shop_messagebox.SelectedItem.ToString()}" + "%");
                    //cmd.Parameters.AddWithValue("@SearchProductID", "%" + "" + "%");
                    //cmd.Parameters.AddWithValue("@SearchPictureID", "%" + "" + "%");



                    //string strPicID = "";
                    ////執行並讀取DATAREADER資料
                    //SqlDataReader reader1 = cmd1.ExecuteReader();
                    //while (reader1.Read())
                    //{

                    //    strPicID = reader1["PictureID"].ToString();
                    //    //cBox_product_User_page.Items.Add(reader["ProductName"]);
                    //}

                    ///////////////////////////
                    ////檔案呼叫相對路徑
                    //string path = "";
                    //if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows應用程式則相等   
                    //{ path = AppDomain.CurrentDomain.BaseDirectory; }
                    //else { path = AppDomain.CurrentDomain.BaseDirectory + @"lunch_pictrue\"; }

                    //picBox_shop_display.Image = Image.FromFile(path + $"{strPicID}.jpg");
                    //////////////////////////

                    //if (strPicID == "")
                    //{ picBox_shop_display.Image = Image.FromFile(path + "NONE.jpg"); }

                    ////關閉讀取器和控制器
                    //reader1.Close();
                }
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();


            //----------

            //建立與打開連結

            if (lbox_shop_messagebox.Text == "")
            {

                con.Open();
                string strSQL2 = "";

                //參數帶入法(讀出資料庫- -便當)
                //參數帶入法(讀出資料庫- -便當)
                strSQL2 += $"SELECT* FROM Suppliers " +
                                $"WHERE ShopName = '{cBox_shop_supplier_name.SelectedItem.ToString()}' ";

                //建立SQL命令對象
                SqlCommand cmd2 = new SqlCommand(strSQL2, con);

                // string strPicID = "";
                //執行並讀取DATAREADER資料
                SqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    //lbox_shop_messagebox.Items.Add(reader2["ProductName"]);
                    tb_shop_phone.Text = reader2["Phone"].ToString();
                    tb_shop_supplier_name.Text = reader2["ShopName"].ToString();
                }

                //關閉讀取器和控制器
                reader2.Close();
                con.Close();
            }


        }

        private void cbox_select_shop_SelectedIndexChanged(object sender, EventArgs e)
        {
            //開啟點餐功能
            ////////////////////////////////////////
            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////

            //使用者畫面欄位初始化
            // cBox_shop_supplier_name.Items.Clear();
            //lbox_shop_messagebox.Items.Clear();


            if (picBox_shop_display.Image != null)
            {
                picBox_shop_display.Image.Dispose();
                picBox_shop_display.Image = null;
            }

            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            string strSQL = "";

            //參數帶入法(讀出資料庫- -便當)
            strSQL += $"SELECT* FROM Suppliers I " +
                            $"inner join Products II on I.SupplierID = II.SupplierID " +
                            $"WHERE I.ShopName = '{cbox_select_shop.Text}' ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有商品
            cmd.Parameters.AddWithValue("@SearchShopName", "%" + "" + "%");

            //模糊搜尋所有商品
            cmd.Parameters.AddWithValue("@SearchSupplierID", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tb_shop_supplier_name.Text = reader["ShopName"].ToString();
                lbox_shop_messagebox.Items.Add(reader["ProductName"]);
                tb_shop_phone.Text = reader["Phone"].ToString();
            }


            //關閉讀取器和控制器
            reader.Close();
            con.Close();
        }

        private void lbox_shop_messagebox_SelectedIndexChanged(object sender, EventArgs e)
        {

            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////

            if (lbox_shop_messagebox.SelectedItem == null || lbox_shop_messagebox.SelectedItem == "")
                return;

            //建立與打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            string strSQL = "";

            //參數帶入法(讀出資料庫- -便當)         
            strSQL += "select * from Products where SupplierID = ( ";
            strSQL += $"select SupplierID from Suppliers where ShopName = '{cBox_shop_supplier_name.Text}' ) "; //從今天的店家
            strSQL += $"and ProductName ='{lbox_shop_messagebox.SelectedItem.ToString()}' ";//從選擇的產品

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有商品           
            cmd.Parameters.AddWithValue("@SearchProductName", "%" + $"{lbox_shop_messagebox.SelectedItem.ToString()}" + "%");
            cmd.Parameters.AddWithValue("@SearchProductID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchPictureID", "%" + "" + "%");



            string strPicID = "";
            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lbl_shop_product_ID.Text = reader["ProductID"].ToString();
                tb_shop_product_name.Text = reader["ProductName"].ToString();
                tb_shop_unit_price.Text = reader["UnitPrice"].ToString() + "元";
                strPicID = reader["PictureID"].ToString();
                //cBox_product_User_page.Items.Add(reader["ProductName"]);
            }

            /////////////////////////
            //檔案呼叫相對路徑
            string path = "";
            if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows應用程式則相等   
            { path = AppDomain.CurrentDomain.BaseDirectory; }
            else { path = AppDomain.CurrentDomain.BaseDirectory + @"lunch_pictrue\"; }

            picBox_shop_display.Image = Image.FromFile(path + $"{strPicID}.jpg");
            ////////////////////////

            if (strPicID == "")
            { picBox_shop_display.Image = Image.FromFile(path + "NONE.jpg"); }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();
        }

        string strStudentClassID = "";
        private void cBox_class_class_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            //若班級已選取則讀取所有姓名資料
            //////////////////////////////////////////////////////////////           
            lbox_class_messagebox.Items.Clear();

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();


            //搜尋所有班級對應姓名
            string strSQL = "select* from Customers where ClassID = ( ";
            strSQL += $"select ClassID from Classes where ClassName = '{cBox_class_class_name.SelectedItem.ToString()}' ) ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lbox_class_messagebox.Items.Add(reader["StudentName"]);
                strStudentClassID = reader["ClassID"].ToString();
                tb_CLASS_class_name.Text = cBox_class_class_name.SelectedItem.ToString();

            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

        }/////////////////////////////////////////////////

        private void cbox_select_class_SelectedIndexChanged(object sender, EventArgs e)
        {


        }/////////////////////////////////

        private void lbox_class_messagebox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbox_class_messagebox.SelectedItem == null || lbox_class_messagebox.SelectedItem == "")
                return;

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //搜尋所有班級對應姓名
            string strSQL = "select* from Customers where ClassID = ( ";
            strSQL += $"select ClassID from Classes where ClassName = '{tb_CLASS_class_name.Text}') ";
            strSQL += $"and StudentName = '{lbox_class_messagebox.SelectedItem.ToString()}' ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有帳號
            cmd.Parameters.AddWithValue("@SearchAccount", "%" + tb_account.Text + "%");//模糊搜尋

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();//執行資料庫讀取器

            //定義檢查帳號用參考變數
            string check_account = "";
            string check_password = "";


            if (reader.Read())
            {
                //填入資料空格
                lbl_class_student_ID.Text = string.Format("{0}", reader["CustomerID"]);
                tb_class_student_name.Text = string.Format("{0}", reader["StudentName"]);
                tb_class_student_account.Text = string.Format("{0}", reader["StudentAccount"]);
                tb_class_student_password.Text = string.Format("{0}", reader["StudentPassword"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

        }///////////////////////////////////////////////////

        private void btn2_plus_unit_Click(object sender, EventArgs e)
        {
            GetQuantity++;
            btn_minus_unit.Enabled = true;//杯數加按鈕時>重新啟用
            tb_Quantity.Text = GetQuantity.ToString();
            tb_RowTotal.Text = (GetUnitPrice * GetQuantity).ToString();
        }

        private void btn2_minus_unit_Click(object sender, EventArgs e)
        {
            GetQuantity--;
            if (GetQuantity < 0)
            {
                btn_minus_unit.Enabled = false;//杯數減按鈕於0杯時>自動失效
                GetQuantity = 0;
            }
            tb_Quantity.Text = GetQuantity.ToString();
            tb_RowTotal.Text = (GetUnitPrice * GetQuantity).ToString();
        }


        private void btn_create_data_Click(object sender, EventArgs e)
        {
            if (cBox_class_class_name.SelectedItem == null)
                return;

            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();


            //手動輸入新增資料
            string strSQL = "insert into Customers values" +
            "(@NewStudentID,@NewClassID,@NewName,@NewAccount,@NewPassword)";

            string StuNum = string.Format("{0:00}", lbox_class_messagebox.Items.Count + 1);
            string StuClss = string.Format("{0:00}", cBox_class_class_name.SelectedIndex + 1);
            SqlCommand cmd = new SqlCommand(strSQL, con);

            cmd.Parameters.AddWithValue("@NewStudentID", "S098" + StuClss + StuNum);
            cmd.Parameters.AddWithValue("@NewClassID", strStudentClassID);
            cmd.Parameters.AddWithValue("@NewName", tb_class_student_name.Text);
            cmd.Parameters.AddWithValue("@NewAccount", tb_class_student_account.Text);
            cmd.Parameters.AddWithValue("@NewPassword", tb_class_student_password.Text);


            int rows = cmd.ExecuteNonQuery();//執行但沒有查詢 卻還是有回傳值 回傳影響的比數
            con.Close();
            MessageBox.Show(string.Format("資料新增完畢,共影響{0}筆資料", rows));

        }/////////////////////////////////////



        private void btn_delete_data_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            string strSQL = "delete from Customers where CustomerID = @SearchID";
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.AddWithValue("@SearchID", lbl_class_student_ID.Text);

            int rows = cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show(string.Format("資料刪除完畢,共影響{0}筆資料", rows));

            //清空欄位
            lbl_class_student_ID.Text = "";
            tb_class_student_name.Text = "";
            tb_class_student_account.Text = "";
            tb_class_student_password.Text = "";


            lbox_class_messagebox.Items.Clear();//同時刪除listbox表單人名

        }

        private void tPage_Class_page_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void tPage_User_page_MouseMove(object sender, MouseEventArgs e)
        {

        }

        //使用者登出登入畫面功能連線重設
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            lbl_total_price.Text = "Total Price";
            lbl2_total_price.Text = "Total Price";
            tb_Quantity.Clear();
            tb_RowTotal.Clear();
            btn2_minus_unit.Visible = false;
            btn2_plus_unit.Visible = false;
            btn2_add_unit_price.Visible = false;
            lBox_product_User_page.Visible = false;
            btn_lBox_product_close.Visible = false;
            btn_lBox_product_Open_user_page.Visible = true;
            lbl_today_shop.Text = "歡迎使用本系統";



            //SQL語法查詢資料庫  
            //////////////////////////////////////////////////////////////
            cbox_login_class.Items.Clear();

            //建立並打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //參數帶入法(讀出資料庫- -班級)
            string strSQL = "select * from Classes where ClassName like @SearchClassName\n";
            // strSQL += "select * from Products where ProductName like @SearchProductName\n";

            //模糊搜尋所有班級
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.AddWithValue("@SearchClassID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbox_login_class.Items.Add(reader["ClassName"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

            gpBox_admin_login.Visible = false;
            gpBox_passkey.Visible = false;
            ckBox_admin_login.Checked = false;
            gpBox_Admin_exchange.Visible = false;


            //清空登入時頁面姓名
            cbox_login_ID.Items.Clear();
            tb_passkey.Text = "";


            //登出
            go_login();
        }

        private void tb_password_TextChanged(object sender, EventArgs e)
        {
            if (tb_account.Text == "" && tb_password.Text == "")
                lbl_today_shop.Text = "歡迎使用本系統";
            else
            {

                //建立並打開連接         
                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();

                //參數帶入法(篩選客戶裡的帳號)
                string strSQL = "select*from ChosenShop where ClassID  = ( ";
                strSQL += $"select ClassID from Customers where StudentAccount = '{tb_account.Text}') ";
                //建立SQL命令對象
                SqlCommand cmd = new SqlCommand(strSQL, con);

                ////模糊搜尋所有帳號
                //cmd.Parameters.AddWithValue("@SearchAccount", "%" + tb_account.Text + "%");//模糊搜尋

                //執行並讀取DATAREADER資料
                SqlDataReader reader = cmd.ExecuteReader();//執行資料庫讀取器
                while (reader.Read())
                {
                    lbl_today_shop.Text = string.Format("供餐店家：{0}", reader["ShopName"]);
                }
                reader.Close();



                //參數帶入法(篩選客戶裡的帳號)
                string strSQL2 = "select*from Classes where ClassID  = ( ";
                strSQL2 += $"select ClassID from Customers where StudentAccount = '{tb_account.Text}') ";


                //建立SQL命令對象           
                SqlCommand cmd2 = new SqlCommand(strSQL2, con);
                //模糊搜尋所有帳號
                //cmd2.Parameters.AddWithValue("@SearchAccount", "%" + tb_account.Text + "%");//模糊搜尋
                //執行並讀取DATAREADER資料
                SqlDataReader reader2 = cmd2.ExecuteReader();//執行資料庫讀取器
                while (reader2.Read())
                {
                    lbl_today_shop.Text += string.Format("\n登入班級：{0}", reader2["ClassName"]);
                    lbl_USER_class.Text = string.Format("{0}", reader2["ClassName"]);


                }
                reader2.Close();

                //關閉全部連結
                con.Close();

            }

        }///////////////////////////////////////////////////////

        //系統關閉
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cbox_select_class_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //若班級已選取則讀取所有姓名資料
            //////////////////////////////////////////////////////////////           
            lbox_class_messagebox.Items.Clear();

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();


            //搜尋所有班級對應姓名
            string strSQL = "select* from Customers where ClassID = ( ";
            strSQL += $"select ClassID from Classes where ClassName = '{cbox_select_class.SelectedItem.ToString()}' ) ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lbox_class_messagebox.Items.Add(reader["StudentName"]);
                strStudentClassID = reader["ClassID"].ToString();
                tb_CLASS_class_name.Text = cbox_select_class.SelectedItem.ToString();

            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            cmdbl = new SqlCommandBuilder(adap);
            adap.Update(ds);
        }

        private void tbn_change_guy_Click(object sender, EventArgs e)
        {
            tbn_change_guy.Text = "啟動中..";
            cbox_login_class.Visible = false;
            label4.Text = "設定模式";
            MessageBox.Show("請開始選擇工作人員");


        }

        private void tb_set_blocktime_TextChanged(object sender, EventArgs e)
        {
            lbl_NOTE.Text = $"請於{tb_set_blocktime.Text}以前完成訂購";
        }

        private void tPage_Login_page_MouseMove(object sender, MouseEventArgs e)
        {
            lbl_NOTE.Text = $"請於{tb_set_blocktime.Text}以前完成訂購";

            if (label4.Text == "設定模式")
            {
                tbn_change_guy.Enabled = false;
                btn_Login_GO_User.Enabled = false;
                btn_to_admin_Page.Enabled = false;
            }
            else
            {
                tbn_change_guy.Enabled = true;
                btn_Login_GO_User.Enabled = true;
                btn_to_admin_Page.Enabled = true;
            }
        }

        private void dataGridView2_MouseEnter(object sender, EventArgs e)
        {
            SqlConnection con2 = new SqlConnection(scsb.ToString());
            con2.Open();

            //學生ID
            SqlCommand cmd40 = new SqlCommand();
            cmd40.CommandText = $"select CustomerID from Customers where StudentName ='{tb_student_name.Text}' ";
            cmd40.CommandType = System.Data.CommandType.Text;
            cmd40.Connection = con2;
            string CustomerID = Convert.ToString(cmd40.ExecuteScalar());

            //訂購月日期
            string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));




            //讀出GV
            string strSQL2 = "select DISTINCT II.ItemNO,IV.ProductName,IV.UnitPrice,II.Quantity,II.RowTotal,II.CustomerID, II.OrderDate " +
                             "from(((Customers I " +
                             "inner join OrderDetails II on I.CustomerID = II.CustomerID) " +
                             "inner join OrderMasters III on I.ClassID = III.ClassID) " +
                             "inner join Products IV on II.ProductID = IV.ProductID) " +
                            $"where II.OrderDate = '{OrderDate}' and II.CustomerID = '{CustomerID}'" +
                            $"order by itemNO";

            SqlCommand cmd2 = new SqlCommand(strSQL2, con2);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            dataGridView2.Rows.Clear();
            while (reader2.Read())
            {
                DataGridViewRowCollection rows1 = dataGridView2.Rows;
                rows1.Add(reader2[0], reader2[1], reader2[2], reader2[3], reader2[4]);
            }
            reader2.Close();


            //讀出SumTotal
            string strSQL = "SELECT  DISTINCT ItemNO,D.ClassID, D.OrderDate,D.CustomerID,D.ProductID,D.Quantity,D.RowTotal " +
                       "FROM Customers C, OrderDetails D, OrderMasters M " +
                        "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                        $"and D.CustomerID='{CustomerID}'  and D.OrderDate = '{OrderDate}' ";




            SqlCommand cmd1 = new SqlCommand(strSQL, con2);
            SqlDataReader reader1 = cmd1.ExecuteReader();
            SumTotal = 0;
            QtyTotal = 0;
            while (reader1.Read())
            {
                QtyTotal += Convert.ToInt32(reader1["Quantity"]);
                SumTotal += Convert.ToInt32(reader1["RowTotal"]);
            }
            reader1.Close();

            lbl_total_price.Text = "您的當前金額：" + SumTotal.ToString() + "元 ";
            lbl2_total_price.Text = "您的全部金額：" + SumTotal.ToString() + "元 ";

            con2.Close();
        }

        private void DataGridView3_MouseEnter(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //訂購日期
            string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));

            //排序依照(某學生ID)
            string OrderStu = lBox_Result_Select_name.SelectedItems.ToString();

            //班級ID
            SqlCommand cmd40 = new SqlCommand();
            cmd40.CommandText = $"select ClassID from Customers where StudentName ='{tb_Admin_Name.Text}' ";
            cmd40.CommandType = System.Data.CommandType.Text;
            cmd40.Connection = con;
            string ClassID = Convert.ToString(cmd40.ExecuteScalar());
            label37.Text = ClassID;

            //訂購店家ID
            SqlCommand cmd41 = new SqlCommand();
            cmd41.CommandText = $"select SupplierID from Products Where ProductID in (select ProductID from OrderDetails where ClassID = '{ClassID}' and OrderDate = '{OrderDate}')";
            cmd41.CommandType = System.Data.CommandType.Text;
            cmd41.Connection = con;
            string SupplierID = Convert.ToString(cmd41.ExecuteScalar());



            //讀出GV
            string strSQL2 = "select DISTINCT  II.ItemNO , II.OrderDate ,I.StudentName , IV.ProductName , IV.UnitPrice , II.Quantity , II.RowTotal,II.CustomerID " +
                             "from(((Customers I " +
                             "inner join OrderDetails II on I.CustomerID = II.CustomerID) " +
                             "inner join OrderMasters III on I.ClassID = III.ClassID) " +
                             "inner join Products IV on II.ProductID = IV.ProductID) " +
                            $"where II.OrderDate = '{OrderDate}' and II.ClassID = '{ClassID}'" +
                            $"order by itemNO ,I.StudentName  ";

            SqlCommand cmd2 = new SqlCommand(strSQL2, con);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            DataGridView3.Rows.Clear();
            while (reader2.Read())
            {
                DataGridViewRowCollection rows1 = DataGridView3.Rows;
                rows1.Add(reader2[0], reader2[1], reader2[2], reader2[3], reader2[4], reader2[5], reader2[6]);
            }
            reader2.Close();

            //----------------------------------------------------------------------------------
            string strTest = "";


            string strSQL7 = "SELECT  DISTINCT  D.ItemNO,D.ClassID,D.OrderDate,D.Quantity,D.RowTotal " +
                    "FROM Customers C, OrderDetails D, OrderMasters M " +
                     "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                     $"and D.OrderDate = '{OrderDate}' and D.ClassID='{ClassID}' ";

            int QtyTotalCLASS = 0;
            int SumTotalCLASS = 0;
            //string Name="";
            SqlCommand cmd7 = new SqlCommand(strSQL7, con);
            SqlDataReader reader7 = cmd7.ExecuteReader();
            QtyTotal = 0;
            SumTotal = 0;
            while (reader7.Read())
            {
                //Name+=Convert.ToInt32(reader1["StudentName"]);
                QtyTotal += Convert.ToInt32(reader7["Quantity"]);
                SumTotal += Convert.ToInt32(reader7["RowTotal"]);
            }
            if (QtyTotal != 0 || SumTotal != 0)
                strTest += $"＊ 全班已購買{QtyTotal}個物品　一共需繳款{SumTotal}元 ＊\n\n";
            else
                strTest += "本班級尚未有人進行訂購！！\n\n";

            reader7.Close();

            //---------------------------------------------------------------------


            strTest += "＊物品分類清單：\n";

            for (i = 1; i <= 3; i++)
            {
                SqlCommand cmd3 = new SqlCommand();
                cmd3.CommandText = $"select DISTINCT ProductName from OrderDetails I inner join  Products II on I.ProductID = II.ProductID  Where I.ClassID = 'C01' and I.OrderDate = '{OrderDate}' and I.ProductID = 'H01P0{i}' ";
                cmd3.CommandType = System.Data.CommandType.Text;
                cmd3.Connection = con;


                string strSQL4 = "SELECT  DISTINCT  D.ItemNO,C.StudentName,D.OrderDate,D.Quantity,D.RowTotal " +
                      "FROM Customers C, OrderDetails D, OrderMasters M " +
                       "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                       $"and  D.ClassID ='{ClassID}' and D.OrderDate = '{OrderDate}' and D.ProductID='{SupplierID}P0{i}' ";

                int QtyTotal4 = 0;
                int SumTotal4 = 0;
                //string Name="";
                SqlCommand cmd4 = new SqlCommand(strSQL4, con);
                SqlDataReader reader4 = cmd4.ExecuteReader();
                while (reader4.Read())
                {
                    //Name+=Convert.ToInt32(reader1["StudentName"]);
                    QtyTotal4 += Convert.ToInt32(reader4["Quantity"]);
                    SumTotal4 += Convert.ToInt32(reader4["RowTotal"]);
                }
                reader4.Close();
                if (QtyTotal4 != 0 || SumTotal4 != 0)
                {
                    strTest += " " + Convert.ToString(cmd3.ExecuteScalar()) + " ";
                    strTest += $"{QtyTotal4}個 共{SumTotal4}元\n";
                }

            }

            //--------------------------------------------------------------------------


            //分割ClassID
            string[] ClassNO = ClassID.Split('C');
            label36.Text = ClassNO[1].ToString();

            strTest += "\n";
            strTest += "＊個人購買清單：\n";

            for (int j = 1; j <= 3; j++)
            {
                SqlCommand cmd5 = new SqlCommand();
                cmd5.CommandText = $"select DISTINCT II.StudentName from OrderDetails I inner join  Customers II on I.CustomerID = II.CustomerID  Where I.ClassID = 'C01' and I.OrderDate = '2017/10/17' and I.CustomerID = 'S098010{j}' ";
                cmd5.CommandType = System.Data.CommandType.Text;
                cmd5.Connection = con;
                strTest += Convert.ToString(cmd5.ExecuteScalar()) + " ";

                string strSQL6 = "SELECT  DISTINCT  D.ItemNO,C.StudentName,D.OrderDate,D.Quantity,D.RowTotal " +
                        "FROM Customers C, OrderDetails D, OrderMasters M " +
                         "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                         $"and D.OrderDate = '{OrderDate}' and D.CustomerID='S098{ClassNO[1]}0{j}' ";

                QtyTotal = 0;
                SumTotal = 0;

                SqlCommand cmd6 = new SqlCommand(strSQL6, con);
                SqlDataReader reader6 = cmd6.ExecuteReader();
                while (reader6.Read())
                {
                    Name = reader6["StudentName"].ToString();
                    QtyTotal += Convert.ToInt32(reader6["Quantity"]);
                    SumTotal += Convert.ToInt32(reader6["RowTotal"]);
                }

                if (Name != "Main_page" && QtyTotal != 0 && SumTotal != 0)
                    strTest += $"{Name} 共購買{QtyTotal}個物品,需繳款{SumTotal}元\n";


                reader6.Close();
            }

            lbl_Result_strSQL.Text = strTest;

            con.Close();



        }

        private void btn_order_go_Click(object sender, EventArgs e)
        {
            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = null;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = this.tabCon_Main_page;

            //若班級已選取則讀取所有姓名資料
            //////////////////////////////////////////////////////////////           
            lBox_Result_Select_name.Items.Clear();

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();


            //搜尋所有班級對應姓名
            string strSQL = "select* from Customers where ClassID = ( ";
            strSQL += $"select ClassID from Classes where ClassName = '{tb_Admin_class.Text}' ) ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lBox_Result_Select_name.Items.Add(reader["StudentName"]);
                lbl_RESULT_CLASS.Text = tb_Admin_class.Text;
            }
            reader.Close();

            //讀表

            //班級ID
            SqlCommand cmd30 = new SqlCommand();
            cmd30.CommandText = $"select ClassID from Classes where ClassName ='{tb_Admin_class.Text}' ";
            cmd30.CommandType = System.Data.CommandType.Text;
            cmd30.Connection = con;
            string ClassID = Convert.ToString(cmd30.ExecuteScalar());

            //學生ID
            SqlCommand cmd40 = new SqlCommand();
            cmd40.CommandText = $"select CustomerID from Customers where StudentName ='{tb_Admin_Name.Text}' ";
            cmd40.CommandType = System.Data.CommandType.Text;
            cmd40.Connection = con;
            string CustomerID = Convert.ToString(cmd40.ExecuteScalar());

            //訂購日期
            string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));

            //排序依照(某學生ID)
            string OrderStu = lBox_Result_Select_name.SelectedItems.ToString();

            //讀出GV
            string strSQL2 = "select DISTINCT  II.ItemNO , II.OrderDate , II.CustomerID , IV.ProductName , IV.UnitPrice , II.Quantity , II.RowTotal " +
                             "from(((Customers I " +
                             "inner join OrderDetails II on I.CustomerID = II.CustomerID) " +
                             "inner join OrderMasters III on I.ClassID = III.ClassID) " +
                             "inner join Products IV on II.ProductID = IV.ProductID) " +
                            $"where II.OrderDate = '{OrderDate}' and II.ClassID = '{ClassID}'" +
                            $"order by itemNO , II.CustomerID  ";



            SqlCommand cmd2 = new SqlCommand(strSQL2, con);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            DataGridView3.Rows.Clear();
            while (reader2.Read())
            {
                DataGridViewRowCollection rows2 = DataGridView3.Rows;
                rows2.Add(reader2[0], reader2[1], reader2[2], reader2[3], reader2[4], reader2[5], reader2[6]);
            }
            reader2.Close();



            string strTest = "";

            strTest += "物品分類清單：\n";

            for (i = 1; i <= 3; i++)
            {
                SqlCommand cmd3 = new SqlCommand();
                cmd3.CommandText = $"select DISTINCT ProductName from OrderDetails I inner join  Products II on I.ProductID = II.ProductID  Where I.ClassID = 'C01' and I.OrderDate = '{OrderDate}' and I.ProductID = 'H01P0{i}' ";
                cmd3.CommandType = System.Data.CommandType.Text;
                cmd3.Connection = con;
                strTest += Convert.ToString(cmd3.ExecuteScalar()) + " ";


                //SqlCommand cmd4 = new SqlCommand();
                //cmd4.CommandText = $"select Quantity from OrderDetails Where ClassID ='C01' and OrderDate = '{OrderDate}' and ProductID = 'H01P0{i}' ";
                //cmd4.CommandType = System.Data.CommandType.Text;
                //cmd4.Connection = con;

                string strSQL4 = "SELECT  DISTINCT  D.ItemNO,C.StudentName,D.OrderDate,D.Quantity,D.RowTotal " +
                      "FROM Customers C, OrderDetails D, OrderMasters M " +
                       "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                       $"and  D.ClassID ='C01' and D.OrderDate = '{OrderDate}' and D.ProductID='H01P0{i}' ";

                int QtyTotal4 = 0;
                int SumTotal4 = 0;
                //string Name="";
                SqlCommand cmd4 = new SqlCommand(strSQL4, con);
                SqlDataReader reader4 = cmd4.ExecuteReader();
                while (reader4.Read())
                {
                    //Name+=Convert.ToInt32(reader1["StudentName"]);
                    QtyTotal4 += Convert.ToInt32(reader4["Quantity"]);
                    SumTotal4 += Convert.ToInt32(reader4["RowTotal"]);
                }
                reader4.Close();
                strTest += $"{QtyTotal4}個 共{SumTotal4}元\n";
            }

            strTest += "\n";
            strTest += "個人購買清單：\n";

            for (int j = 1; j <= 3; j++)
            {
                SqlCommand cmd5 = new SqlCommand();
                cmd5.CommandText = $"select DISTINCT II.StudentName from OrderDetails I inner join  Customers II on I.CustomerID = II.CustomerID  Where I.ClassID = 'C01' and I.OrderDate = '2017/10/17' and I.CustomerID = 'S098010{j}' ";
                cmd5.CommandType = System.Data.CommandType.Text;
                cmd5.Connection = con;
                strTest += Convert.ToString(cmd5.ExecuteScalar()) + " ";

                string strSQL6 = "SELECT  DISTINCT  D.ItemNO,C.StudentName,D.OrderDate,D.Quantity,D.RowTotal " +
                        "FROM Customers C, OrderDetails D, OrderMasters M " +
                         "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                         $"and D.OrderDate = '{OrderDate}' and D.CustomerID='S098010{j}' ";

                int QtyTotal = 0;
                int SumTotal = 0;
                //string Name="";
                SqlCommand cmd6 = new SqlCommand(strSQL6, con);
                SqlDataReader reader6 = cmd6.ExecuteReader();
                while (reader6.Read())
                {
                    //Name+=Convert.ToInt32(reader1["StudentName"]);
                    QtyTotal += Convert.ToInt32(reader6["Quantity"]);
                    SumTotal += Convert.ToInt32(reader6["RowTotal"]);
                }
                strTest += $"共購買{QtyTotal}個物品,需繳款{SumTotal}元\n";
                reader6.Close();
            }


            string strSQL7 = "SELECT  DISTINCT  D.ItemNO,D.ClassID,D.OrderDate,D.Quantity,D.RowTotal " +
                    "FROM Customers C, OrderDetails D, OrderMasters M " +
                     "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                     $"and D.OrderDate = '{OrderDate}' and D.ClassID='C01' ";
            int QtyTotalCLASS = 0;
            int SumTotalCLASS = 0;
            //string Name="";
            SqlCommand cmd7 = new SqlCommand(strSQL7, con);
            SqlDataReader reader7 = cmd7.ExecuteReader();
            while (reader7.Read())
            {
                //Name+=Convert.ToInt32(reader1["StudentName"]);
                QtyTotal += Convert.ToInt32(reader7["Quantity"]);
                SumTotal += Convert.ToInt32(reader7["RowTotal"]);
            }
            strTest += $"\n全班一共購買{QtyTotal}個物品,班級共需繳款{SumTotal}元\n";
            reader7.Close();


            lbl_Result_strSQL.Text = strTest;


            //  select RowTotal from OrderDetails Where ClassID = 'C01' and OrderDate = '2017/10/17' and ProductID = 'H01P01'


            con.Close();
        }///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_Result_TO_Admin_Click(object sender, EventArgs e)
        {
            lbl_Result_strSQL.Text = "";

            this.tPage_Login_page.Parent = null;
            this.tPage_User_page.Parent = null;
            this.tPage_Admin_page.Parent = this.tabCon_Main_page; ;
            this.tPage_Shop_page.Parent = null;
            this.tPage_Class_page.Parent = null;
            this.tPage_Result_page.Parent = null;
        }

        private void lBox_Result_Select_name_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (lbox_class_messagebox.SelectedItem == null || lbox_class_messagebox.SelectedItem == "")
                return;

            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            //搜尋所有班級對應姓名
            string strSQL = "select* from Customers where ClassID = ( ";
            strSQL += $"select ClassID from Classes where ClassName = '{tb_CLASS_class_name.Text}') ";
            strSQL += $"and StudentName = '{lbox_class_messagebox.SelectedItem.ToString()}' ";

            //建立SQL命令對象
            SqlCommand cmd = new SqlCommand(strSQL, con);

            //模糊搜尋所有帳號
            cmd.Parameters.AddWithValue("@SearchAccount", "%" + tb_account.Text + "%");//模糊搜尋

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();//執行資料庫讀取器


            if (reader.Read())
            {
                //填入資料空格
                lbl_class_student_ID.Text = string.Format("{0}", reader["CustomerID"]);
                tb_class_student_name.Text = string.Format("{0}", reader["StudentName"]);
                tb_class_student_account.Text = string.Format("{0}", reader["StudentAccount"]);
                tb_class_student_password.Text = string.Format("{0}", reader["StudentPassword"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();

        }

        private void btn2_Result_delete_Click(object sender, EventArgs e)
        {
            //建立並打開連接         
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            // https://zhidao.baidu.com/question/199060766.html
            //刪除單行或多行
            try
            {


                String ItemNO = this.DataGridView3.SelectedRows[0].Cells[0].Value.ToString();
                String MONEY = this.DataGridView3.SelectedRows[0].Cells[6].Value.ToString();

                // bindMyGridView();
                MessageBox.Show($"刪除單筆資料 {MONEY}元 成功 !!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                //string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                SqlCommand cmd = new SqlCommand($"delete from OrderDetails where ItemNO = '{ItemNO}'  ", con);
                cmd.ExecuteNonQuery();

                // and OrderDate = '{tb_datetime.Text}'

            }
            catch (Exception ex)
            {
                MessageBox.Show("請選擇一筆資料", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }




            //{
            //    foreach (DataGridViewRow dr in DataGridView3.Rows)
            //        dr.Cells[0].Value = ((CheckBox)DataGridView3.Controls.Find("check", true)[0]).Checked;
            //}



            /*       SqlConnection con = new SqlConnection(scsb.ToString());
                    con.Open();*/



            //-------------- -網路資源3
            /*
            Sample Code 提供給大家，自行理解囉
代碼:
//取得DataTable資料來源
DBCommand.CommandText = "Select * From VIEW_ASTAF";
            dt = CommonVariable.DBC.DBReader(DBConn, DBCommand);


            //將資料與DataGrid關連
            this.dgv1.DataSource = dt;


            //建立一個DataGridView的Column物件及其內容
            DataGridViewColumn dgvc = new DataGridViewCheckBoxColumn();
            dgvc.Width = 40;
            dgvc.Name = "選取";


            //新增到DataGridView內的第0欄
            this.dgv1.Columns.Insert(0, dgvc);


            至於取得值的方式，可以參考如下
            代碼:
foreach (DataGridViewRow dr in this.dgv1.Rows)
            {
                if (dr.Cells[0].Value != null && (bool)dr.Cells[0].Value)
                {
                    MessageBox.Show("財編號碼 " + ((System.Data.DataRowView)(dr.DataBoundItem)).Row.ItemArray[1] + " 被選取了！");
                }
            }
            */

            //--------------網路資源2
            /*
              private void gvBind()
        {
            using (SqlConnection con = new SqlConnection(ConnecString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select * from Region", con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    
                }
            }
        }
然後先建立一個 CheckBox 欄，等下才可以全部勾選

 
        {
            gvBind();
            
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;      //奇數列顏色
            
            //先建立個 CheckBox 欄
            DataGridViewCheckBoxColumn cbCol = new DataGridViewCheckBoxColumn();
            cbCol.Width = 50;   //設定寬度
            cbCol.HeaderText = "　全選";
            cbCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;   //置中
            dataGridView1.Columns.Insert(0, cbCol);
        }
像這樣

image

接著再建立要全選的 CheckBox ，一樣放到 Form1_Load 中

 

            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 -9;
            rect.Y = rect.Location.Y + (rect.Height / 2 -9);

            CheckBox cbHeader = new CheckBox();
            cbHeader.Name = "checkboxHeader";
            cbHeader.Size = new Size(18, 18);
            cbHeader.Location = rect.Location;
            //全選要設定的事件
            cbHeader.CheckedChanged += new EventHandler(cbHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dataGridView1.Controls.Add(cbHeader);
            
            #endregion
再把 CheckBox 全選的事件建立

 
        {
            foreach (DataGridViewRow dr in dataGridView1.Rows)
                dr.Cells[0].Value = ((CheckBox)dataGridView1.Controls.Find("checkboxHeader", true)[0]).Checked;
        } 


             */

        }

        private void btn_Result_delete_Click(object sender, EventArgs e)
        {
            try
            {
                int QtyTotal = 0;
                int SumTotal = 0;


                SqlConnection con = new SqlConnection(scsb.ToString());
                con.Open();

                //學生ID
                SqlCommand cmd40 = new SqlCommand();
                cmd40.CommandText = $"select CustomerID from Customers where StudentName ='{lBox_Result_Select_name.SelectedItem.ToString()}' ";
                cmd40.CommandType = System.Data.CommandType.Text;
                cmd40.Connection = con;
                string CustomerID = Convert.ToString(cmd40.ExecuteScalar());

                String ItemNO = this.DataGridView3.SelectedRows[0].Cells[0].Value.ToString();
                String MONEY = this.DataGridView3.SelectedRows[0].Cells[6].Value.ToString();

                //訂購日期
                string OrderDate = string.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));


                string strSQL = "SELECT D.CustomerID,D.OrderDate,D.Quantity,D.RowTotal " +
                           "FROM Customers C, OrderDetails D, OrderMasters M " +
                            "WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID " +
                            $"and D.OrderDate = '{OrderDate}' and D.CustomerID='{CustomerID}' ";


                SqlCommand cmd1 = new SqlCommand(strSQL, con);
                SqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    QtyTotal += Convert.ToInt32(reader1["Quantity"]);
                    SumTotal += Convert.ToInt32(reader1["RowTotal"]);
                }
                reader1.Close();

                //產品數量{QtyTotal} 總共{SumTotal}元
                // bindMyGridView();
                MessageBox.Show($"學號{CustomerID}, 學生{lBox_Result_Select_name.SelectedItem.ToString()} 刪除個人訂購資料  成功 !!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                //string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                SqlCommand cmd = new SqlCommand($"delete from OrderDetails where CustomerID ='{CustomerID}' ", con);
                cmd.ExecuteNonQuery();

                // and OrderDate = '{tb_datetime.Text}'

            }
            catch (Exception ex)
            {
                MessageBox.Show("請選擇一筆資料", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_SHOP_add_shop_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();

            if (tb_shop_supplier_name.Text == null)
                return;

            //string strSQL2 = $"UPDATE SupplierID SET " +
            //                           $"SumTotal='{SumTotal}' and  " +

            //SqlCommand cmda1 = new SqlCommand(strSQL2, con);
            //cmda1.ExecuteNonQuery(); ////執行SQL語法



            //手動輸入新增資料
            string strSQL = "insert into Suppliers values" +
            "(@SupplierID,@ShopName,@Phone)";


            string SupplierID = string.Format("{0:00}", cBox_shop_supplier_name.Items.Count + 1);
            SqlCommand cmd = new SqlCommand(strSQL, con);

            cmd.Parameters.AddWithValue("@SupplierID", "H" + SupplierID);
            cmd.Parameters.AddWithValue("@ShopName", tb_shop_supplier_name.Text);
            cmd.Parameters.AddWithValue("@Phone", tb_shop_phone.Text);


            int rows = cmd.ExecuteNonQuery();//執行但沒有查詢 卻還是有回傳值 回傳影響的比數

            MessageBox.Show(string.Format("資料新增完畢,共影響{0}筆資料", rows));

            //-------

            //讀取資料庫資料 
            //////////////////////////////////////////////////////////////
            //前一次選單讀取值清空
            cBox_shop_supplier_name.Items.Clear();



            string strSQL2 = "";

            //參數帶入法(讀出資料庫- -店家)
            strSQL2 += "select * from Suppliers where ShopName like @SearchShopName\n";
            //select * from Suppliers where SupplierID like 'H01'

            //建立SQL命令對象
            SqlCommand cmd1 = new SqlCommand(strSQL2, con);

            //模糊搜尋所有店家
            //cmd.Parameters.AddWithValue("@SearchSupplierID", "%" + "" + "%");
            cmd1.Parameters.AddWithValue("@SearchShopName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader1 = cmd1.ExecuteReader();
            while (reader1.Read())
            {
                cBox_shop_supplier_name.Items.Add(reader1["ShopName"]);
                tb_shop_phone.Text = reader1["Phone"].ToString();

            }

            //關閉讀取器和控制器
            reader1.Close();
            con.Close();
        }

        private void button21_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //String ItemNO = this.dataGridView2.SelectedRows[0].Cells[0].Value.ToString();
            //dataGridView2.Rows.RemoveAt(this.dataGridView2.SelectedRows[0].Index);
            //顯示刪除之金額
            String SHOP = this.cBox_shop_supplier_name.Text;


            // bindMyGridView();
            MessageBox.Show($"刪除店家 {SHOP}  成功 !!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


            //string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            SqlCommand cmd = new SqlCommand($"delete from Suppliers where ShopName = '{SHOP}' ", con);
            cmd.ExecuteNonQuery();

            //-------

            //讀取資料庫資料 
            //////////////////////////////////////////////////////////////
            //前一次選單讀取值清空
            cBox_shop_supplier_name.Items.Clear();
            con.Close();

            //建立與打開連結

            con.Open();

            string strSQL = "";

            //參數帶入法(讀出資料庫- -店家)
            strSQL += "select * from Suppliers where ShopName like @SearchShopName\n";
            //select * from Suppliers where SupplierID like 'H01'

            //建立SQL命令對象
            SqlCommand cmd1 = new SqlCommand(strSQL, con);

            //模糊搜尋所有店家
            //cmd.Parameters.AddWithValue("@SearchSupplierID", "%" + "" + "%");
            cmd1.Parameters.AddWithValue("@SearchShopName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                cBox_shop_supplier_name.Items.Add(reader["ShopName"]);
                tb_shop_supplier_name.Text = "";
                tb_shop_phone.Text = "";
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void cbox_login_class_MouseClick(object sender, MouseEventArgs e)
        {

            //SQL語法查詢資料庫 
            //////////////////////////////////////////////////////////////
            cbox_login_class.Items.Clear();

            //建立並打開連結
            SqlConnection con = new SqlConnection(scsb.ToString());
            con.Open();
            //參數帶入法(讀出資料庫- -姓名)
            string strSQL = "select * from Classes where ClassName like @SearchClassName\n";

            //模糊搜尋所有班級
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.AddWithValue("@SearchClassID", "%" + "" + "%");
            cmd.Parameters.AddWithValue("@SearchClassName", "%" + "" + "%");

            //執行並讀取DATAREADER資料
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbox_login_class.Items.Add(reader["ClassName"]);
            }

            //關閉讀取器和控制器
            reader.Close();
            con.Close();
        }

        private void tPage_Result_page_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void tb_account_TextChanged(object sender, EventArgs e)
        {

        }
    }//________________________public partial class Main_page : Form_________________________
}//__________________________namespace 期末專題_便當訂購系統________________________
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////

















/*
public void bindMyGridView()
{
if (dataGridView1.SelectedCells.ToString() == "" || dataGridView1.SelectedCells.ToString() == null)
    return;

SqlConnection con = new SqlConnection(scsb.ToString());
con.Open();
//SqlCommand cod = new SqlCommand("select name 姓名,class 期別,product_name 品名,quan 數量,price 價格,Companyname 廠商 from orders", con);
//SqlDataReader reader = cod.ExecuteReader();
//DataTable mydatatable = new DataTable();
//mydatatable.Load(reader);
//dataGridView1.DataSource = mydatatable;
//reader.Close();
//con.Close();

//if (type == "79")
//{
adap = new SqlDataAdapter("select item_no No,s.name 訂購人,p.product_name 品名,o.quan 數量 ,o.price 單價,o.quan*o.price 總價 from student s ,order_detail o,order_master om,products p where s.stu_id=o.stu_id and p.product_id=o.product_id and om.order_no=o.order_no and om.class='" + type + "'", con);
//}
//else if (type == "80")
//{
//    adap = new SqlDataAdapter("select item_no No,s.name 訂購人,p.product_name 品名,o.quan 數量 ,o.price 單價,o.quan*o.price 總價 from student s ,order_detail o,order_master om,products p where s.stu_id=o.stu_id and p.product_id=o.product_id and om.order_no=o.order_no and om.class='" + type + "'", con);
//}
//else if (type == "81")
//{
//    adap = new SqlDataAdapter("select item_no No,s.name 訂購人,p.product_name 品名,o.quan 數量 ,o.price 單價,o.quan*o.price 總價 from student s ,order_detail o,order_master om,products p where s.stu_id=o.stu_id and p.product_id=o.product_id and om.order_no=o.order_no and om.class='" + type + "'", con);
//}

ds = new System.Data.DataSet();
adap.Fill(ds);
dataGridView1.DataSource = ds.Tables[0];
con.Close();

dataGridView1.Columns[0].Width = 40;
dataGridView1.Columns[1].Width = 100;
dataGridView1.Columns[2].Width = 100;
dataGridView1.Columns[3].Width = 180;
dataGridView1.Columns[4].Width = 120;

bool blnColorCahnge = false;
foreach (DataGridViewRow r in dataGridView1.Rows)
{
    blnColorCahnge = !blnColorCahnge;
    if (blnColorCahnge)
        r.DefaultCellStyle.BackColor = Color.LightBlue;
    else
        r.DefaultCellStyle.BackColor = Color.White;
}

}
*/
















////建立並打開連接         
//SqlConnection con = new SqlConnection(scsb.ToString());       
//con.Open();

//String strSQL = "";

////建立SQL命令對象
//SqlCommand cmd = new SqlCommand(strSQL, con);

////得到Data結果集
//SqlDataReader reader = cmd.ExecuteReader();

////斷開Data和SQL
//reader.Close();
//con.Close();


//strSQL = "select ClassID from Classes where ClassName = '行動應用開發程式設計師' ";

// strSQL += "select  ClassID from Customers where  ClassID = " + strUserClassID;
//  strSQL += "select  ClassName from Classes where  ClassName = " + strUserClassName;

//   strSQL = "select* from Customers where ClassID = ( "+
//   "select ClassID from Classes where ClassName = '/ "+ SQLClassName + " '/  )"; //+ strUserClassID;

//string strSQL = "select * from Customers where StudentName like @SearchName";

//strSQL += " and  ClassName = " + strUserClass;
//strSQL +=  ' select ClassID, StudentName from Customers where ClassID = ( select ClassID from Classes where ClassName = 行動應用開發程式設計師 )';











//     string imgid = Request.QueryString["imgid"];
//string connstr = ((NameValueCollection)
//Context.GetConfig("appSettings"))["connstr"];
//string sql = "SELECT imgdata, imgtype FROM ImageStore WHERE id = " + imgid;
//SqlConnection connection = new SqlConnection(connstr);
//SqlCommand command = new SqlCommand(sql, connection);
//connection.Open(); 
//　 SqlDataReader dr = command.ExecuteReader(); 
//　 if(dr.Read()) 
//　 { 
//　　 Response.ContentType = dr["imgtype"].ToString();
//Response.BinaryWrite((byte[]) dr["imgdata"] ); 
//　 } 
//　 connection.Close(); 



//圖檔讀資料庫
//程式設計的方式設定圖片https://msdn.microsoft.com/zh-tw/library/t94wdca5(v=vs.110).aspx
//利用outputstream輸出圖檔http://hanchaohan.blog.51cto.com/2996417/922335
//c# 獲取相對路徑http://www.zendei.com/article/6140.html
//c# 獲取相對路徑http://fecbob.pixnet.net/blog/post/38088023-c%23-%E7%8D%B2%E5%8F%96%E7%9B%B8%E5%B0%8D%E8%B7%AF%E5%BE%91
//http://ithelp.ithome.com.tw/articles/10029065
//http://fanli7.net/a/shujuku/20110819/104516.html
//http://www.blueshop.com.tw/board/fum20050124192253inm/BRD20100510190615Q6R.html
//https://support.microsoft.com/zh-tw/help/317701/how-to-copy-a-picture-from-a-database-directly-to-a-picturebox-control


//分割字串
//string item = lBox_user_order_messagebox.SelectedItems.ToString();
//string[] parts = item.Split('=');
//label5.Text = item[0].ToString();
//SumTotal -= int.Parse(item.Split('=')[0]);
//            字串分割範例: String demostr = "微積分,39136001,80";
//            依照逗號分割出第1個子字串:
//            demostr.Split(',')[0]
//依照逗號分割出第2個子字串:
//            demostr.Split(',')[1]
//依照逗號分割出第3個子字串:
//            demostr.Split(',')[2]


//DataGridView的几个基本操作：
//1、获得某个（指定的）单元格的值：
//dataGridView1.Row[i].Cells[j].Value;
//2、获得选中的总行数：
//dataGridView1.SelectedRows.Count;
//3、获得当前选中行的索引：
//dataGridView1.CurrentRow.Index;
//4、获得当前选中单元格的值：
//dataGridView1.CurrentCell.Value;
//5、取选中行的数据
//string[] str = new string[dataGridView.Rows.Count];
//for(int i;i<dataGridView1.Rows.Count;i++)
//{
//if(dataGridView1.Rows[i].Selected == true)
//{
//str[i] = dataGridView1.Rows[i].Cells[1].Value.ToString();
//}
//}
//7、获取选中行的某个数据
//int a = dataGridView1.SelectedRows.Index;
//dataGridView1.Rows[a].Cells["你想要的某一列的索引，想要几就写几"].Value;
