using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHB.WebApi.ViewModels
{
    public class PayStackResponseModel
    {
        public string email { get; set; }

        public int amount { get; set; }

        public string RefCode { get; set; }

        public string PayStackReference { get; set; }
    }
}
