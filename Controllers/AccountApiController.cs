using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BuyAndSell.Data;
using BuyAndSell.Models;
using BuyAndSell.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BuyAndSell.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountApiController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly byte[] _jwtSecretKey;

        public AccountApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountApiController> logger, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _jwtSecretKey = Encoding.ASCII.GetBytes("MySuperSecretKey12345678901234567890"); // Увеличить размер ключа до 32 байт (256 бит)
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _applicationDbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }
            return Ok(user);
        }

        [HttpGet("AllUsersProfile")]
        public IActionResult GetUsers()
        {
            var users = _applicationDbContext.Users.ToList();
            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Attempting to login user with email: {Email}", model.Email);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found.", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return BadRequest(ModelState);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in successfully: {Email}", model.Email);
                    var userInfo = new { FirstName = user.FirstName, LastName = user.LastName };
                    var token = GenerateJwtToken(user);
                    return Ok(new { Token = token, UserInfo = userInfo });
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "User account locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning("User account not allowed to login: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "User account not allowed to login.");
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user with email: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecretKey12345678901234567890"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
