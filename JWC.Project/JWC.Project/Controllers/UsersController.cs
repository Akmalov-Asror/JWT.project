using JWC.Project.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWC.Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        string jsonpath = "data.json";

        [HttpPost]
        public IActionResult SignIn(Users users)
        {
            var ketByte = System.Text.Encoding.UTF8.GetBytes("ggfkgsiufgsuugfkuwue");
            var securityKey = new SigningCredentials(new SymmetricSecurityKey(ketByte), SecurityAlgorithms.HmacSha256);

            var security = new JwtSecurityToken(
                issuer: "SignIn",
                audience: "Users",
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, users.Name),
                    new Claim(ClaimTypes.Email, users.Email),
                    new Claim(ClaimTypes.HomePhone, users.Phone),
                },
                expires: DateTime.Now.AddSeconds(20),
                signingCredentials: securityKey);
            var token = new JwtSecurityTokenHandler().WriteToken(security);

            var user = ReadUsers();
            user = user ?? new List<Users>();
            user.Add(users);

            SaveUser(user);

            return Ok(user);
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(ReadUsers());
        }
        [HttpGet("{Name}")]
        public IActionResult GetUserById(string Name)
        {
            var user = ReadUsers()?.FirstOrDefault(u => u.Name == Name);

            if (user is null)
                return NotFound();

            return Ok(user);    
        }
        private void SaveUser(List<Users> users)
        {
            var jsondata = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(jsonpath, jsondata);
        }
        private List<Users>? ReadUsers()
        {
            if (System.IO.File.Exists(jsonpath))
            {
                return null;
            }
            var jsonData = System.IO.File.ReadAllText(jsonpath);
            return JsonConvert.DeserializeObject<List<Users>>(jsonData);
        }
    }

}
