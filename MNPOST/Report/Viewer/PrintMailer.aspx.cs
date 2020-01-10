using CrystalDecisions.CrystalReports.Engine;
using MNPOST.Models;
using MNPOSTCOMMON;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace MNPOST.Report.Viewer
{

    public partial class PrintMailer : System.Web.UI.Page
    {
        MNPOSTEntities db = new MNPOSTEntities();
        ReportDocument rptH;
        string searchText = "";
        List<ListItem> provinces = null;
        List<MailerRpt> mailers = new List<MailerRpt>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnxem.Enabled = true;
                searchText = Request.QueryString["post"];

                var custoomers = db.BS_Customers.Where(p => p.IsActive == true && p.PostOfficeID == searchText).ToList();

                foreach (var item in custoomers)
                {
                    cbsender.Items.Add(new ListItem(item.CustomerName, item.CustomerCode));
                }

                var prodata = db.BS_Provinces.ToList();
                provinces = new List<ListItem>();
                foreach (var item in prodata)
                {
                    provinces.Add(new ListItem(item.ProvinceName, item.ProvinceID));
                }

                cbProvince.Items.AddRange(provinces.ToArray());

                
                if(custoomers.Count > 0)
                {
                    this.changeCus(custoomers[0].CustomerCode);
                }
            } else
            {
                if (Session["report"] != null)
                {
                    MailerRptViewer.ReportSource = (ReportDocument)Session["report"];
                }
            }


        }

        private List<string> GetPostPermit(string lv, string currentPost)
        {
            List<string> result = new List<string>();

            // toan
            if (lv == "HOST")
                return db.BS_PostOffices.Select(p => p.PostOfficeID).ToList();


            // local
            if (lv == "LOCAL")
            {
                result.Add(currentPost);
                return result;
            }

            // option
            return db.UserPostOptions.Where(p => p.TUser == User.Identity.Name).Select(p => p.TPostId).ToList();
        }

        private void LoadReport(List<MailerRpt> mailers)
        {
            if (rptH != null)
            {
                rptH.Close();
                rptH.Dispose();
            }

            rptH = new ReportDocument();
            string reportPath = Server.MapPath("~/Report/MailerPrint.rpt");
            rptH.Load(reportPath);

            rptH.SetDataSource(mailers);


            MailerRptViewer.ReportSource = rptH;
            Session.Add("report", rptH);
        }


        protected void CrystalReportViewer1_Init(object sender, EventArgs e)
        {

        }

        protected void btnxem_Click(object sender, EventArgs e)
        {
            if (FileUploadControl.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUploadControl.FileName);
                   

                    MailerHandleCommon mailerHandle = new MailerHandleCommon(db);
                    mailers = new List<MailerRpt>();

                    // var findVSVX = db.BS_Services.Where(p => p.ServiceID == "VSVX").FirstOrDefault();
                    var allService = db.BS_Services.Select(p => new ItemPriceCommon()
                    {
                        code = p.ServiceID,
                        name = p.ServiceName,
                        price = p.Price,
                        choose = false,
                        percent = p.IsPercent

                    }).ToList();
                    // check sender
                    var checkSender = db.BS_Customers.Where(p => p.CustomerCode == cbsender.SelectedItem.Value).FirstOrDefault();

                    if (checkSender == null)
                        throw new Exception("Sai thông tin người gửi");

                    var checkSendProvince = db.BS_Provinces.Find(cbProvince.SelectedItem.Value);

                    if (checkSendProvince == null)
                        throw new Exception("Sai thông tin tỉnh thành");

                    var checkSendDistrict = db.BS_Districts.Find(cbDistrict.SelectedItem.Value);
                    if (checkSendDistrict == null)
                        throw new Exception("Sai thông tin quận huyện ");

                    string extension = System.IO.Path.GetExtension(filename);


                    if (extension.Equals(".xlsx") || extension.Equals(".xls"))
                    {
                        string fileSave = "mailersupload" + DateTime.Now.ToString("ddMMyyyyhhmmss") + extension;
                        string path = Server.MapPath("~/Temps/" + fileSave);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        FileUploadControl.SaveAs(path);
                  
                        FileInfo newFile = new FileInfo(path);
                        var package = new ExcelPackage(newFile);

                        ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                        int totalRows = sheet.Dimension.End.Row;
                        int totalCols = sheet.Dimension.End.Column;

                        // 
                        int mailerCodeIdx = -1;
                        int receiverIdx = -1;
                        int receiPhoneIdx = -1;
                        int receiAddressIdx = -1;
                        int receiProvinceIdx = -1;
                        int receiDistrictIdx = -1;
                        int mailerTypeIdx = -1;
                        int payTypeIdx = -1;
                        int codIdx = -1;
                        int merchandiseIdx = -1;
                        int weigthIdx = -1;
                        int quantityIdx = -1;
                        int notesIdx = -1;
                        int desIdx = -1;
                        int vsvxIdx = -1;

                        // lay index col tren excel
                        for (int i = 0; i < totalCols; i++)
                        {
                            var colValue = Convert.ToString(sheet.Cells[1, i + 1].Value).Trim();

                            Regex regex = new Regex(@"\((.*?)\)");
                            Match match = regex.Match(colValue);

                            if (match.Success)
                            {
                                string key = match.Groups[1].Value;

                                switch (key)
                                {
                                    case "1":
                                        mailerCodeIdx = i + 1;
                                        break;
                                    case "2":
                                        receiverIdx = i + 1;
                                        break;
                                    case "3":
                                        receiPhoneIdx = i + 1;
                                        break;
                                    case "4":
                                        receiAddressIdx = i + 1;
                                        break;
                                    case "5":
                                        receiProvinceIdx = i + 1;
                                        break;
                                    case "6":
                                        receiDistrictIdx = i + 1;
                                        break;
                                    case "8":
                                        mailerTypeIdx = i + 1;
                                        break;
                                    case "9":
                                        payTypeIdx = i + 1;
                                        break;
                                    case "10":
                                        codIdx = i + 1;
                                        break;
                                    case "11":
                                        merchandiseIdx = i + 1;
                                        break;
                                    case "12":
                                        weigthIdx = i + 1;
                                        break;
                                    case "13":
                                        quantityIdx = i + 1;
                                        break;
                                    case "17":
                                        notesIdx = i + 1;
                                        break;
                                    case "18":
                                        desIdx = i + 1;
                                        break;
                                    case "14":
                                        vsvxIdx = i + 1;
                                        break;
                                }

                            }
                        }

                        // check cac gia tri can
                        if (receiverIdx == -1 || receiAddressIdx == -1 || receiPhoneIdx == -1 || receiProvinceIdx == -1 || weigthIdx == -1)
                            throw new Exception("Thiếu các cột cần thiết");
                        //

                        for (int i = 2; i <= totalRows; i++)
                        {
                            string mailerId = mailerCodeIdx == -1 ? mailerHandle.GeneralMailerCode(searchText) : Convert.ToString(sheet.Cells[i, mailerCodeIdx].Value);
                            if (String.IsNullOrEmpty(mailerId))
                                mailerId = mailerHandle.GeneralMailerCode(searchText);
                            //
                            string receiverPhone = Convert.ToString(sheet.Cells[i, receiPhoneIdx].Value);
                            if (String.IsNullOrEmpty(receiverPhone))
                                throw new Exception("Dòng " + (i) + " cột " + receiPhoneIdx + " : thiếu thông tin");

                            //
                            string receiver = Convert.ToString(sheet.Cells[i, receiverIdx].Value);
                            if (String.IsNullOrEmpty(receiver))
                                throw new Exception("Dòng " + (i) + " cột " + receiverIdx + " : thiếu thông tin");
                            //
                            string receiverAddress = Convert.ToString(sheet.Cells[i, receiAddressIdx].Value);
                            if (String.IsNullOrEmpty(receiverAddress))
                                throw new Exception("Dòng " + (i) + " cột " + receiAddressIdx + " : thiếu thông tin");
                            // 
                            string receiverProvince = receiProvinceIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, receiProvinceIdx].Value);
                            var checkProvince = db.BS_Provinces.Where(p => p.ProvinceCode == receiverProvince).FirstOrDefault();


                            //
                            string receiverDistrict = receiDistrictIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, receiDistrictIdx].Value);
                            var receiverDistrictSplit = receiverDistrict.Split('-');
                            var checkDistrict = db.BS_Districts.Find(receiverDistrictSplit[0]);

                            string mailerType = Convert.ToString(sheet.Cells[i, mailerTypeIdx].Value);
                            var checkMailerType = db.BS_ServiceTypes.Find(mailerType);

                            //
                            var mailerPay = payTypeIdx == -1 ? "NGTT" : Convert.ToString(sheet.Cells[i, payTypeIdx].Value);
                            if (payTypeIdx != -1)
                            {
                                var checkMailerPay = db.CDatas.Where(p => p.Code == mailerPay && p.CType == "MAILERPAY").FirstOrDefault();
                                mailerPay = checkMailerPay == null ? "NGTT" : checkMailerPay.Code;
                            }

                            // COD
                            var codValue = sheet.Cells[i, codIdx].Value;
                            decimal cod = 0;
                            if (codValue != null)
                            {
                                var isCodeNumber = codIdx == -1 ? false : Regex.IsMatch(codValue.ToString(), @"^\d+$");
                                cod = isCodeNumber ? Convert.ToDecimal(codValue) : 0;
                            }

                            // hang hoa
                            var merchandisType = Convert.ToString(sheet.Cells[i, merchandiseIdx].Value);
                            var checkMerchandisType = db.CDatas.Where(p => p.Code == merchandisType && p.CType == "GOODTYPE").FirstOrDefault();
                            if (checkMerchandisType == null)
                                throw new Exception("Dòng " + (i) + " cột " + merchandiseIdx + " : sai thông tin");

                            // trong luong
                            var weightValue = sheet.Cells[i, weigthIdx].Value;
                            double weight = 0;
                            if (weightValue == null)
                                throw new Exception("Dòng " + (i) + " cột " + weigthIdx + " : sai thông tin");
                            else
                            {
                                var isWeightNumber = Regex.IsMatch(weightValue.ToString(), @"^\d+$");
                                weight = isWeightNumber ? Convert.ToDouble(sheet.Cells[i, weigthIdx].Value) : 0;
                            }

                            // so luong
                            var quantityValue = sheet.Cells[i, quantityIdx].Value;
                            var isQuantityNumber = quantityIdx == -1 ? false : Regex.IsMatch(quantityValue == null ? "0" : quantityValue.ToString(), @"^\d+$");
                            var quantity = isQuantityNumber ? Convert.ToInt32(quantityValue) : 0;
                            //
                            string notes = notesIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, notesIdx].Value);

                            //
                            string describe = desIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, desIdx].Value);
                            string vsvs = vsvxIdx == -1 ? "N" : Convert.ToString(sheet.Cells[i, vsvxIdx].Value);
                            decimal? price = 0;

                            if (cod > 0)
                            {

                                price = db.CalPriceCOD(weight, checkSender.CustomerCode, checkProvince.ProvinceID, "CD", searchText, DateTime.Now.ToString("yyyy-MM-dd"), vsvs == "N" ? 0 : 1, checkMailerType.ServiceID == "ST" ? "CODTK" : "CODN").FirstOrDefault();
                            }
                            else
                            {
                                price = db.CalPrice(weight, checkSender.CustomerCode, checkProvince.ProvinceID, checkMailerType.ServiceID, searchText, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
                            }

                            var codPrice = 0;

                            var services = new List<ItemPriceCommon>(allService);
                            decimal? priceService = 0;
                            if (vsvs == "Y" && cod == 0)
                            {
                                services.Where(p => p.code == "VSVX").FirstOrDefault().choose = true;
                                var serviceVSVX = services.Where(p => p.code == "VSVX").FirstOrDefault();

                                if (serviceVSVX.percent == true)
                                {
                                    priceService = (price * serviceVSVX.price) / 100;
                                }
                                else
                                {
                                    priceService = serviceVSVX.price;
                                }
                            }

                            mailers.Add(new MailerRpt()
                            {
                                MailerID = mailerId,
                                SenderID = checkSender.CustomerCode,
                                SenderAddress = "Địa chỉ: " + txtAddress.Text,
                                SenderDistrictID = cbDistrict.SelectedValue,
                                SenderDistrictName = cbDistrict.SelectedItem.Text,
                                SenderProvinceName = cbProvince.SelectedItem.Text,
                                ReceiverDistrictName = checkDistrict != null ? checkDistrict.DistrictName : "",
                                ReceiverProvinceName = checkProvince != null ? checkProvince.ProvinceName : "",
                                SenderName = checkSender.CustomerName,
                                SenderPhone = txtPhone.Text,
                                SenderProvinceID = cbProvince.SelectedValue,
                                RecieverAddress = "Địa chỉ: " +  receiverAddress,
                                RecieverDistrictID = checkDistrict != null ? checkDistrict.DistrictID : "",
                                RecieverName = receiver,
                                RecieverPhone = receiverPhone,
                                RecieverProvinceID = checkProvince != null ? checkProvince.ProvinceID : "",
                                PostOfficeName = searchText,
                                COD = cod.ToString("C", MNPOST.Utils.Cultures.VietNam),
                                Weight = weight.ToString(),
                                Quantity = quantity.ToString(),
                                Amount =(price + priceService).Value.ToString("C", MNPOST.Utils.Cultures.VietNam),
                                Price = price.Value.ToString("C", MNPOST.Utils.Cultures.VietNam),
                                ServicePrice = priceService.Value.ToString("C", MNPOST.Utils.Cultures.VietNam)
                            });

                        }
                        // xoa file temp
                        package.Dispose();
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }


                        LoadReport(mailers);
                    }

                    

                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Không có file để upload');", true);
            }
        }

        protected void cbsender_SelectedIndexChanged(object sender, EventArgs e)
        {
            var customerID = cbsender.SelectedItem.Value;

            this.changeCus(customerID);
        }

        private void changeCus(string cusID)
        {
            var findCus = db.BS_Customers.Where(p => p.CustomerCode == cusID).FirstOrDefault();

            if (findCus != null)
            {
                btnxem.Enabled = true;

                txtPhone.Text = findCus.Phone;
                txtAddress.Text = findCus.Address;

                if (!String.IsNullOrEmpty(findCus.ProvinceID))
                {
                    cbProvince.SelectedValue = findCus.ProvinceID;

                    var findAllDis = db.BS_Districts.Where(p => p.ProvinceID == findCus.ProvinceID).ToList();
                    this.cbDistrict.Items.Clear();
                    foreach (var item in findAllDis)
                    {
                        cbDistrict.Items.Add(new ListItem(item.DistrictName, item.DistrictID));
                    }

                    if(!String.IsNullOrEmpty(findCus.DistrictID))
                    {
                        cbDistrict.SelectedValue = findCus.DistrictID;
                    }
                }


            }
            else
            {
                btnxem.Enabled = false;
            }
        }

        protected void cbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            var findAllDis = db.BS_Districts.Where(p => p.ProvinceID == cbProvince.SelectedItem.Value).ToList();

            this.cbDistrict.Items.Clear();
            foreach (var item in findAllDis)
            {
                cbDistrict.Items.Add(new ListItem(item.DistrictName, item.DistrictID));
            }
        }
    }
}
 