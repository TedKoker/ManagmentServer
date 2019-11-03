using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Contacts
{
    public static class ApiRoutes
    {
        private const string Base = "api";

        public static class Identity
        {
            public const string LogIn = Base + "/login";
            public const string Register = Base + "/register";
        }
    }
}
