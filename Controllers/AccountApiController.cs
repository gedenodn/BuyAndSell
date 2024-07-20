using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuyAndSell.Data;
using BuyAndSell.Models;
using BuyAndSell.Models.ViewModel;
using BuyAndSell.Services;
using BuyAndSell.Validators.UserModelValidator;
using BuyAndSell.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IUserService _userService;

        public AccountApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,ILogger<AccountApiController> logger,
         ApplicationDbContext applicationDbContext, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _userService = userService;
            _jwtSecretKey = Encoding.ASCII.GetBytes("MySuperSecretKey12345678901234567890"); 
        }

        [HttpGet("CurrentUser")]
        public IActionResult GetCurrentUser(string userId)
        {
            var user = _applicationDbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError($"[{DateTime.UtcNow}] User not found.");
                return NotFound("User not found.");
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
            var validator = new RegisterViewModelValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Registration validation failed.");
                return BadRequest(validationResult.Errors);
            }    

            await _userService.AddUserAsync(model);
            _logger.LogInformation($"[{DateTime.UtcNow}] User registered successfully.");
            return Ok("User registered successfully.");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var validator = new LoginViewModelValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Login validation failed.");
                return BadRequest(validationResult.Errors);
            }

            var success = await _userService.LoginUserAsync(model);

            if (success)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var token = GenerateJwtToken(user);

                _logger.LogInformation($"[{DateTime.UtcNow}] User logged in successfully.");
                return Ok(new { Token = token, UserId = user.Id });
            }
            else
            {
                _logger.LogError($"[{DateTime.UtcNow}] Invalid login attempt.");
                return BadRequest("Invalid login attempt.");
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation($"[{DateTime.UtcNow}] User logged out successfully.");
            return Ok();
        }


        [HttpPut("EditProfile")]
        public async Task<IActionResult> EditProfile(ChangeUserInformationModel model)
        {
            var validator = new EditUserValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                _logger.LogError($"[{DateTime.UtcNow}] User information validation failed.");
                return BadRequest(validationResult.Errors);
            }

            await _userService.EditUser(model);
            _logger.LogInformation($"[{DateTime.UtcNow}] User information edited!");
            return Ok("User edit successfully.");
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
