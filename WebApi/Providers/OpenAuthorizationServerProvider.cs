using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Ronzl.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace WebApi.Providers
{
    public class OpenAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public OpenAuthorizationServerProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        /// <summary>
        /// 生成 access_token（resource owner password credentials 授权方式）
        /// </summary>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //客户端发起验证请求时，必然是跨域的，token这个请求不属于任何ApiController的Action，而在WebApiConfig.cs中启用全局的CORS，只对ApiController有效，对token请求是不起作用的。
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (string.IsNullOrEmpty(context.UserName))
            {
                context.SetError("invalid_username", "username is not valid");
                return;
            }
            if (string.IsNullOrEmpty(context.Password))
            {
                context.SetError("invalid_password", "password is not valid");
                return;
            }
            //: OAuthAuthorizationServerProvider

            var loginModel = new { errcode = "200", msg = "成功" };

            if (loginModel.errcode == "200")
            {
                context.SetError(loginModel.errcode, loginModel.msg);
                return;
            }



            var OAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            OAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            OAuthIdentity.AddClaim(new Claim("userModelJson", SerializationHelper.JsonSerialize(loginModel)));
            context.Validated(OAuthIdentity);
            AuthenticationProperties properties = CreateProperties(context.UserName, SerializationHelper.JsonSerialize(loginModel));
            AuthenticationTicket ticket = new AuthenticationTicket(OAuthIdentity, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // 资源所有者密码凭据未提供客户端 ID。
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string userModelJson)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "userModelJson",userModelJson}
            };
            return new AuthenticationProperties(data);
        }
    }
}