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

namespace Spectre.Forms
{
    public partial class frmStudent : Form
    {
        DataTable tblSV;
        public frmStudent()
        {
            InitializeComponent();
        }
        #region Reset
        private void ResetButtons()
        {
            btnAdd2.Enabled = true;
            btnSave2.Enabled = false;
            btnEdit2.Enabled = false;
            btnDelete2.Enabled = false;
            btnSelectImage2.Enabled = false;
            btnAdd2.Visible = true;
            btnEdit2.Visible = true;
            cboClassID.Enabled = false;
            txtStudentID.ReadOnly = true;
            txtImagePath2.ReadOnly = true;
            txtStudentName.ReadOnly = true;
            txtStudentPOB.ReadOnly = true;
            dtpStudentDOB.Enabled = false;

        }
        private void ResetValues()
        {
            cboClassID.Text = "";
            txtImagePath2.Text = "";
            txtStudentID.Text = "";
            txtStudentName.Text = "";
            txtStudentPOB.Text = "";
        }
        #endregion
        #region ButtonStates
        private void AddStateButtons()
        {
            btnEdit2.Enabled = false;
            btnDelete2.Enabled = false;
            btnAdd2.Visible = false;
            btnSave2.Enabled = true;
            btnSelectImage2.Enabled = true;
            cboClassID.Enabled = true;
            txtStudentID.ReadOnly = false;
            txtImagePath2.ReadOnly = false;
            txtStudentName.ReadOnly = false;
            txtStudentPOB.ReadOnly = false;
            dtpStudentDOB.Enabled = true;
        }
        private void UpdateStateButtons()
        {
            btnEdit2.Enabled = false;
            btnDelete2.Enabled = false;
            btnAdd2.Enabled = false;
            btnEdit2.Visible = false;
            btnSave2.Enabled = true;
            btnSelectImage2.Enabled = true;
            cboClassID.Enabled = false;
            txtStudentID.ReadOnly = true;
            txtImagePath2.ReadOnly = false;
            txtStudentName.ReadOnly = false;
            txtStudentPOB.ReadOnly = false;
            dtpStudentDOB.Enabled = true;
        }
        #endregion
        public static void FillCombo(string sql, ComboBox cbo, string ma)
        {
            SqlDataAdapter Mydata = new SqlDataAdapter(sql, frmClass.Functions.Conn);
            DataTable table = new DataTable();
            Mydata.Fill(table);
            cbo.ValueMember = ma;    // Truong gia tri
            cbo.DisplayMember = ma;    // Truong hien thi
            cbo.DataSource = table;
        }

        public void Load_dgvStudent()
        {
            string sql;
            sql = "SELECT maLop, maSV, tenSV,queQuan,ngaySinh,anhSV FROM SinhVien";
            tblSV = frmClass.Functions.GetDataToTable(sql);
            dgvStudent.DataSource = tblSV;
            dgvStudent.Columns[0].HeaderText = "Mã Lớp";
            dgvStudent.Columns[1].HeaderText = "Mã Sinh Viên";
            dgvStudent.Columns[2].HeaderText = "Tên Sinh Viên";
            dgvStudent.Columns[3].HeaderText = "Quê Quán";
            dgvStudent.Columns[4].HeaderText = "Ngày Sinh";
            dgvStudent.Columns[5].HeaderText = "Ảnh Sinh Viên";

            // Không cho phép thêm mới dữ liệu trực tiếp strên lưới
            dgvStudent.AllowUserToAddRows = false;
            // Không cho phép sửa dữ liệu trực tiếp trên lưới
            dgvStudent.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        
        private byte[] ConvertImagetobyte() //ham chuyen doi image thanh byte
        {
            FileStream fs;
            fs = new FileStream(txtImagePath2.Text, FileMode.Open, FileAccess.Read);
            byte[] imagebyte = new byte[fs.Length];
            fs.Read(imagebyte, 0, System.Convert.ToInt32(fs.Length));
            fs.Close();
            return imagebyte;
        }
        private void frmStudent_Load(object sender, EventArgs e)
        {
            Load_dgvStudent();
            FillCombo("SELECT maLop FROM Lop", cboClassID, "maLop");
            cboClassID.SelectedIndex = -1;
            ResetButtons();
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tblSV.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK,
  MessageBoxIcon.Information);
                return;
            }
            cboClassID.Text = dgvStudent.CurrentRow.Cells["maLop"].Value.ToString();
            txtStudentID.Text = dgvStudent.CurrentRow.Cells["maSV"].Value.ToString();
            txtStudentName.Text = dgvStudent.CurrentRow.Cells["tenSV"].Value.ToString();
            dtpStudentDOB.Value = (DateTime)dgvStudent.CurrentRow.Cells["ngaySinh"].Value;
            txtStudentPOB.Text = dgvStudent.CurrentRow.Cells["queQuan"].Value.ToString();

            try
            {
                MemoryStream ms = new MemoryStream((byte[])dgvStudent.CurrentRow.Cells["anhSV"].Value);
                picStudent.Image = Image.FromStream(ms);
            }
            catch
            {
                picStudent.Image = picStudent.ErrorImage;
            }
            btnDelete2.Enabled = true;
            btnEdit2.Enabled = true;
        }

        private void btnAdd2_Click(object sender, EventArgs e)
        {
            AddStateButtons();
            ResetValues();
            txtStudentID.Focus();
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            string sql;
            if (btnAdd2.Visible == false)
            {
                sql = "SELECT maSV FROM SinhVien WHERE maSV=N'" + txtStudentID.Text.Trim() + "'";
                if (txtStudentID.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập mã sinh viên ", "Thông báo",
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStudentID.Focus();
                    return;
                }
                if (txtStudentName.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập tên sinh viên", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStudentName.Focus();
                    return;
                }
                if (cboClassID.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải chọn mã lớp ", "Thông báo",
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboClassID.Focus();
                    return;
                }
                if (frmClass.Functions.CheckKey(sql))
                {
                    MessageBox.Show("Mã sinh viên này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStudentID.Focus();
                    txtStudentID.Text = "";
                    return;
                }
                if (txtImagePath2.Text == "")
                {

                    sql = "INSERT INTO SinhVien (maLop, maSV,tenSV,queQuan,ngaySinh )  VALUES (@maLop, @maSV,@tenSV,@queQuan,@ngaySinh)";
                    SqlCommand com = new SqlCommand(sql, frmClass.Functions.Conn);
                    com.Parameters.AddWithValue("@maLop", cboClassID.Text);
                    com.Parameters.AddWithValue("@maSV", txtStudentID.Text);
                    com.Parameters.AddWithValue("@tenSV", txtStudentName.Text);
                    com.Parameters.AddWithValue("@queQuan", txtStudentPOB.Text);
                    com.Parameters.AddWithValue("@ngaySinh", dtpStudentDOB.Value);
                    com.ExecuteNonQuery();
                    com.Dispose();

                    MessageBox.Show("Thêm thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetValues();
                    ResetButtons();
                    Load_dgvStudent();
                }


                else
                {
                    try
                    {
                        sql = "INSERT INTO SinhVien(maLop, maSV,tenSV,queQuan,ngaySinh ,anhSV)  VALUES (@maLop, @maSV,@tenSV,@queQuan,@ngaySinh ,@anhSV)";
                        SqlCommand com = new SqlCommand(sql, frmClass.Functions.Conn);
                        com.Parameters.AddWithValue("@maLop", cboClassID.Text);
                        com.Parameters.AddWithValue("@maSV", txtStudentID.Text);
                        com.Parameters.AddWithValue("@tenSV", txtStudentName.Text);
                        com.Parameters.AddWithValue("@queQuan", txtStudentPOB.Text);
                        com.Parameters.AddWithValue("@ngaySinh", dtpStudentDOB.Value);
                        com.Parameters.AddWithValue("@anhSV", ConvertImagetobyte());
                        com.ExecuteNonQuery();
                        com.Dispose();
                        com = null;
                        ResetValues();
                        MessageBox.Show("Thêm thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvStudent();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            if (btnEdit2.Visible == false)
            {
                if (txtStudentID.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập mã sinh viên ", "Thông báo",
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStudentID.Focus();
                    return;
                }
                if (txtStudentName.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập tên sinh viên", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStudentName.Focus();
                    return;
                }
                if (cboClassID.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Bạn phải chọn mã lớp ", "Thông báo",
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboClassID.Focus();
                    return;
                }
                try
                {
                    if (txtImagePath2.Text == "")
                    {
                        string sql1 = "UPDATE SinhVien SET tenSV = @tenSV, queQuan = @queQuan, ngaySinh = @ngaySinh Where maSV= N'" + txtStudentID.Text + "'";
                        var cmd1 = new SqlCommand(sql1, frmClass.Functions.Conn);
                        cmd1.Parameters.AddWithValue("@tenSV", txtStudentName.Text);
                        cmd1.Parameters.AddWithValue("@queQuan", txtStudentPOB.Text);
                        cmd1.Parameters.AddWithValue("@ngaySinh", dtpStudentDOB.Value);
                        cmd1.ExecuteNonQuery();
                        cmd1.Dispose();
                        cmd1 = null;
                        ResetValues();
                        MessageBox.Show("Sửa thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvStudent();
                    }
                    else
                    {

                        string sql2 = "UPDATE SinhVien SET tenSV = @tenSV, queQuan = @queQuan, ngaySinh = @ngaySinh, anhSV = @anhSV Where maSV= N'" + txtStudentID.Text + "'"; ;
                        var cmd2 = new SqlCommand(sql2, frmClass.Functions.Conn);
                        cmd2.Parameters.AddWithValue("@tenSV", txtStudentName.Text);
                        cmd2.Parameters.AddWithValue("@queQuan", txtStudentPOB.Text);
                        cmd2.Parameters.AddWithValue("@ngaySinh", dtpStudentDOB.Value);
                        cmd2.Parameters.AddWithValue("@anhSV", ConvertImagetobyte());
                        cmd2.ExecuteNonQuery();
                        cmd2.Dispose();
                        cmd2 = null;
                        ResetValues();
                        MessageBox.Show("Sửa thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetButtons();
                        Load_dgvStudent();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        private void btnEdit2_Click(object sender, EventArgs e)
        {
            UpdateStateButtons();
            txtStudentName.Focus();
        }

        private void btnDelete2_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblSV.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu!", "Thông báo", MessageBoxButtons.OK,
MessageBoxIcon.Information);
                return;
            }
            if (txtStudentID.Text == "")
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
                    sql = "DELETE SinhVien WHERE maSV=N'" + txtStudentID.Text + "'";
                    var cmd3 = new SqlCommand(sql, frmClass.Functions.Conn);
                    cmd3.ExecuteNonQuery();
                    cmd3.Dispose();
                    cmd3 = null;
                    Load_dgvStudent();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Forms.frmSearch f2 = new Forms.frmSearch();
            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void btnSelectImage2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = openFile.Filter = "JPG file(*.jpg)|*.jpg|All file (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                picStudent.ImageLocation = openFile.FileName; //cho phep chon 1 anh
                txtImagePath2.Text = openFile.FileName;
            }
        }
    }
}
