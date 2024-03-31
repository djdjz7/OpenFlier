namespace OpenFlier.Desktop.Services
{
    public class User
    {
        public string Username { get; set; }
        public string? UserId { get; set; }
        public string? CurrentClientId { get; set; }
        public bool AllowCommandInput { get; set; }
        public string? CommandInputSource { get; set; }
        public bool IsBusy { get; set; }
    }
}
