using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spectre.Forms
{
    public partial class frmSearch : Form
    {
        public frmSearch()
        {
            InitializeComponent();
        }
        DataTable tblSV;
        private void SearchTextbox(string header, TextBox textBox)
        {
            string cmd = header + " LIKE '{0}%'";
            (dgvSearch.DataSource as DataTable).DefaultView.RowFilter = string.Format(cmd, textBox.Text);
        }
        public void Load_Search()
        {
            string sql;
            sql = "SELECT maLop,maSV, tenSV, queQuan, ngaySinh, anhSV FROM SinhVien";
            tblSV = frmClass.Functions.GetDataToTable(sql);
            dgvSearch.DataSource = tblSV;
            dgvSearch.Columns[0].HeaderText = "Mã Lớp";
            dgvSearch.Columns[1].HeaderText = "Mã Sinh Viên";
            dgvSearch.Columns[2].HeaderText = "Tên Sinh Viên";
            dgvSearch.Columns[3].HeaderText = "Quê Quán";
            dgvSearch.Columns[4].HeaderText = "Ngày Sinh";
            dgvSearch.Columns[5].HeaderText = "Ảnh Sinh Viên";

            // Không cho phép thêm mới dữ liệu trực tiếp strên lưới
            dgvSearch.AllowUserToAddRows = false;
            // Không cho phép sửa dữ liệu trực tiếp trên lưới
            dgvSearch.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        private void frmSearch_Load(object sender, EventArgs e)
        {
            Load_Search();
            txtStudentPOB2.ReadOnly = true;
            dtpStudentDOB2.Enabled = false;
        }
        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tblSV.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK,
  MessageBoxIcon.Information);
                return;
            }
            txtClassID3.Text = dgvSearch.CurrentRow.Cells["maLop"].Value.ToString();
            txtStudentID2.Text = dgvSearch.CurrentRow.Cells["maSV"].Value.ToString();
            txtStudentName2.Text = dgvSearch.CurrentRow.Cells["tenSV"].Value.ToString();
            dtpStudentDOB2.Value = (DateTime)dgvSearch.CurrentRow.Cells["ngaySinh"].Value;
            txtStudentPOB2.Text = dgvSearch.CurrentRow.Cells["queQuan"].Value.ToString();

            try
            {
                MemoryStream ms = new MemoryStream((byte[])dgvSearch.CurrentRow.Cells["anhSV"].Value);
                picStudent2.Image = Image.FromStream(ms);
            }
            catch
            {
                picStudent2.Image = picStudent2.ErrorImage;
            }
        }
        private void txtStudentID2_TextChanged(object sender, EventArgs e)
        {
            if (txtStudentID2.Focused == true)
            {
                SearchTextbox("maSV", txtStudentID2);
                txtClassID3.Text = null;
                txtStudentName2.Text = null;
            }
        }

        private void txtClassID3_TextChanged(object sender, EventArgs e)
        {
            if (txtClassID3.Focused == true)
            {
                SearchTextbox("maLop", txtClassID3);
                txtStudentID2.Text = null;
                txtStudentName2.Text = null;
            }




        }

        private void txtStudentName2_TextChanged(object sender, EventArgs e)
        {
            if (txtStudentName2.Focused == true)
            {
                SearchTextbox("tenSV", txtStudentName2);
                txtStudentID2.Text = null;
                txtClassID3.Text = null;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtClassID3.Text = "";
            txtStudentID2.Text = "";
            txtStudentName2.Text = "";
            Load_Search();
        }
    }
}
