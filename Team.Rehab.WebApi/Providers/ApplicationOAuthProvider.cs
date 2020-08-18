
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using System.Configuration;
using Team.Rehab.WebApi;
using Team.Rehab.BusinessEntities;
using Microsoft.Owin.Security;
using Team.Rehab.WebApi.ErrorHelper;
using System.Net;
using Team.Rehab.Repository;

namespace Team.Rehab.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        //private readonly string _publicClientId;
        private readonly IUserRepository _userRepository;
        private readonly ICommonRepository _commmonRepository;
        private readonly IAuthRepository _authRepository;
        public ApplicationOAuthProvider()
        {
            //  this._userRepository = UserRepository;
            _userRepository = new UserRepository();
            _commmonRepository = new CommonRepository();
            _authRepository = new AuthRepository();
            //if (publicClientId == null)
            //{
            //    throw new ArgumentNullException("publicClientId");
            //}

            //_publicClientId = publicClientId;
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;
            //  var clientIds = Convert.ToString(ConfigurationManager.AppSettings["ClientId"].ToString());


            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            var clientIds = context.ClientId;
            if (clientIds == "" || clientIds == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }
       

            client = _authRepository.FindClient(clientIds);


            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                clientSecret = client.Secret;
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    //if (client.Secret != Helper.GetHash(clientSecret))
                    if (client.Secret != clientSecret)
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            double timeDiff = 0;
            tblBlockedIP tblBlockedIP = new tblBlockedIP();
            var ip = HttpContext.Current.Request.GetOwinContext().Request.RemoteIpAddress;
            tblBlockedIP = _commmonRepository.GetIps();
            int IPBlockAttempt = Convert.ToInt32(ConfigurationManager.AppSettings["IPBlockAttempt"]);
            int IPBlockedTime = Convert.ToInt32(ConfigurationManager.AppSettings["IPBlockedTime"]);
            if (tblBlockedIP != null)
            {
                timeDiff = TimeDifferenceInMinutes(DateTime.Now, Convert.ToDateTime(tblBlockedIP.ModifiedOn));
            }
            if (timeDiff > IPBlockedTime)
            {
                _commmonRepository.DeleteUser(tblBlockedIP.BlockedIPID);
                tblBlockedIP = null;
            }
            tblBlockedIP blockIP = new tblBlockedIP();
            try
            {

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                List<tblAppUser> userDetails = new List<tblAppUser>();
                List<ValidateUserEntity> postValidateEntity = new List<ValidateUserEntity>();

                if (context.ClientId == "MobileApp")

                {
                    userDetails = _userRepository.GetUser(context.UserName, context.Password);
                }
                else
                {
                    postValidateEntity = _userRepository.AuthenticateUser(context.UserName, context.Password);
                }
                //  DevicesEntity userDetails = _userRepository.GetUserByPhoneNo(context.UserName);
                if (userDetails.Count > 0 || postValidateEntity.Count > 0)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));

                    var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    },

                });

                    var ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);

                    if (tblBlockedIP != null)
                    {
                        _commmonRepository.DeleteUser(tblBlockedIP.BlockedIPID);
                        //db.tblBlockedIPs.Remove(tblBlockedIP.ID);
                        //db.SaveChanges();
                    }
                }
                else
                {
                    blockIP.IPAddress = ip;
                    if (tblBlockedIP == null)
                    {

                        blockIP.FailAttempts = 1;
                        blockIP.IPAddress = ip;
                        blockIP.ModifiedOn = DateTime.Now;
                        _commmonRepository.CreateFailureEntry(blockIP);
                        context.SetError("Invalid user or the user is not registered");
                        // throw new ApiDataException(1005, "Invalid user or the user is not registered", HttpStatusCode.InternalServerError);

                        //  db.tblBlockedIPs.Add(blockIP);
                    }
                    else
                    {
                        if (tblBlockedIP.FailAttempts < IPBlockAttempt)
                        {
                            tblBlockedIP.FailAttempts = tblBlockedIP.FailAttempts + 1;
                            tblBlockedIP.BlockedIPID = tblBlockedIP.BlockedIPID;
                            tblBlockedIP.ModifiedOn = DateTime.Now;
                            // db.Entry(blockIP).State = EntityState.Modified;
                            _commmonRepository.UpdateFailureInfo(tblBlockedIP);
                            context.SetError("Invalid user or the user is not registered");
                            //  throw new ApiDataException(1005, "Invalid user or the user is not registered", HttpStatusCode.InternalServerError);

                        }
                        else
                        {
                            context.SetError("Invalid user or the user is not registered");
                            //    throw new ApiDataException(1005, "Invalid user or the user is not registered", HttpStatusCode.InternalServerError);

                        }
                    }
                    // db.SaveChanges();
                    //
                }
            }
            catch (Exception ex)
            {



                context.SetError("Invalid user or the user is not registered");
                //throw new ApiDataException(1004, "Invalid credentials", HttpStatusCode.InternalServerError);

            }
        }

        public double TimeDifferenceInMinutes(DateTime dateone, DateTime datetwo)
        {
            var duration = dateone - datetwo;
            return duration.TotalMinutes;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}