using System.Security.Claims;
using UrbanGrid.Core.Entities;

namespace UrbanGrid.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}
