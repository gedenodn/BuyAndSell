using BuyAndSell.Data;
using BuyAndSell.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BuyAndSell.Services
{
    public class AdService : IAdService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AdService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Ad> AddAdAsync(Ad ad)
        {
            _applicationDbContext.Ads.Add(ad);
            await _applicationDbContext.SaveChangesAsync();
            return ad;
        }

        public async Task<Ad> UpdateAdAsync(int id, Ad updatedAd)
        {
            var existingAd = await _applicationDbContext.Ads.FindAsync(id);
            if (existingAd == null)
            {
                throw new InvalidOperationException("Ad not found.");
            }

            existingAd.Title = updatedAd.Title;
            existingAd.Description = updatedAd.Description;
            existingAd.ImageUrl = updatedAd.ImageUrl;
            existingAd.Price = updatedAd.Price; // Установка цены

            await _applicationDbContext.SaveChangesAsync();
            return existingAd;
        }

        public async Task<Ad> UpdatePartialAdAsync(int id, JsonPatchDocument<Ad> patchDoc)
        {
            var adToUpdate = await _applicationDbContext.Ads.FindAsync(id);
            if (adToUpdate == null)
            {
                throw new InvalidOperationException("Ad not found.");
            }

            patchDoc.ApplyTo(adToUpdate);

            // Убедитесь, что вы применяете изменения цены, если они есть в JsonPatchDocument
            if (patchDoc.Operations.Any(op => op.path == "/Price"))
            {
                _applicationDbContext.Entry(adToUpdate).Property("Price").IsModified = true;
            }

            await _applicationDbContext.SaveChangesAsync();
            return adToUpdate;
        }


        public async Task DeleteAdAsync(int id)
        {
            var adToDelete = await _applicationDbContext.Ads.FindAsync(id);
            if (adToDelete == null)
            {
                throw new InvalidOperationException("Ad not found.");
            }

            _applicationDbContext.Ads.Remove(adToDelete);
            await _applicationDbContext.SaveChangesAsync();
        }

    }
}
