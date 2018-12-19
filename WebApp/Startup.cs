using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events = new CookieAuthenticationEvents()
                {
                    OnValidatePrincipal = async context =>
                    {
                        if (context.Principal.Identity.IsAuthenticated)
                        {
                            var accessToken = context.Properties.GetTokenValue(OpenIdConnectParameterNames.AccessToken);
                            var refreshToken = context.Properties.GetTokenValue(OpenIdConnectParameterNames.RefreshToken);
                            var jwtHandler = new JwtSecurityTokenHandler();
                            var token = jwtHandler.ReadJwtToken(accessToken);
                            var exp = DateTimeOffset.FromUnixTimeSeconds(token.Payload.Exp.Value).LocalDateTime;
                            if (exp < DateTime.Now)
                            {
                                var httpClient = new HttpClient();
                                var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest()
                                {
                                    Address = "https://gia-dev3.santepublique.rtss.qc.ca/auth/realms/msss/protocol/openid-connect/token",
                                    ClientId = "pqdcdev3",
                                    ClientSecret = "a984903f-fdf2-464c-817d-596bbdd9fdbb",
                                    RefreshToken = refreshToken
                                });

                                if (!response.IsError)
                                {
                                    context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, response.RefreshToken);
                                    context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, response.AccessToken);
                                    context.ShouldRenew = true;
                                }
                                else
                                {
                                    context.RejectPrincipal();
                                }
                            }
                        }
                    }
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://gia-dev3.santepublique.rtss.qc.ca/auth/realms/msss";
                options.ClientId = "pqdcdev3";
                options.ClientSecret = "a984903f-fdf2-464c-817d-596bbdd9fdbb";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.UseTokenLifetime = true;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
