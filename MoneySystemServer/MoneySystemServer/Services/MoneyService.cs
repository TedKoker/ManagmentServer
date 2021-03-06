﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneySystemServer.Data;
using MoneySystemServer.Models;
using Microsoft.EntityFrameworkCore;
using MoneySystemServer.Contacts.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Cors;

namespace MoneySystemServer.Services
{
    public class MoneyService : IMoneyService
    {
        private readonly ApplicationDbContext Context;
        private readonly IMapper Mapper;
        public MoneyService(ApplicationDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public async Task<List<UserMoneyDetaleResponse>> DeleteMonthAsync(UserMoneyDetaleRequest moneyId, string userId)
        {
            MoneyDetale removeThis = Context.MoneyDetale.FirstOrDefault(x => x.Id == moneyId.Id);
            Context.MoneyDetale.Remove(removeThis);
            Context.SaveChanges();
            return await GetMonthAsync(userId, moneyId.Date.Month, 1); 
        }

        public async Task<List<UserMoneyDetaleResponse>> GetMonthAsync(string userId, int? monthNumber, int? year)
        {
            if (monthNumber == null)
            {
                monthNumber = DateTime.Now.Month;
            }
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            List<MoneyDetale> moneyDetales = await Context.MoneyDetale.Select(x => x)
                .Where(x => x.UserId == userId.ToString())
                .Where(x => x.Date.Month == monthNumber && x.Date.Year == year)
                .OrderBy(x=>x.Date)
                .ToListAsync();

            List<UserMoneyDetaleResponse> userMoneyDetaleResponses = new List<UserMoneyDetaleResponse>();

            foreach (MoneyDetale moneyDetale in moneyDetales)
            {
                userMoneyDetaleResponses.Add(Mapper.Map<UserMoneyDetaleResponse>(moneyDetale));
            }

            return userMoneyDetaleResponses;
        }

        public async Task<UserMoneyDetaleResponse> PostMonth(UserMoneyDetaleRequest userMoneyDetaleRequest, string userId)
        {
            //Maybe it's needed to add another auto mapper in the mvcInstaller
            MoneyDetale money = Mapper.Map<MoneyDetale>(userMoneyDetaleRequest);
            money.UserId = userId;
            money.Id = Guid.NewGuid().ToString();
            await Context.MoneyDetale.AddAsync(money);
            await Context.SaveChangesAsync();
            UserMoneyDetaleResponse userMoneyDetaleResponse = Mapper.Map<UserMoneyDetaleResponse>(money);
            return userMoneyDetaleResponse;
        }


    }
}
