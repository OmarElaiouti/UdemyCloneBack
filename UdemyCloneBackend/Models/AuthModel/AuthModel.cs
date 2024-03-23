namespace UdemyCloneBackend.Models.AuthModel
{
    public class AuthModel
    {
        public string Message { get; set; }

        public bool isAuthenticated { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public List<string> Roles { get; set; }

        public DateTime ExpiresOn { get; set; }
    }
}
