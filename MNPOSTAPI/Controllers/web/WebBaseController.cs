using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNPOSTAPI.Controllers.web
{

    [Authorize(Roles = "website")]
    public class WebBaseController : ApiController
    {
        protected MNPOSTEntities db = new MNPOSTEntities();
        protected NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    }
}
