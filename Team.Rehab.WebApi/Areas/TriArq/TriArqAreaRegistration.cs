using System.Web.Http;
using System.Web.Mvc;
using WebApiAreasRouteFix.Extensions;

namespace Team.Rehab.WebApi.Areas.TriArq
{
    public class TriArqAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TriArq";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
  //          context.MapHttpRoute(
  //name: "TriArq_default",
  //routeTemplate: "FHIR/api/{controller}/{id}",
  //defaults: new { id = RouteParameter.Optional }
  //);
            //context.MapRoute(
            //    "TriArq_default",
            //    "TriArq/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}