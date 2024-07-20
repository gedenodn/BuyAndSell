using BuyAndSell.Data;
using BuyAndSell.Models;
using BuyAndSell.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using BuyAndSell.Validators;
using Microsoft.EntityFrameworkCore;
using BuyAndSell.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BuyAndSell.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<AdController> _logger;
        private readonly IAdService _adService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdController(ApplicationDbContext context, ILogger<AdController> logger, IAdService adService, UserManager<ApplicationUser> userManager)
        {
            _adService = adService;
            _applicationDbContext = context;
            _logger = logger;
            _userManager = userManager;
        }
        [HttpGet("GetAd/{adId}")]
        public async Task<IActionResult> GetAd(int adId)
        {
            var ad = await _applicationDbContext.Ads.FindAsync(adId);
            if (ad == null)
            {
                return NotFound("Ad not found");
            }

            return Ok(ad);
        }
        [HttpGet("GetAllAds")]
        public async Task<IActionResult> GetAds()
        {

            var ads = await _applicationDbContext.Ads
                .Select(ad => new
                {
                    ad.Id,
                    ad.Title,
                    ad.Description,
                    ad.ImageUrl,
                    ad.Price
                })
                .ToListAsync();

            return Ok(ads);
        }
        [HttpPost("Сreate")]
        public async Task<IActionResult> CreateAd([FromBody] AdRequest adRequest)
        {
            if (string.IsNullOrEmpty(adRequest.UserId))
            {
                return BadRequest("User ID not provided.");
            }

            var ad = new Ad()
            {
                Description = adRequest.Description,
                Title = adRequest.Title,
                ImageUrl = adRequest.ImageUrl,
                UserId = adRequest.UserId, 
                Price = adRequest.Price 
            };

            var validator = new AdValidator();
            var validationResult = validator.Validate(ad);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdAd = await _adService.AddAdAsync(ad);
            return CreatedAtAction(nameof(GetMyAds), new { id = createdAd.Id }, createdAd);
        }

        [HttpGet("MyAds/{userId}")]
        public async Task<IActionResult> GetMyAds(string userId)
        {
            var userAds = await _applicationDbContext.Ads
                .Where(ad => ad.UserId == userId)
                .ToListAsync();

            return Ok(userAds);
        }

        [HttpDelete("DeleteAd/{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var adToDelete = await _applicationDbContext.Ads.FindAsync(id);
         
            await _adService.DeleteAdAsync(id);
            return NoContent();
        }



        [HttpPut("UpdateAd/{id}")]
        public async Task<IActionResult> UpdateAd(int id, [FromBody] Ad updatedAd)
        {
            var validator = new AdValidator();
            var validationResult = validator.Validate(updatedAd);
            

            var ad = await _adService.UpdateAdAsync(id, updatedAd);
            return Ok(ad);
        }


        [HttpPatch("UpdatePartialAd/{id}")]
        public async Task<IActionResult> UpdatePartialAd(int id, [FromBody] JsonPatchDocument<Ad> patchDoc)
        {
            var adToUpdate = await _applicationDbContext.Ads.FindAsync(id);
            var updatedAd = new Ad();
            patchDoc.ApplyTo(updatedAd, ModelState);

            var validator = new AdValidator();
            var validationResult = validator.Validate(updatedAd);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _adService.UpdatePartialAdAsync(id, patchDoc);
            return NoContent();
        }



    }
}
