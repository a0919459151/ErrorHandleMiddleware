using Microsoft.AspNetCore.Mvc;
using Middleware.Exceptions;
using Middleware.Models;

namespace Middleware.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static IEnumerable<User> users = new User[]
        {
            // UserName: "admin"; Password: "123456"
            new User()
            {
                UserName = "admin",
                PasswordHash = "$2a$11$5IIa5qXLyJlAQ/Agl1BTeuf.NRpLknWE.m0PPT0Hw9eUDg1p55csa",
                PasswordSalt = "$2a$11$5IIa5qXLyJlAQ/Agl1BTeu"
            }
        };

        // HttpPost registers a new user
        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            // validate
            if (users.Any(x => x.UserName == request.UserName))
                throw new AppException($"Username \"{request.UserName}\" is already taken.");

            // Create password hash
            var (passwordHash, passwordSalt) = CreatePasswordHash(request.Password);

            // Set password hash and salt
            var user = new User()
            {
                UserName = request.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            // update & save user
            users = users.Append(user);

            // return user
            return user;
        }

        // HttpPost authenticates a user
        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate(UserDto request)
        {
            // validate
            if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.UserName))
                throw new AppException("Username and password are required");

            // get user
            var user = users.SingleOrDefault(x => x.UserName == request.UserName);

            // check if user exists
            if (user == null)
                throw new AppException("Username or password is incorrect");

            // check if password is correct
            if (!VerifyPasswordHash(request.Password, user.PasswordHash))
                throw new AppException("Password is incorrect");

            // authentication successful
            return user;
        }

        private (string passwordHash, string passwordSalt) CreatePasswordHash(string password)
        {
            var passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, passwordSalt);
            return (passwordHash, passwordSalt);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}