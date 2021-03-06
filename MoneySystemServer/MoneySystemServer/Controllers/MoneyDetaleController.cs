﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneySystemServer.Contacts;
using MoneySystemServer.Contacts.Requests;
using MoneySystemServer.Extantions;
using MoneySystemServer.Services;

namespace MoneySystemServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)
    // [EnableCors("_myAllowSpecificOrigins")]

    public class MoneyDetaleController : Controller
    {
        private readonly IMoneyService MoneyService;
        public MoneyDetaleController(IMoneyService moneyService)
        {
            MoneyService = moneyService;
        }

        [HttpGet(ApiRoutes.MoneyDetaleRoute.GetLastMonth)]
        public async Task<IActionResult> GetLastMonth()
        {
            List<UserMoneyDetaleResponse> userMoneyDetaleResponse = await MoneyService.GetMonthAsync(GeneralExtantions.GetUserId(HttpContext), null, null);
            return Ok(userMoneyDetaleResponse);
        }

        [HttpGet(ApiRoutes.MoneyDetaleRoute.GetMonth)]
        public async Task<IActionResult> GetMonth([FromRoute] int month, [FromRoute] int year)
        {
            List<UserMoneyDetaleResponse> userMoneyDetaleResponse = await MoneyService.GetMonthAsync(GeneralExtantions.GetUserId(HttpContext), month, year);
            return Ok(userMoneyDetaleResponse);
        }

        [HttpPost(ApiRoutes.MoneyDetaleRoute.Post)] //ApiRoutes.MoneyDetaleRoute.Post
        public async Task<IActionResult> PostMonth([FromBody] UserMoneyDetaleRequest userMoneyDetaleRequest)
        {
            UserMoneyDetaleResponse userMoneyDetaleResponse = await MoneyService.PostMonth(userMoneyDetaleRequest, GeneralExtantions.GetUserId(HttpContext));
            return Ok(userMoneyDetaleResponse);
        }

        [HttpDelete(ApiRoutes.MoneyDetaleRoute.Delete)]
        public async Task<IActionResult> DeleteMonth([FromBody] UserMoneyDetaleRequest userMoneyDetaleRequest)
        {
            List<UserMoneyDetaleResponse> userMoneyDetaleResponse = await MoneyService.DeleteMonthAsync(userMoneyDetaleRequest, GeneralExtantions.GetUserId(HttpContext));
            return Ok(userMoneyDetaleResponse);
        }

    }
}