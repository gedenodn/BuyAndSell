using BuyAndSell.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BuyAndSell.Services
{
	public interface IAdService
	{
        Task<Ad> AddAdAsync(Ad ad);
        Task<Ad> UpdateAdAsync(int id, Ad updatedAd);
        Task<Ad> UpdatePartialAdAsync(int id, JsonPatchDocument<Ad> patchDoc);
        Task DeleteAdAsync(int id);
    }
}

