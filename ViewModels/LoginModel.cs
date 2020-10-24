using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHB.WebApi.ViewModels
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class RefreshTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
