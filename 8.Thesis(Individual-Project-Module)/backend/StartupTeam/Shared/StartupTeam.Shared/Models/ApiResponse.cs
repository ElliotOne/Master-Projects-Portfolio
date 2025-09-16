namespace StartupTeam.Shared.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public object? Errors { get; set; }

        public ApiResponse()
        {
            Success = true;
        }
    }
}
