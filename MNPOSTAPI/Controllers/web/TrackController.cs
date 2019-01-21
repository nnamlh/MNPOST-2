using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MNPOSTCOMMON;
using MNPOSTAPI.Models;

namespace MNPOSTAPI.Controllers.web
{
    public class TrackController : ApiController
    {
        MNPOSTEntities db = new MNPOSTEntities();

        [HttpGet]
        public ResultInfo Find(string mailerId)
        {
            var data = db.MAILER_GETTRACKING(mailerId).ToList();
            var mailer = db.MAILER_GETINFO_BYID(mailerId).Select(p => new
            {
                Id = p.MailerID,
                weight = p.Weight,
                service = p.MailerTypeID,
                sendFrom = p.SendProvinceName + ", " + p.SendDistrictName,
                sendTo = p.RecieProvinceName + ", " + p.ReceiDistrictName
            }).FirstOrDefault();


            if (mailer == null)
            {
                return new ResponseInfo()
                {
                    msg = "Không tìm thấy",
                    error = 1
                };
            }
            else
                return new ResponseInfo()
                {
                    data = new
                    {
                        info = mailer,
                        tracks = data
                    },
                    error = 0
                };
        }
    }
}
