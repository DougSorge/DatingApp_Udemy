using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        //always remember that _context IS THE DATABASE. The type DataContext could have been called anythiing!!! It is defined in the Data Folder and injected in the startup file!
        private readonly DataContext _context;
        
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenservice)
        {
            _tokenService = tokenservice;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            //Since we are returning an action result we can return certain http status codes like BadResult for isntance.
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");



            //provides the hashing algorithm
            //the using keyword makes sure that we call dispose on the instance of the class when it is no longer being used.
            using var hmac = new HMACSHA512();

            //create new User
            var user = new AppUser
            {
                //username is set to username parameter
                UserName = registerDto.Username.ToLower(),

                //password is taken as paramter and passed to hmac.ComputeHash(); However, compute hash expects a byte array as input and so we call Encoding.UTF8.GetBytes(password), which is provided by Sysyem.Text to change the string into a byte array. The result is a hashed password;
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),

                //The Salt will be the key of the hashing algorithm. Hovering on the new HMACSHA512 instantiation above will inform us that each instance is automatically given a randomly generated key.This line is setting our PasswordSalt property equal to that key. 
                PasswordSalt = hmac.Key
            };

            //tells Entity FW to track this newly created user but doesn't actually add it to the database. In otherwords, Entity is aware of this users existence. 
            _context.Users.Add(user);

            //this line completes the async function and finally adds the user to the table with a hashed and salted password.
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto){
            //compares username data received from login request with usernames in the database
            //if found, the user variable will be set to the values from the corresponding record in the database
            //if not found, the user will be set to null and an exception will be thrown.
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid Username");

            //here we create a new hmac and manually assign its key the value of the password salt from the user in the database we have identified on line 60.
            using var hmac = new HMACSHA512(user.PasswordSalt);

            //here we are doing the same operation as in the register action. We are taking the password data provided to us in the loginDto and hashing it.
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //Here we are comparing the hased password we just created to the hashed password stored on the user object that we pulled from the database. 
            //When the user registers they type in a password, that password is hashed using a key (salt). That salt and that hashed password are stored in the database.
            //When the user logs in he types the pasword he used to register. This login password is hashed again but this time we use the key associated with the user in the database. Since the password being used to login is hashed using the same key as it was upon registration, this hash will be identical. 
            //If the hashed login password is different than the stored hash in the database we know that the user has entered an incorrect password because we are controlling the key.
            //So if they match we return the user obejct, if not, error out.
            if (Enumerable.SequenceEqual(computedHash, user.PasswordHash))
                return new UserDTO
                {
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };

            return Unauthorized("Invalid Password");

        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
