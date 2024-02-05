using BuyAndSell.Data;
using BuyAndSell.DTO;
using BuyAndSell.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;

namespace BuyAndSell.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<AdController> _logger; 

        public AdController(ApplicationDbContext context, ILogger<AdController> logger)
        {
            _applicationDbContext = context;
            _logger = logger; 
        }

        [HttpGet("GetAllAds")]
        public IActionResult GetAds()
        {
            try
            {
                var ads = _applicationDbContext.Ads
                    .ToList()
                    .Select(ad => new
                    {
                        ad.Id,
                        ad.Title,
                        ad.Description,
                        ad.ImageUrl
                    })
                    .ToList();

                return Ok(ads);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateAd/{id}")]
        public IActionResult UpdateAd(int id, [FromBody] Ad updatedAd)
        {
            try
            {
                if (id != updatedAd.Id)
                {
                    return BadRequest("Id in URL does not match Id in Ad object.");
                }

                var existingAd = _applicationDbContext.Ads.FirstOrDefault(a => a.Id == id);
                if (existingAd == null)
                {
                    return NotFound("Ad not found.");
                }

                // Update properties of existingAd with values from updatedAd
                existingAd.Title = updatedAd.Title;
                existingAd.Description = updatedAd.Description;
                existingAd.ImageUrl = updatedAd.ImageUrl;
                // Update other properties as needed

                _applicationDbContext.SaveChanges();

                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("create")]
        public IActionResult CreateAd([FromBody] Ad ad)
        {
            try
            {
                _applicationDbContext.Ads.Add(ad);
                _applicationDbContext.SaveChanges();

                return Ok(ad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
    }
}
