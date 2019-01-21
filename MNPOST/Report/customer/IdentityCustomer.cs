using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Report.customer
{
    public class IdentityCustomer
    {
    }
    public class CustomerDebitMaster
    {
        public string DocumentID { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public decimal CuocDV { get; set; }
        public decimal GiamTru { get; set; }
        public decimal VAT { get; set; }
        public decimal TongTien {get;set;}
        public string GhiChu { get; set; }
        public string DebtMonth { get; set; }

    }
    public class CustomerDebitDetail
    {
        public string NgayGui { get; set; }
        public string SoPhieu { get; set; }
        public string NoiDen { get; set; }
        public string DichVu { get; set; }
        public int SoLuong { get; set; }
        public double TrongLuong { get; set; }
        public decimal CuocPhi { get; set; }
        public decimal PhuPhi { get; set; }
        public decimal ThanhTien { get; set; }
        public int Group { get; set; }
    }
}