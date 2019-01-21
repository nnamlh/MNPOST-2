using MNPOSTAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MNPOSTAPI.Controllers.mobile.mailer
{
    public class MailerAPIController : BaseMobileController
    {

        MailerPresenter presenter = new MailerPresenter();


        [HttpGet]
        public ResultInfo GetDeliveryByEmployee(string employeeId)
        {

            var result = presenter.GetListDelivery(employeeId);



            return result;

        }

        [HttpGet]
        public ResultInfo GetDeliveryEmployeeHistory(string employeeId, string date)
        {
            return presenter.GetListHistoryDelivery(employeeId, date);
        }

        [HttpGet]
        public ResultInfo GetReturnReasons()
        {

            return presenter.GetListReturnReason();
        }

        [HttpPost]
        public ResultInfo UpdateDelivery()
        {

            var result = new ResultInfo()
            {
                error = 0,
                msg = "success"
            };

            try
            {
          
                var requestContent = Request.Content.ReadAsStringAsync().Result;
                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<UpdateDeliveryReceive>(requestContent);

                var user = User.Identity.Name;

                result = presenter.UpdateDelivery(paser, user);

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;
        }


        ///lay hang
        ///
        [HttpGet]
        public ResultInfo GetTakeMailerInDay()
        {
            var user = User.Identity.Name;
            var data = presenter.GetTakeMailer(user);

            return data;
        }

        [HttpGet]
        public ResultInfo GetTakeMailery(int status, string date)
        {
            DateTime dateTime = DateTime.Now;
            try
            {
                dateTime = DateTime.ParseExact(date, "dd/M/yyyy", null);
            }
            catch
            {
                dateTime = DateTime.Now;
            }

            var user = User.Identity.Name;
            var data = presenter.GetTakeMailer(user);

            return data;
        }

        [HttpGet]
        public ResultInfo GetDetails(string documentID)
        {
            return presenter.GetTakeMailerDetail(documentID);
        }

        [HttpPost]
        public ResultInfo UpdateTakeMailer()
        {
            var result = new ResultInfo()
            {
                error = 0,
                msg = "success"
            };
            try
            {

                var requestContent = Request.Content.ReadAsStringAsync().Result;
                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<UpdateTakeMailerReceive>(requestContent);

                var user = User.Identity.Name;

                result = presenter.UpdateTakeMailer(user, paser);

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;

        }

        [HttpPost]
        public ResultInfo CancelTakeMailer()
        {
            var result = new ResultInfo()
            {
                error = 0,
                msg = "success"
            };
            try
            {

                var requestContent = Request.Content.ReadAsStringAsync().Result;
                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<UpdateTakeMailerReceive>(requestContent);

                var user = User.Identity.Name;

                result = presenter.CancelTakeMailer(user, paser);

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;

        }

        [HttpGet]
        public ResponseInfo GetReportDelivery(string employeeId, string codeTime)
        {
            var fDate = DateTime.Now;
            var tDate = DateTime.Now;

            return presenter.GetReportDelivert(employeeId, "", "");
        }
    }
}
