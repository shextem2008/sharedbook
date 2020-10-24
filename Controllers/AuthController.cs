using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SHB.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // new
    public class AuthController : ControllerBase
    {

        public ActionResult GetToken()
        {
            //return Ok("Hello From Api");

            //security key
            var securitykey = "this_is_our_long_security_key_for_token_validation";

            //symetric security key
            var symetricsercuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitykey));

            //Signing Credential
            var signingcredentials = new SigningCredentials(symetricsercuritykey, SecurityAlgorithms.HmacSha256Signature);

            //Create token
            var token = new JwtSecurityToken(
                issuer: "SHB.Api",
                audience: "SHB.Web",
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingcredentials
                );
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));

        }
    }
}