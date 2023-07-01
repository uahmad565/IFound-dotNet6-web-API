namespace IFoundBackend.Areas.Help
{
    public static class AuthenticateService
    {
        public static string FindUserName(string email)
        {
            int index = email.IndexOf("@");
            return email[..index];
        }
    }
}
