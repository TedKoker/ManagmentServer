using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MoneySystemServer.Contacts;
using MoneySystemServer.Contacts.Requests;
using MoneySystemServer.Contacts.Response;
using MoneySystemServer.Services;

namespace MoneySystemServer.Controllers
{
    public class IdentityController : Controller
    {
        IIdentityService IdentityService;
        public IdentityController(IIdentityService iIdentityService)
        {
            IdentityService = iIdentityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AouthResponseFaile
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var authResponse = await IdentityService.RegisterAsync(request.Email, request.Password);

            if (!authResponse.Sucsses)
            {
                return BadRequest(new AouthResponseFaile
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSucssesResponse
            {
                Token = authResponse.Token
            });
        }

        [HttpPost(ApiRoutes.Identity.LogIn)]
        public async Task<IActionResult> Login([FromBody] UserLogInRequest request)
        {
            var authResponse = await IdentityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Sucsses)
            {
                return BadRequest(new AouthResponseFaile
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSucssesResponse
            {
                Token = authResponse.Token
            });
        }
    }
}