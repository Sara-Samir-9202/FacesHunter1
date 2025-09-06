public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
 
    public string? Email { get; set; }
    public string? Code { get; set; }
    public string? NewPassword { get; set; }
    
}

