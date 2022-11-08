namespace WepJWT;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash{ get; set; }
    public byte[] PasswordSald{ get; set; }
    public string Key { get; set; } = string.Empty;
}
