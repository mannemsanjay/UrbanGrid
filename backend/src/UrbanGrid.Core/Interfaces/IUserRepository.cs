using UrbanGrid.Core.Entities;

namespace UrbanGrid.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);

    // ✅ Paginated — used by Admin GetAllUsers
    Task<(IEnumerable<User> Items, int Total)> GetAllAsync(int page, int limit);

    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);

    // ✅ Check if any user exists — used by DbSeeder
    Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate);

    // ✅ NEW — Delete user by id
    Task DeleteAsync(Guid id);
}
