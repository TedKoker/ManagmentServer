using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Contacts.Requests
{
    public class UserLogInRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
