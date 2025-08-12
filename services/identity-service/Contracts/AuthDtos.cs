namespace IdentityService.Contracts;

public record RegisterDto(string Email, string FullName, string Password);
public record LoginDto(string Email, string Password);