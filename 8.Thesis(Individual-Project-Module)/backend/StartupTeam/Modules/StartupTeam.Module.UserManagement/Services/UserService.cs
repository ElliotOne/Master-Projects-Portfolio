using Microsoft.AspNetCore.Identity;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Helpers;
using StartupTeam.Shared.Models.Constants;
using StartupTeam.Shared.Services;

namespace StartupTeam.Module.UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IJwtHelper _jwtHelper;
        private readonly IBlobStorageService _blobStorageService;

        public UserService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IJwtHelper jwtHelper,
            IBlobStorageService blobStorageService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtHelper = jwtHelper;
            _blobStorageService = blobStorageService;
        }

        public async Task<IdentityResult> RegisterUserAsync(BasicSignUpDto basicSignUpDto)
        {
            var user = new User()
            {
                UserName = basicSignUpDto.Email,
                Email = basicSignUpDto.Email
            };

            return await _userManager.CreateAsync(user, basicSignUpDto.Password);
        }

        public async Task<IdentityResult> RegisterExternalUserAsync(ExternalUserDto externalUserDto)
        {
            var user = new User()
            {
                UserName = externalUserDto.Email,
                Email = externalUserDto.Email,
                EmailConfirmed = true,
                FirstName = externalUserDto.FirstName,
                LastName = externalUserDto.LastName,
                ProfilePictureUrl = externalUserDto.ProfilePictureUrl
            };

            return await _userManager.CreateAsync(user);
        }

        public async Task<IdentityResult> CompleteUserRegistrationAsync(CompleteSignUpDto completeSignUpDto)
        {
            var user = await _userManager.FindByEmailAsync(completeSignUpDto.Email);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found."
                });
            }

            var roleExists = await _roleManager.RoleExistsAsync(completeSignUpDto.RoleName);

            if (!roleExists)
            {
                var roleError = new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"The role '{completeSignUpDto.RoleName}' does not exist."
                };
                return IdentityResult.Failed(roleError);
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, completeSignUpDto.RoleName);

            if (!addToRoleResult.Succeeded)
            {
                return addToRoleResult;
            }

            user.UserName = completeSignUpDto.Username;
            user.FirstName = completeSignUpDto.FirstName;
            user.LastName = completeSignUpDto.LastName;

            var updateResult = await _userManager.UpdateAsync(user);
            return updateResult;
        }

        public async Task<string?> GenerateEmailConfirmationTokenAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        public async Task<string?> LoginUserAsync(SignInDto signInDto)
        {
            var user = await _userManager.FindByEmailAsync(signInDto.Email);

            if (user != null
                && await _userManager.CheckPasswordAsync(user, signInDto.Password)
                && user.EmailConfirmed)
            {
                return await GenerateJwtToken(user);
            }

            return null; //Authentication failed
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> FindByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var roles = await GetRolesAsync(user);
            return _jwtHelper.GenerateToken(user, roles);
        }

        public async Task<IEnumerable<ProfileDto>> GetProfilesAsync()
        {
            var users = _userManager.Users.ToList();

            var profiles = new List<ProfileDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var roleName = roles.FirstOrDefault() ?? string.Empty;

                switch (roleName)
                {
                    case RoleConstants.StartupFounder:
                        roleName = "Founder";
                        break;
                    case RoleConstants.SkilledIndividual:
                        roleName = "Individual";
                        break;
                }

                var profile = new ProfileDto
                {
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = roleName,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };

                profiles.Add(profile);
            }

            return profiles;
        }

        public async Task<ProfileDetailDto?> GetProfileByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var roleName = roles.FirstOrDefault() ?? string.Empty;

            switch (roleName)
            {
                case RoleConstants.StartupFounder:
                    roleName = "Founder";
                    break;
                case RoleConstants.SkilledIndividual:
                    roleName = "Individual";
                    break;
            }

            var profileDetail = new ProfileDetailDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = roleName,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return profileDetail;
        }

        public async Task<ProfileFormDto?> GetProfileFormByUserIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var roleName = roles.FirstOrDefault() ?? string.Empty;

            switch (roleName)
            {
                case RoleConstants.StartupFounder:
                    roleName = "Founder";
                    break;
                case RoleConstants.SkilledIndividual:
                    roleName = "Individual";
                    break;
            }

            var profileFormDto = new ProfileFormDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = roleName,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return profileFormDto;
        }

        public async Task<IdentityResult> UpdateProfileAsync(ProfileFormDto profileFormDto)
        {
            var user = await _userManager.FindByIdAsync(profileFormDto.UserId.ToString());

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            // Handle profile picture update
            if (profileFormDto.ProfilePictureFile != null)
            {
                // Delete old profile picture if it exists
                if (!string.IsNullOrEmpty(profileFormDto.ProfilePictureUrl))
                {
                    var oldBlobName = Path.GetFileName(profileFormDto.ProfilePictureUrl);
                    await _blobStorageService.DeleteBlobAsync(BlobStorageConstants.ProfilePicturesContainerName, oldBlobName);
                }

                // Upload new profile picture
                await using var stream = profileFormDto.ProfilePictureFile.OpenReadStream();
                profileFormDto.ProfilePictureUrl = await _blobStorageService.UploadBlobAsync(
                    BlobStorageConstants.ProfilePicturesContainerName,
                    FormFileHelper.GenerateNewFileName(profileFormDto.ProfilePictureFile),
                    stream);
            }

            user.FirstName = profileFormDto.FirstName;
            user.LastName = profileFormDto.LastName;
            user.PhoneNumber = profileFormDto.PhoneNumber;
            user.ProfilePictureUrl = profileFormDto.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);

            return result;
        }

        private async Task<IEnumerable<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
