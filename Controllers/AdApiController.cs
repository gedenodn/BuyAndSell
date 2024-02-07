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
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.JsonPatch.Adapters;

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

        [HttpGet("GetAd/{id}")]
        public IActionResult GetAd(int id)
        {
            try
            {
                var ad = _applicationDbContext.Ads.FirstOrDefault(a => a.Id == id);
                if (ad == null)
                {
                    return NotFound("Ad not found.");
                }

                return Ok(ad);
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

               
                existingAd.Title = updatedAd.Title;
                existingAd.Description = updatedAd.Description;
                existingAd.ImageUrl = updatedAd.ImageUrl;
                

                _applicationDbContext.SaveChanges();

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteAd/{id}")]
        public IActionResult DeleteAd(int id)
        {
            try
            {
                var adToDelete = _applicationDbContext.Ads.FirstOrDefault(a => a.Id == id);
                if (adToDelete == null)
                {
                    return NotFound("Ad not found.");
                }

                _applicationDbContext.Ads.Remove(adToDelete);
                _applicationDbContext.SaveChanges();

                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("UpdatePartialAd/{id}")]
        public IActionResult UpdatePartialAd(int id, [FromBody] JsonPatchDocument<Ad> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var adToUpdate = _applicationDbContext.Ads.FirstOrDefault(a => a.Id == id);
            if (adToUpdate == null)
            {
                return NotFound();
            }

            try
            {
                patchDoc.ApplyTo(adToUpdate, ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to apply patch: {ex.Message}");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _applicationDbContext.SaveChanges();

            return NoContent();
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
