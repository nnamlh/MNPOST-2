using MNPOSTAPI.Models;
using MNPOSTCOMMON;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace MNPOSTAPI.Controllers.web
{

    public class CustomerController : WebBaseController
    {


        [HttpPost]
        public ResultInfo AddCustomer()
        {
            ResponseInfo result = new ResponseInfo()
            {
                error = 0,
                msg = "Them moi thanh cong"
            };

            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;
                logger.Info(requestContent);
                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<AddCustomerRequest>(requestContent);

                MailerHandleCommon handle = new MailerHandleCommon(db);

                var groups = new BS_CustomerGroups()
                {
                    IsActive = true,
                    ConatctPhone = paser.phone,
                    ContactEmail = paser.email,
                    CreationDate = DateTime.Now,
                    CustomerGroupID = Guid.NewGuid().ToString(),
                    CustomerGroupCode = handle.GeneralCusGroupCode(),
                    PaymentMethodID = "money",
                    CustomerGroupName = paser.fullName
                };
                db.BS_CustomerGroups.Add(groups);

                db.SaveChanges();

                // customer 

                var code = handle.GeneralCusCode(groups.CustomerGroupCode);
                var ins = new BS_Customers()
                {
                    CustomerID = Guid.NewGuid().ToString(),
                    CustomerName = paser.fullName,
                    CountryID = "VN",
                    Address = "",
                    CreateDate = DateTime.Now,
                    CustomerCode = code,
                    CustomerGroupID = groups.CustomerGroupID,
                    Deputy = paser.fullName,
                    DistrictID = "",
                    Email = paser.email,
                    IsActive = true,
                    Phone = paser.phone,
                    PostOfficeID = "BCQ3",
                    ProvinceID = "",
                    ClientUser = paser.clientUser,
                    WardID = ""
                };

                db.BS_Customers.Add(ins);

                db.SaveChanges();

                result.data = code;

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }

            return result;
        }

        [HttpGet]
        public ResultInfo CustomerInfo(string cusId)
        {
            var data = db.BS_Customers.Where(p => p.CustomerCode == cusId).Select(p => new CustomerInfo()
            {
                Address = p.Address,
                CountryID = p.CountryID,
                CustomerCode = p.CustomerCode,
                CustomerID = p.CustomerID,
                CustomerName = p.CustomerName,
                Deputy = p.Deputy,
                DistrictID = p.DistrictID,
                Email = p.Email,
                Phone = p.Phone,
                ProvinceID = p.ProvinceID,
                WardID = p.WardID
            }).FirstOrDefault();

            if (data == null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không tìm thấy"
                };
            }

            return new ResponseInfo()
            {
                data = data,
                error = 0,
                msg = ""
            };

        }


        [HttpPost]
        public ResultInfo UpdateCustomer()
        {
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "Cap nhat thanh cong"
            };

            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;

                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<CustomerInfo>(requestContent);

                var check = db.BS_Customers.Find(paser.CustomerID);

                if (check == null)
                    throw new Exception("Thông tin sai");

                check.CustomerName = paser.CustomerName;
                check.ProvinceID = paser.ProvinceID;
                check.DistrictID = paser.DistrictID;
                check.WardID = paser.WardID;
                check.Phone = paser.Phone;
                check.Email = paser.Email;
                check.Deputy = paser.Deputy;
                check.Address = paser.Address;

                db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;
        }
       
    }

}
