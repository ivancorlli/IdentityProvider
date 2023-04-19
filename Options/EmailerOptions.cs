namespace IdentityProvider.Options;

public class EmailerOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public string Username { get; set; } = string.Empty;
    public string Password{ get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
