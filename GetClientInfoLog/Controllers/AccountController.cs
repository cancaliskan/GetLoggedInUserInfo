using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FreeGeoIPCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using GetClientInfoLog.Models;
using UAParser;

namespace GetClientInfoLog.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (LoginUser(loginModel.Username, loginModel.Password))
            {
                var claims = new List<Claim>
                             {
                                 new Claim(ClaimTypes.Name, loginModel.Username)
                             };

                var userIdentity = new ClaimsIdentity(claims, "login");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                //get client info
                var model = GetClientInfoLog();

                //Just redirect to our index after logging in. 
                return RedirectToAction("Index", "Home", model);
            }
            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }

        private bool LoginUser(string username, string password)
        {
            if (username == "can" && password == "123")
            {
                return true;
            }

            return false;
        }

        private ClientLogInfo GetClientInfoLog()
        {
            var log = new ClientLogInfo();

            log.UserAgent = Request.Headers["User-Agent"];
            log.Ip = GetIpAddress().ToString();
            log.IpLocation = GetIpLocation(log.Ip);
            log.Platform = GetPlatform();
            log.Browser = GetBrowser(log.UserAgent);

            return log;
        }

        private IPAddress GetIpAddress()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4();
        }

        private string GetPlatform()
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        }

        /// <summary>
        /// we are use "FreeGeoIP" package for get location information
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private string GetIpLocation(string ipAddress)
        {
            FreeGeoIPClient client = new FreeGeoIPClient();
            var location = client.GetLocation(ipAddress).Result;
            return location.ToString();
        }

        /// <summary>
        /// we are use "UAParser" package for get browser information
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private string GetBrowser(string userAgent)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo clientInfo = uaParser.Parse(userAgent);
            return clientInfo.UserAgent.Family + " " + clientInfo.UserAgent.Major + "." + clientInfo.UserAgent.Minor;
        }
    }
}