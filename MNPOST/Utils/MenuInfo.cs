using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Utils
{
    public class MenuInfo
    {
        public string name { get; set; }

        public string link { get; set; }
    }

    public class GroupMenuInfo
    {
        public string name { get; set; }

        public string icon { get; set; }

        public List<MenuInfo> menus { get; set; }
    }
}