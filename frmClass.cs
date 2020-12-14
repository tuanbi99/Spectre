using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spectre
{
    public partial class frmClass : Form
    {
        DataTable tblLop;
        public frmClass()
        {
            InitializeComponent();
        }
        public class Functions
        {
            public static SqlConnection Conn;  //Khai bao doi tuong ket noi
            public static string connString; //Khai bao bien chua chuoi ket noi
            public static void Connect()
            {
                connString = (@"Data Source=LAP-KAO\WINCC;Initial Catalog=SV;Integrated Security=True");
                Conn = new SqlConnection(); // Cap phat doi tuong
                Conn.ConnectionString = connString; // Ket noi
                Conn.Open(); // Mo ket noi
            }
            public static void Disconnect()
            {

                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close(); // Dong ket noi
                    Conn.Dispose(); // Giai phong tai nguyen
                }

            }
            public static DataTable GetDataToTable(string sql)
            {
                SqlDataAdapter Mydata = new SqlDataAdapter(sql, Conn);
                DataTable table = new DataTable();
                Mydata.Fill(table);
                return table;
            }
            public static bool CheckKey(string sql) //Kiem tra khoa trung
            {
                SqlDataAdapter Mydata = new SqlDataAdapter(sql, Conn);
                DataTable table = new DataTable();
                Mydata.Fill(table);
                if (table.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
        }
        #region Reset
        private void ResetButtons()
        {
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnSelectImage.Enabled = false;
            btnAdd.Visible = true;
            btnEdit.Visible = true;
            txtClassID.ReadOnly = true;
            txtClassName.ReadOnly = true;
            txtImagePath.ReadOnly = true;
        }
        private void ResetValues()
        {
            txtClassID.Text = "";
            txtClassName.Text = "";
            txtImagePath.Text = "";
        }
        #endregion
        #region ButtonStates
        private void AddStateButtons()
        {
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnAdd.Visible = false;
            btnSave.Enabled = true;
            btnSelectImage.Enabled = true;
            txtClassID.ReadOnly = false;
            txtClassName.ReadOnly = false;
            txtImagePath.ReadOnly = false;
        }
        private void UpdateStateButtons()
        {
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnAdd.Enabled = false;
            btnEdit.Visible = false;
            btnSave.Enabled = true;
            btnSelectImage.Enabled = true;
            txtClassID.ReadOnly = true;
            txtClassName.ReadOnly = false;
            txtImagePath.ReadOnly = false;
        }
        #endregion
        
        private byte[] ConvertImagetobyte() //ham chuyen doi image thanh byte
        {
            FileStream fs;
            fs = new FileStream(txtImagePath.Text, FileMode.Open, FileAccess.Read);
            byte[] imagebyte = new byte[fs.Length];
            fs.Read(imagebyte, 0, System.Convert.ToInt32(fs.Length));
            fs.Close();
            return imagebyte;
        }
        public void Load_dgvClass()
        {
            string sql;
            sql = "SELECT maLop, tenLop, anhLop FROM Lop";
            tblLop = Functions.GetDataToTable(sql);
            dgvClass.DataSource = tblLop;
            dgvClass.Columns[0].HeaderText = "Mã Lớp";
            dgvClass.Columns[1].HeaderText = "Tên Lớp";
            dgvClass.Columns[2].HeaderText = "Ảnh lớp";

            // Không cho phép thêm mới dữ liệu trực tiếp strên lưới
            dgvClass.AllowUserToAddRows = false;
            // Không cho phép sửa dữ liệu trực tiếp trên lưới
            dgvClass.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        #region OpenClosingForm
        private void frmClass_Load(object sender, EventArgs e)
        {
            Functions.Connect();
            Load_dgvClass();
            ResetButtons();
        }
        private void frmClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            Functions.Disconnect();
            Application.Exit();
        }
        #endregion
        private void dgvClass_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (btnAdd.Visible == false)
            {
                MessageBox.Show("Đang ở chế độ thêm mới!", "Thông báo",
     MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtClassID.Focus();
                return;
            }
            if (btnEdit.Visible == false)
            {
                MessageBox.Show("Đang ở chế độ sửa !", "Thông báo",
     MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtClassID.Focus();
                return;
            }
            if (tblLop.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK,
  MessageBoxIcon.Information);
                return;
            }
            txtClassID.Text = dgvClass.CurrentRow.Cells["maLop"].Value.ToString();
            txtClassName.Text = dgvClass.CurrentRow.Cells["tenLop"].Value.ToString();
            try
            {
                MemoryStream ms = new MemoryStream((byte[])dgvClass.CurrentRow.Cells["anhLop"].Value);
                picClass.Image = Image.FromStream(ms);
            }
            catch
            {
                picClass.Image = picClass.ErrorImage;
            }
            btnDelete.Enabled = true;
            btnEdit.Enabled = true;
        }
        #region Buttons
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddStateButtons();
            ResetValues();
            txtClassID.Focus();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string sql;
            if (btnAdd.Visible == false)
            {
                sql = "SELECT maLop FROM Lop WHERE maLop=N'" + txtClassID.Text.Trim() + "'";
                if (txtClassID.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập mã lớp ", "Thông báo",
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClassID.Focus();
                    return;
                }
                if (txtClassName.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập tên lớp", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClassName.Focus();
                    return;
                }
                if (frmClass.Functions.CheckKey(sql))
                {
                    MessageBox.Show("Mã lớp này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClassID.Focus();
                    txtClassID.Text = "";
                    return;
                }
                if (txtImagePath.Text == "")
                {
                    
                        sql = "INSERT INTO Lop(maLop, tenLop)  VALUES (@maLop,@tenLop)";
                        SqlCommand com = new SqlCommand(sql, Functions.Conn);
                        com.Parameters.AddWithValue("@maLop", txtClassID.Text);
                        com.Parameters.AddWithValue("@tenLop", txtClassName.Text);
                        com.ExecuteNonQuery();
                        com.Dispose();
                        com = null;
                        ResetValues();
                        MessageBox.Show("Thêm thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvClass();
                    
                    
                }
                else
                {
                    try
                    {
                        sql = "INSERT INTO Lop(maLop, tenLop, anhLop)  VALUES (@maLop,@tenLop,@anhLop)";
                        SqlCommand com = new SqlCommand(sql, Functions.Conn);
                        com.Parameters.AddWithValue("@maLop", txtClassID.Text);
                        com.Parameters.AddWithValue("@tenLop", txtClassName.Text);
                        com.Parameters.AddWithValue("@anhLop", ConvertImagetobyte());
                        com.ExecuteNonQuery();
                        com.Dispose();
                        com = null;
                        ResetValues();
                        MessageBox.Show("Thêm thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvClass();
                    }
                    catch
                    {
                        picClass.Image = picClass.ErrorImage;
                    }
                }
            }
            if (btnEdit.Visible == false)
            {
                try
                {
                    if (txtClassName.Text.Trim().Length == 0)
                    {
                        MessageBox.Show("Bạn phải nhập tên lớp", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtClassName.Focus();
                        return;
                    }
                    if (txtImagePath.Text == "")
                    {
                        
                        string sql1 = "UPDATE Lop SET tenLop = @tenLop Where maLop= N'" + txtClassID.Text + "'";
                        var cmd1 = new SqlCommand(sql1, Functions.Conn);
                        cmd1.Parameters.AddWithValue("@tenLop", txtClassName.Text);
                        cmd1.ExecuteNonQuery();
                        cmd1.Dispose();
                        cmd1 = null;
                        ResetValues();
                        MessageBox.Show("Sửa thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvClass();
                    }
                    else
                    {
                        string sql2 = "UPDATE Lop SET tenLop = @tenLop , anhLop = @anhLop Where maLop= N'" + txtClassID.Text + "'"; ;
                        var cmd2 = new SqlCommand(sql2, Functions.Conn);
                        cmd2.Parameters.AddWithValue("@tenLop", txtClassName.Text);
                        cmd2.Parameters.AddWithValue("@anhLop", ConvertImagetobyte());
                        cmd2.ExecuteNonQuery();
                        cmd2.Dispose();
                        cmd2 = null;
                        ResetValues();
                        MessageBox.Show("Sửa thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvClass();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = openFile.Filter = "JPG file(*.jpg)|*.jpg|All file (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                picClass.ImageLocation = openFile.FileName; //cho phep chon 1 anh
                txtImagePath.Text = openFile.FileName;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            UpdateStateButtons();
            txtClassName.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblLop.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu!", "Thông báo", MessageBoxButtons.OK,
MessageBoxIcon.Information);
                return;
            }
            if (txtClassID.Text == "")
            {
                MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo",
MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có muốn xóa không?", "Thông báo",
MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    sql = "DELETE Lop WHERE maLop=N'" + txtClassID.Text + "'";
                    var cmd3 = new SqlCommand(sql, Functions.Conn);
                    cmd3.ExecuteNonQuery();
                    cmd3.Dispose();
                    cmd3 = null;
                    Load_dgvClass();
                    MessageBox.Show("Xoá thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetButtons();
                    ResetValues();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void btnDBStudent_Click(object sender, EventArgs e)
        {
            Forms.frmStudent f = new Forms.frmStudent();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }
        #endregion
    }
}
