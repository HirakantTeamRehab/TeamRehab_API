using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.Owin.Security.Infrastructure;
using Team.Rehab.BusinessEntities;
using Team.Rehab.WebApi;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using Team.Rehab.WebApi.ErrorHelper;

namespace Team.Rehab.Providers
{
    public class ApplicationRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly IAuthRepository _authRepository;

        public OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public ApplicationRefreshTokenProvider()
        {
            //  this._userRepository = UserRepository;

            _authRepository = new AuthRepository();
            //if (publicClientId == null)
            //{
            //    throw new ArgumentNullException("publicClientId");
            //}

            //_publicClientId = publicClientId;
        }


        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];
        
          //  var clientid = Convert.ToString(ConfigurationManager.AppSettings["ClientId"].ToString());
            string deviceId = Convert.ToString(HttpContext.Current.Request.Form["DeviceId"]);
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");


            //  var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
            var refreshTokenLifeTime = Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenExpireTime"].ToString());

            var token = new tblRefreshTokens()
            {
                Id = Helper.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                //   VerificationCode = 999999,
                RefreshToken = refreshTokenId,
                DeviceId = deviceId
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddYears(60);
            token.ProtectedTicketLT = context.SerializeTicket();

            var result = await _authRepository.AddRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }


        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            string verificationCode = Convert.ToString(HttpContext.Current.Request.Form["VerificationCode"]);
            string hashedTokenId = Helper.GetHash(context.Token);
            RefreshToken refToken = new RefreshToken();
            var refreshToken = await _authRepository.FindRefreshToken(hashedTokenId);
            //===============Mapper===========================================
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<tblRefreshTokens, tblRefreshTokens>();
            });

            IMapper mapper = config.CreateMapper();
            refToken = mapper.Map<tblRefreshTokens, RefreshToken>(refreshToken);
            //===============mapper end==========================================
            // if (verificationCode == Convert.ToString(refreshToken.VerificationCode))
            //{
            //    //generate access token from lifetime refresh token
            //    context.DeserializeTicket(refreshToken.ProtectedTicketLT);

            //    var result = await _authRepository.RemoveRefreshToken(hashedTokenId);
            //}
            //else
            if (refreshToken != null)
            {
                if (refreshToken.ExpiresUtc > DateTime.UtcNow)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await _authRepository.RemoveRefreshToken(hashedTokenId);

                }
               
                //else
                //{
                //    SendMessage(refToken);
                //}

            }



        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

    }
}

