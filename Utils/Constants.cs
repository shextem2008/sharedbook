using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHB.WebApi.Utils
{

    public class WebConstants 
    {
        public const string ConnectionStringName = "Database";

        public const int DefaultPageSize = int.MaxValue;

        public class Sections
        {
            internal const string Smtp = "Smtp";
            internal const string AuthJwtBearer = "Authentication:JwtBearer";
            internal const string Erp = "Erp";
            //internal const string Booking = "Booking";
            internal const string App = "App";
            internal const string Paystack = "Payment:PayStack";
        }
    }
}
