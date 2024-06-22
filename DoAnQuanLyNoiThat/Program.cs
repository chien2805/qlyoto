using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoAnQuanLyNoiThat.View;

namespace DoAnQuanLyNoiThat
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DatabaseHelper.dbName = "QuanLyOto";
            DatabaseHelper.serverName = "LAPTOP-V2UK0679\\SQLEXPRESS";
            DatabaseHelper.userDb = "sa001";
            DatabaseHelper.password = "1";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login());
        }
    }
}
