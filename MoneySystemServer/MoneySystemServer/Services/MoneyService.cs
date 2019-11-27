using System;
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

        public async Task<List<UserMoneyDetaleResponse>> GetMonthAsync(string userId, int? monthNumber, int page)
        {
            if (monthNumber == null)
            {
                monthNumber = DateTime.Now.Month;
            }
            List<MoneyDetale> moneyDetales = await Context.MoneyDetale.Select(x => x)
                .Where(x => x.UserId == userId.ToString())
                .Where(x => x.Date.Month == monthNumber)
                .ToListAsync();
            
            List<UserMoneyDetaleResponse> userMoneyDetaleResponses = new List<UserMoneyDetaleResponse>();

            
            OrderLinqList firstItam = new OrderLinqList() 
            { 
                UserMoneyDetaleResponse = Mapper.Map<UserMoneyDetaleResponse>(moneyDetales[0]) 
            };

            for(int i=1; i<moneyDetales.Count; i++)
            {
                OrderLinqList currentItam = firstItam;
                bool addedToEnd = false;
                while(moneyDetales[i].Date.Day>=currentItam.UserMoneyDetaleResponse.Date.Day && !addedToEnd)
                {
                    if (currentItam.SonNode == null)
                    {
                        currentItam.SonNode = new OrderLinqList(currentItam, null)
                        {
                            UserMoneyDetaleResponse = Mapper.Map<UserMoneyDetaleResponse>(moneyDetales[i])
                        };

                        addedToEnd = true;
                    }
                    else
                    {
                        currentItam = currentItam.SonNode;
                    }
                }
                if (!addedToEnd)
                {
                    currentItam = new OrderLinqList(currentItam.FatherNode, currentItam)
                    {
                        UserMoneyDetaleResponse = Mapper.Map<UserMoneyDetaleResponse>(moneyDetales[i])
                    };

                    if (currentItam.UserMoneyDetaleResponse.Date.Day < firstItam.UserMoneyDetaleResponse.Date.Day)
                    {
                        firstItam = currentItam;
                    }
                }
            }

            OrderLinqList nowItam = firstItam;
            while (nowItam != null)
            {
                userMoneyDetaleResponses.Add(nowItam.UserMoneyDetaleResponse);
                nowItam = nowItam.SonNode;
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
