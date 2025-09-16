using Microsoft.AspNetCore.Http;

namespace StartupTeam.Shared.Helpers
{
    public static class FormFileHelper
    {
        public static string GenerateNewFileName(IFormFile formFile)
        {
            var fileExtension = Path.GetExtension(formFile.FileName);
            return $"{Guid.NewGuid()}{fileExtension}";
        }
    }
}
