using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Contacts.Requests
{
    public class UserMoneyDetaleRequest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsIncome { get; set; }
    }
}
