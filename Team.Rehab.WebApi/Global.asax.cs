using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Team.Rehab;

using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Http.Dispatcher;
using Team.Rehab.WebApi.Infrastructure.Dispatcher;

namespace Team.Rehab.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AreaHttpControllerSelector(GlobalConfiguration.Configuration));
           Bootstrapper.Initialise();

            //Define Formatters


            //Add CORS Handler
            //  GlobalConfiguration.Configuration.MessageHandlers.Add(new CorsHandler());
        }
        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}
