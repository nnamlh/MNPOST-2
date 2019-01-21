using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOSTAPI.Models
{
    public class ResultInfo
    {
        public int error { get; set; }

        public string msg { get; set; }
    }

    public class ResponseInfo : ResultInfo
    {

        public Object data { get; set; }
    }
    public class ResultWithPaging : ResultInfo
    {
        public int page { get; set; }

        public int toltalSize { get; set; }

        public int pageSize { get; set; }

        public Object data { get; set; }
    }
    public class ItemCommon
    {
        public string code { get; set; }

        public string name { get; set; }
    }

}