using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOSTAPI.Controllers.mobile.mailer
{
    public class MailerIdentity
    {

    }

    public class UpdateDeliveryReceive
    {
        public string DetailId { get; set; }

        public string MailerID { get; set; }

        public string DocumentID { get; set; }

        public int StatusID { get; set; }

        public string Reciever { get; set; }

        public string DeliveryDate { get; set; } // dd/MM/yyyy HH:mm

        public int ReturnReasonID { get; set; }

        public List<string> images { get; set; }

        public string Note { get; set; }
    }

    public class UpdateTakeMailerReceive
    {
        public string documentId { get; set; }

        public string mailers { get; set; }

        public float weight { get; set; }
    }
}