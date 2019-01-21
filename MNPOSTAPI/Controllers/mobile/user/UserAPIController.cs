using MNPOSTAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MNPOSTAPI.Controllers.mobile.user
{

    [Authorize(Roles ="user")]
    public class UserAPIController : ApiController
    {

        UserAPIPresenter mPresenter = new UserAPIPresenter();

        /***
        * 
        * */
        [HttpGet]
        public ResultInfo GetInfo(string firebaseId= "")
        {
            var result = mPresenter.GetUserInfo(User.Identity.Name, firebaseId);


            return result;
        }

        /***
         * 
         * */
    }
}
