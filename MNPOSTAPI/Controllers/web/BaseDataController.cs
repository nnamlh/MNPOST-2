using MNPOSTAPI.Models;
using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNPOSTAPI.Controllers.web
{
    public class BaseDataController : ApiController
    {
        MNPOSTEntities db = new MNPOSTEntities();
        // get province
        [HttpGet]
        public List<ItemCommon> GetProvince()
        {
            var data = db.BS_Provinces.Select(p => new ItemCommon
            {
                code = p.ProvinceID,
                name = p.ProvinceName
            }).ToList();

            return data;
        }

        [HttpGet]
        public List<ItemCommon> GetDistrict(string provinceID)
        {
            var data = db.BS_Districts.Where(p => p.ProvinceID == provinceID).Select(p => new ItemCommon
            {
                code = p.DistrictID,
                name = p.DistrictName
            }).ToList();

            return data;
        }

        [HttpGet]
        public List<ItemCommon> GetWard(string districtID)
        {
            var data = db.BS_Wards.Where(p => p.DistrictID == districtID).Select(p => new ItemCommon
            {
                code = p.WardID,
                name = p.WardName
            }).ToList();

            return data;
        }

        //
        [HttpGet]
        public ResultInfo CheckCus(string email, string phone)
        {
            var data = db.BS_Customers.Where(p => p.Email == email || p.Phone == phone).FirstOrDefault();

            if (data != null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Đã tồn tại"
                };
            }

            return new ResultInfo()
            {
                error = 0,
                msg = ""
            };
        }


        [HttpGet]
        public ResultInfo GetCustomerAddress(string cusId)
        {
            var data = db.BS_Customers.Where(p => p.CustomerCode == cusId).FirstOrDefault();

            if (data != null)
            {
                return new ResponseInfo
                {
                    error = 1,
                    msg = "",
                    data = new
                    {
                        address = data.Address,
                        phone = data.Phone,
                        name = data.CustomerName,
                        provinceId = data.ProvinceID,
                        districtId = data.DistrictID,
                        wardId = data.WardID
                    }
                };
            }
            else
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = ""
                };
            }

        }


        [HttpGet]
        public ResultInfo GetMailerSetting()
        {
            // dịch vu

            var MailerTypes = db.BS_ServiceTypes.Select(p => new ItemCommon()
            {
                code = p.ServiceID,
                name = p.ServiceName
            }).ToList();

            // hinh thuc thanh toan
            var Payments = db.CDatas.Where(p => p.CType == "MAILERPAY").Select(p => new ItemCommon() { code = p.Code, name = p.Name }).ToList();

            return new ResponseInfo()
            {
                data= new
                {
                    MailerTypes = MailerTypes,
                    Payments = Payments
                },
                error = 0
            };

        }


        [HttpGet]
        public ResultInfo CalBillPrice(float weight = 0, string customerId = "", string provinceId = "", string serviceTypeId = "")
        {
            var findCus = db.BS_Customers.Where(p => p.CustomerCode == customerId).FirstOrDefault();
            decimal? price = 0;
            if (findCus != null)
            {
               price = db.CalPrice(weight, findCus.CustomerID, provinceId, serviceTypeId, "BCQ3", DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
            }

         

            return new ResponseInfo()
            {
                error = 0,
                data = new
                {
                    price = price,
                    codPrice = 0
                }
            };
        }

    }
}
