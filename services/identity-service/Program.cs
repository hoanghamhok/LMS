using IdentityService.Contracts;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDb>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration["ConnectionStrings:Default"]));

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/identity/register", async (RegisterDto dto, AppDb db, IPasswordHasher<User> hasher) =>
{
    if (await db.Users.AnyAsync(x => x.Email == dto.Email)) return Results.Conflict("Email existed");
    var user = new User { Id = Guid.NewGuid(), Email = dto.Email, FullName = dto.FullName, Role = "Student" };
    user.PasswordHash = hasher.HashPassword(user, dto.Password);
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/identity/users/{user.Id}", new { user.Id, user.Email, user.FullName });
});

app.MapPost("/identity/login", async (LoginDto dto, AppDb db, IJwtTokenService jwt, IPasswordHasher<User> hasher) =>
{
    var user = await db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
    if (user is null) return Results.Unauthorized();
    var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
    if (verify == PasswordVerificationResult.Failed) return Results.Unauthorized();
    var token = jwt.CreateToken(user);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

app.Run("http://0.0.0.0:8081");