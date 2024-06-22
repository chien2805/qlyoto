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
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.SqlServer.Server;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DoAnQuanLyNoiThat.View;

namespace DoAnQuanLyNoiThat.Controller
{
    internal class ThongKeController
    {
        private DataGridView dataGridView;
        public ThongKeController(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
        }
        public void LoadDataGridView()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.getConnection())
                {
                    conn.Open();
                    string sql = "SELECT  Sanpham.TenSanPham, Chitiet.SoLuong, Chitiet.DonGia, Donhang.NgayDatHang FROM ChiTietDonHang Chitiet " +
                                 "INNER JOIN SanPham Sanpham ON Chitiet.IdSanPham = Sanpham.IdSanPham " +
                                 "INNER JOIN DonHang Donhang ON Chitiet.IdDonHang = Donhang.IdDonHang";

                    SqlCommand com = new SqlCommand(sql, conn);
                    com.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    conn.Close();

                    // Tính cột Thành Tiền và thêm nó vào DataTable
                    dt.Columns.Add("ThanhTien", typeof(decimal));
                    foreach (DataRow row in dt.Rows)
                    {
                        int soLuong = Convert.ToInt32(row["SoLuong"]);
                        decimal donGia = Convert.ToDecimal(row["DonGia"]);
                        decimal thanhTien = soLuong * donGia;
                        row["ThanhTien"] = thanhTien;
                    }

                    // Hiển thị dữ liệu trong DataGridView
                    dataGridView.DataSource = dt;

                    CalculateTotals(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu vào DataGridView: " + ex.Message);
            }
        }

        public (int totalSoLuong, decimal totalThanhTien) CalculateTotals(DataTable dt)
        {
            int totalSoLuong = 0;
            decimal totalThanhTien = 0;

           // Duyệt qua các dòng trong DataTable và tính tổng số lượng và tổng thành tiền
            foreach (DataRow row in dt.Rows)
            {
                totalSoLuong += Convert.ToInt32(row["soLuong"]);
                totalThanhTien += Convert.ToDecimal(row["thanhTien"]);
            }

            return (totalSoLuong, totalThanhTien);
        }





    }
}
