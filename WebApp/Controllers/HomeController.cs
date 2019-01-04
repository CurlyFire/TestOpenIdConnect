using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string accToken = HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken).Result;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5001/Test/GetSomething");

            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accToken);
            var content = await client.SendAsync(request);
            if (content.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Did not receive OK, received {content.StatusCode.ToString()}");
            }
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            //if (!HttpContext.User.Identity.IsAuthenticated)
            //{
            //    var claims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.NameIdentifier, "Steph")
            //    };

            //    var userIdentity = new ClaimsIdentity(claims, "Simple");

            //    ClaimsPrincipal principal = new ClaimsPrincipal();
            //    principal.AddIdentity(userIdentity);

            //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
            //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //    var jwtSecurityToken = new JwtSecurityToken(
            //        issuer: "yourdomain.com",
            //        audience: "yourdomain.com",
            //        claims: claims,
            //        expires: DateTime.Now.AddMinutes(30),
            //        signingCredentials: creds
            //        );

            //    var accessToken = new AuthenticationToken()
            //    {
            //        Name = OpenIdConnectParameterNames.AccessToken,
            //        Value = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            //    };

            //    var tokens = new List<AuthenticationToken>
            //    {
            //        accessToken
            //    };

            //    var authenticationProperties = new AuthenticationProperties();
            //    authenticationProperties.StoreTokens(tokens);
            //    authenticationProperties.IsPersistent = true;

            //    await HttpContext.SignInAsync(principal, authenticationProperties);
            //}
            return View();
        }
    }
}
