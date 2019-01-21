using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
{
    public class ERouteInfo
    {
        public string DistrictID { get; set; }

        public string DistrictName { get; set; }

        public string ProvinceID { get; set; }

        public string Type { get; set; }

        public List<CommonData> Staffs { get; set; }

        public bool ISJoin { get; set; }
    }

    public class ERouteDetail
    {
        public string DistrictID { get; set; }

        public bool ISJoin { get; set; }

        public string WardID { get; set; }


        public string WardName { get; set; }

        public CommonData Staff { get; set; }

        public bool IsChoose { get; set; }
    }
}