using DoAnQuanLyNoiThat.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnQuanLyNoiThat.View
{
    public partial class login : Form
    {
        TaiKhoanController taiKhoanController;
        public login()
        {
            InitializeComponent();
            
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
           
            string taikhoan = txtTK.Text;
            string matkhau = txtMk.Text;
            taiKhoanController = new TaiKhoanController();
            if (taiKhoanController.KiemtraDN(taikhoan, matkhau))
            {
                MDI MDI = new MDI();
                MDI.Show();
                this.Hide();
            }
            else
            {            
                    MessageBox.Show("Đăng nhập không thành công. Kiểm tra lại tên đăng nhập và mật khẩu.");
                   
            }

        }
        private bool isPictureBoxClicked = false;
        private void login_Load(object sender, EventArgs e)
        {
            if (isPictureBoxClicked == true)
            {
                txtMk.PasswordChar = '\0'; // Hiển thị mật khẩu
            }
            else
            {
                txtMk.PasswordChar = '*'; // Ẩn mật khẩu
            }
        }

        private void showpw_Click(object sender, EventArgs e)
        {
            isPictureBoxClicked = !isPictureBoxClicked;

            // Update the PasswordChar based on the flag
            if (isPictureBoxClicked == true)
            {
                txtMk.PasswordChar = '\0'; // Hiển thị mật khẩu
            }
            else
            {
                txtMk.PasswordChar = '*'; // Ẩn mật khẩu
            }
        }
    }
}
