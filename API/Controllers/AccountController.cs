using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            
        }
        
        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Username already exists!");

            var user = _mapper.Map<AppUser>(registerDto);

            //using var hmac = new HMACSHA512();

            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PasswordSalt = hmac.Key;

            // check DateOfBirth and convert to date
            //user.DateOfBirth

            // var user = new AppUser 
            // {
            //     UserName = registerDto.UserName,
            //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //     PasswordSalt = hmac.Key
            // };

            //_context.Users.Add(user);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto 
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UserName.ToLower());
            if (user == null) return Unauthorized("Invalid Username");

            //if (user.PasswordHash == null || user.PasswordSalt == null) return(Unauthorized("Invalid Password"));
            
            // using var hmac = new HMACSHA512(user.PasswordSalt);
            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // for(int i=0; i < computedHash.Length; i++) 
            // {
            //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            // }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return Unauthorized("Invalid Password");

            var userdto = new UserDto();
            userdto.Username = user.UserName;
            userdto.Token = await _tokenService.CreateToken(user);

            var photo = user.Photos.FirstOrDefault(x => x.IsMain);
            if (photo != null)
                userdto.PhotoUrl = photo.Url;

            //userdto.PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url;
            userdto.KnownAs = user.KnownAs ?? "";
            userdto.Gender = user.Gender ?? "";

            return userdto;

            // return new UserDto
            // {
            //     Username = user.UserName,
            //     Token = await _tokenService.CreateToken(user),
            //     PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            //     KnownAs = user.KnownAs ?? "",
            //     Gender = user.Gender ?? ""
            // };
        }

        private async Task<bool> UserExists(string username) 
        {
            return await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}