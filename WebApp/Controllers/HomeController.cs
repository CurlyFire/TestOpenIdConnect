using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:58461/Test/GetSomething");

            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accToken);
            var content = await client.SendAsync(request);
            if (content.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Did not receive OK, received {content.StatusCode.ToString()}");
            }
            return View();
        }
    }
}
