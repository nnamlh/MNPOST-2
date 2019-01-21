using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using MNPOSTAPI.Providers;
using MNPOSTAPI.Models;

[assembly: OwinStartupAttribute("MNPOSTAPI", typeof(MNPOSTAPI.Startup))]
namespace MNPOSTAPI.App_Start
{
    public partial class Startup
    {
       
    }
}