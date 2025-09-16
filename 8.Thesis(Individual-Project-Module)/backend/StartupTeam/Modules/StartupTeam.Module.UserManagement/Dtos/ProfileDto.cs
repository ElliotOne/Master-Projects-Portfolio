namespace StartupTeam.Module.UserManagement.Dtos
{
    public class ProfileDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}
