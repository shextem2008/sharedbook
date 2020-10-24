using SHB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHB.WebApi.ViewModels
{
    //public class JwtConfigModel
    //{
        [Serializable]
        public class JwtConfig : ISettings
        {
            public string SecurityKey { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            /// <summary>
            /// Seconds 
            /// </summary>
            public int TokenDurationInSeconds { get; set; }
        }
    //}
}
