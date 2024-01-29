using System.Threading.Tasks;
using BuyAndSell.Models;
using BuyAndSell.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuyAndSell.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountApiController> _logger; // Зависимость ILogger

        public AccountApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountApiController> logger) // Изменен конструктор для внедрения ILogger
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; // Инициализация _logger
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
                    EmailConfirmed = true, // Устанавливаем EmailConfirmed в true
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
                    return Ok();
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
    }
}
