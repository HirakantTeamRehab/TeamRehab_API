using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;
using Team.Rehab.Providers;
using Unity.WebApi;
using Team.Rehab.WebApi.App_Start;
using Team.Rehab.Repository;

[assembly: OwinStartup(typeof(Team.Rehab.WebApi.Startup))]



namespace Team.Rehab.WebApi
{
    public class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
           
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);
            

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            app.UseWebApi(config);
           

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/authenticateUser"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["TokenExpireTime"].ToString())),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider(),
                Provider = new ApplicationOAuthProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}