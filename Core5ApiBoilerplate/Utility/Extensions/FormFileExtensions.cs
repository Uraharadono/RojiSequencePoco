using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core5ApiBoilerplate.Utility.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            await using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
