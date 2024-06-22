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
using DoAnQuanLyNoiThat.Controller;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Font = iTextSharp.text.Font;

namespace DoAnQuanLyNoiThat.View
{
    public partial class bangthongke : Form
    {
        private ThongKeController thongKeController;
        public bangthongke()
        {
            InitializeComponent();
            thongKeController = new ThongKeController(dataGridView1);
        }

        private void bangthongke_Load(object sender, EventArgs e)
        {

            dateStart.Value = DateTime.Now;
            dateEnd.Value = DateTime.Now;
            thongKeController.LoadDataGridView();

            // Sau khi LoadDataGridView, tính tổng và cập nhật giá trị trong textBox1 và textBox2
            var totals = thongKeController.CalculateTotals(dataGridView1.DataSource as DataTable);
            textBox1.Text = totals.totalSoLuong.ToString();
            textBox2.Text = totals.totalThanhTien.ToString("N2");
        }
        private void btntim_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = dateStart.Value.Date;
                DateTime endDate = dateEnd.Value.Date.AddSeconds(86399); // Kết thúc vào 00:00:00 của ngày tiếp theo

                if (endDate >= startDate)
                {
                    DataView dv = ((DataTable)dataGridView1.DataSource).DefaultView;
                    dv.RowFilter = $"ngayDatHang >= #{startDate}# AND ngayDatHang <= #{endDate}#";

                    // Sau khi lọc dữ liệu, tính lại tổng số lượng và tổng thành tiền
                    int totalSoLuong = 0;
                    decimal totalThanhTien = 0;

                    foreach (DataRowView rowView in dv)
                    {
                        DataRow row = rowView.Row;
                        totalSoLuong += Convert.ToInt32(row["soLuong"]);
                        totalThanhTien += Convert.ToDecimal(row["thanhTien"]);
                    }

                    // Cập nhật TextBoxs hiển thị tổng số lượng và tổng thành tiền
                    textBox1.Text = totalSoLuong.ToString();
                    textBox2.Text = totalThanhTien.ToString("N2");

                }
                else
                {
                    MessageBox.Show("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm" + ex.Message);
            }
        }
        private void btnPDF_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Output.pdf";
                bool fileError = false;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (File.Exists(sfd.FileName))
                        {
                            File.Delete(sfd.FileName);
                        }

                        using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                        {
                            Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            // Thiết lập font Unicode hỗ trợ tiếng Việt
                            BaseFont bf = BaseFont.CreateFont("c:\\windows\\fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                            // Font cho tiêu đề
                            Font titleFont = new Font(bf, 18);
                            titleFont.SetStyle((Font.Bold).ToString());


                            // Font cho các ô dữ liệu
                            Font cellFont = new Font(bf, 13);

                            // Thêm tiêu đề
                            Paragraph title = new Paragraph("Danh Sách Thống Kê Sản Phẩm", titleFont);
                            title.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(title);
                            pdfDoc.Add(new Paragraph("\n\n\n"));
                            pdfDoc.Add(new Paragraph("Từ Ngày: " + dateStart.Text + "    đến ngày: " + dateEnd.Text, cellFont));


                            // Thêm khoảng trắng giữa tiêu đề và bảng
                            pdfDoc.Add(new Paragraph("\n"));

                            // Thêm bảng dữ liệu
                            PdfPTable pdfTable = new PdfPTable(dataGridView1.Columns.Count);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;

                            // Thêm dòng tiêu đề từ DataGridView
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, cellFont));
                                pdfTable.AddCell(cell);
                            }

                            // Thêm dữ liệu từ DataGridView
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row != null)
                                {
                                    foreach (DataGridViewCell cell in row.Cells)
                                    {
                                        if (cell != null && cell.Value != null)
                                        {
                                            pdfTable.AddCell(new Phrase(cell.Value.ToString(), cellFont));
                                        }
                                        else
                                        {
                                            pdfTable.AddCell(new Phrase("", cellFont)); // Hoặc xử lý nếu cần thiết
                                        }
                                    }
                                }
                            }

                            // Thêm bảng vào tài liệu
                            pdfDoc.Add(pdfTable);
                            // Thêm dòng mới cho TextBox1 và TextBox2
                            pdfDoc.Add(new Paragraph("\n"));
                            pdfDoc.Add(new Paragraph("Tổng Số Lượng Sản Phẩm đã bán: " + textBox1.Text, cellFont));
                            pdfDoc.Add(new Paragraph("Tổng Doanh Thu Cửa Hàng: " + textBox2.Text, cellFont));

                            pdfDoc.Close();

                            MessageBox.Show("Dữ liệu Export thành công!!!", "Info");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Mô tả lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Không có bản ghi nào được Export!!!", "Info");
            }
        }

    }
}
