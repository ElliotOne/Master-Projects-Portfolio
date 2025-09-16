using Microsoft.AspNetCore.Identity;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Models;

namespace StartupTeam.Module.UserManagement.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(BasicSignUpDto basicSignUpDto);
        Task<IdentityResult> RegisterExternalUserAsync(ExternalUserDto externalUserDto);
        Task<IdentityResult> CompleteUserRegistrationAsync(CompleteSignUpDto completeSignUpDto);
        Task<string?> GenerateEmailConfirmationTokenAsync(string username);
        Task<IdentityResult> ConfirmEmailAsync(string email, string token);
        Task<string?> LoginUserAsync(SignInDto signInDto);
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByIdAsync(Guid id);
        Task<string> GenerateJwtToken(User user);
        Task<IEnumerable<ProfileDto>> GetProfilesAsync();
        Task<ProfileDetailDto?> GetProfileByUsernameAsync(string username);
        Task<ProfileFormDto?> GetProfileFormByUserIdAsync(Guid userId);
        Task<IdentityResult> UpdateProfileAsync(ProfileFormDto profileFormDto);
    }
}
