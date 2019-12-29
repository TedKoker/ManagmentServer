using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Models
{
    public class AouthenticationResoult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Sucsses { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
