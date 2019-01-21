using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
{
    public class IdentityPriceMatrix
    {
    }
    public class RangeZoneInfo
    {
        public string GroupID { get; set; }

        public string ZoneID { get; set; }

        public int? No { get; set; }
    }
    public class RangeWeightInfo
    {
        public string GroupID { get; set; }

        public float FromWeight { get; set; }

        public float ToWeight { get; set; }

        public bool IsNextWeight { get; set; }

        public int? No { get; set; }
    }
}