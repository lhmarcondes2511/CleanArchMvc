using CleanArchMvc.API.Models;
using CleanArchMvc.Domain.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchMvc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private readonly IAuthenticate _authentication;
        private readonly IConfiguration _configuration;

        public TokenController(IAuthenticate authentication, IConfiguration configuration)
        {
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _configuration = configuration;
        }

        [HttpPost("CreateUser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CreateUser([FromBody] LoginModel loginModel)
        {
            var result = await _authentication.RegisterUser(loginModel.Email, loginModel.Password);

            if (result)
            {
                return Ok($"User {loginModel.Email} was create successfully");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return BadRequest(ModelState);
            }
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel loginModel)
        {
            var result = await _authentication.Authenticate(loginModel.Email, loginModel.Password);

            if (result)
            {
                return GenerateToken(loginModel);
                //return Ok($"User {loginModel.Email} login successfully");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return BadRequest(ModelState);
            }
        }

        private UserToken GenerateToken(LoginModel loginModel)
        {
            //User declaration
            var claims = new[]
            {
                new Claim("email", loginModel.Email),
                new Claim("meuvalor", "o que você quiser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //FK private generate
            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            //Generate key
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            //Time expiration token
            var expiration = DateTime.UtcNow.AddMinutes(10);

            //Generate token
            JwtSecurityToken token = new JwtSecurityToken(
                //issuer
                issuer: _configuration["Jwt:Issuer"],
                //audience
                audience: _configuration["Jwt:Audience"],
                //claims
                claims: claims,
                //expiration
                expires: expiration,
                //credentials
                signingCredentials: credentials
            );

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
