namespace LabMS.Tests.New;

using FluentAssertions;
using LabMS.Contracts.Authentication;
using LabMS.Data;
using LabMS.Entities;
using LabMS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class AuthServiceTests
{
    private const string JwtSecret = "test-jwt-signing-key-with-32-characters-minimum";

    [Fact]
    public async Task RegisterAsync_WhenJwtSecretComesFromEnvironmentKey_IssuesValidToken()
    {
        await using var context = CreateDbContext();
        context.Roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin"
        });
        await context.SaveChangesAsync();

        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["JWT_SECRET_KEY"] = JwtSecret,
            ["Jwt:Issuer"] = "LabMS",
            ["Jwt:Audience"] = "LabMSUsers"
        });

        var service = new AuthService(context, configuration);

        var response = await service.RegisterAsync(new RegisterRequest(
            "admin",
            "System Administrator",
            "admin@example.com",
            "StrongPassword123!",
            ["Admin"]));

        response.Should().NotBeNull();
        response!.Roles.Should().Contain("Admin");
        ValidateToken(response.Token, JwtSecret).Should().Contain(claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
    }

    [Fact]
    public async Task RegisterAsync_WhenJwtSecretIsTooShort_ThrowsInvalidOperationException()
    {
        await using var context = CreateDbContext();
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "too-short",
            ["Jwt:Issuer"] = "LabMS",
            ["Jwt:Audience"] = "LabMSUsers"
        });

        var service = new AuthService(context, configuration);

        var act = () => service.RegisterAsync(new RegisterRequest(
            "admin",
            "System Administrator",
            "admin@example.com",
            "StrongPassword123!",
            []));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*at least 32 characters*");
    }

    private static LabMSDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LabMSDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LabMSDbContext(options);
    }

    private static IConfiguration CreateConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static IEnumerable<Claim> ValidateToken(string token, string jwtSecret)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "LabMS",
            ValidAudience = "LabMSUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        }, out _);

        return principal.Claims;
    }
}
