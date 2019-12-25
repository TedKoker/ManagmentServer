using MoneySystemServer.Contacts.Requests;
using MoneySystemServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Services
{
    public interface IMoneyService
    {
        Task<List<UserMoneyDetaleResponse>> GetMonthAsync(string userId, int? monthNumber, int page);
        Task<UserMoneyDetaleResponse> PostMonth(UserMoneyDetaleRequest userMoneyDetaleRequest, string userId);
        Task<List<UserMoneyDetaleResponse>> DeleteMonthAsync(UserMoneyDetaleRequest moneyId, string userId);
    }
}
