using System.Web.Mvc;

namespace Team.Rehab.WebApi.Areas.FHIR
{
    public class FHIRAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FHIR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FHIR_default",
                "FHIR/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}