using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOSTAPI.Controllers.partner
{
    public class PartnerIdentityResult
    {
    }


    ///viettel
    ///
    public class ViettelTrackData
    {
        public string ORDER_NUMBER { get; set; }
        public string ORDER_REFERENCE { get; set; }
        public string ORDER_STATUSDATE { get; set; }
        public int ORDER_STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public string LOCALION_CURRENTLY { get; set; }
        public string NOTE { get; set; }
        public int MONEY_COLLECTION { get; set; }
        public int MONEY_FEECOD { get; set; }
        public int MONEY_TOTAL { get; set; }
    }

    public class ViettelTrackResult
    {
        public ViettelTrackData DATA { get; set; }
        public string TOKEN { get; set; }
    }

}