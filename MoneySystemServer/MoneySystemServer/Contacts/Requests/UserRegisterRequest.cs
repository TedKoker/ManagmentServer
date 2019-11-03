﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Contacts.Requests
{
    public class UserRegisterRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConformPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
