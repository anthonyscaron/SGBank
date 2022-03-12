using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.Models.Responses
{
    public class AccountLookUpResponse : Response
    {
        public Account Account { get; set; }
    }
}
