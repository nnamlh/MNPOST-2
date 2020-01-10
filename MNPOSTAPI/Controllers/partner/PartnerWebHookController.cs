using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MNPOSTCOMMON;
using MNPOSTAPI.Models;
using System.Web.Script.Serialization;
using MNPOSTAPI.Controllers.partner;

namespace MNPOSTAPI.Controllers
{
    public class PartnerWebHookController : ApiController
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        MNPOSTEntities db = new MNPOSTEntities();

        [HttpPost]
        public ResultInfo Viettel()
        {
            logger.Info("Viettel send tracking...");
            //8DDDFA02-E82A-418C-ACC7-7D8AB8B52660 --> token
            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;
                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<ViettelTrackResult>(requestContent);

                logger.Info(requestContent);

                if (paser.TOKEN != "8DDDFA02-E82A-418C-ACC7-7D8AB8B52660")
                    throw new Exception("Sai token");

                var order = paser.DATA.ORDER_REFERENCE;
                var thirtCode = paser.DATA.ORDER_NUMBER;

                var findMailer = db.MM_Mailers.Where(p => p.ThirdpartyDocID == thirtCode).FirstOrDefault();

                if (findMailer == null)
                {
                    findMailer = db.MM_Mailers.Where(p => p.MailerID == order).FirstOrDefault();
                }

                /*
                if (findMailer == null)
                    throw new Exception("Sai mã order");
                    */
                var date = DateTime.ParseExact(paser.DATA.ORDER_STATUSDATE, "dd/M/yyyy HH:mm:ss", null);

                var tracking = new MM_TrackingPartner()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateTime = date,
                    Describe = paser.DATA.NOTE,
                    MailerID = findMailer != null ? findMailer.MailerID : order,
                    OrderReferece = paser.DATA.ORDER_NUMBER,
                    LocationCurrent = findMailer != null ? findMailer.CurrentPostOfficeID : "",
                    StatusID = Convert.ToString(paser.DATA.ORDER_STATUS),
                    StatusName = paser.DATA.STATUS_NAME
                };

                db.MM_TrackingPartner.Add(tracking);
                db.SaveChanges();


                if (tracking.StatusID == "501")
                {
                    if (findMailer != null)
                    {
                        findMailer.DeliveryTo = paser.DATA.NOTE;
                        findMailer.DeliveryDate = date;
                        findMailer.DeliveryNotes = "Đã phát";
                        findMailer.CurrentStatusID = 4;

                        db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return new ResultInfo()
            {
                error = 0,
                msg = ""
            };
        }

    }
}
