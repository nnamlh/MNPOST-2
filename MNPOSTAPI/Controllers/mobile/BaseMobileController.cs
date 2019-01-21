using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNPOSTAPI.Controllers.mobile
{

    [Authorize(Roles = "user")]
    public class BaseMobileController : ApiController
    {

     

    }
}
