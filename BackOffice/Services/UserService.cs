using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BackOffice.Data;
using BackOffice.Models;
using BackOffice.ViewModels;

namespace BackOffice.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly CongeService soldeService;

        public UserService(AppDbContext context, CongeService soldeService)
        {
            _context = context;
            this.soldeService = soldeService;
        }

        public async Task<PagedResult<User>> GetAllUsers(int page, int pageSize)
        {
            var query = _context.Users.AsNoTracking();

            var total = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = users,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
        
        
        public async Task<User> CreateUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.LastName);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); 
            await soldeService.CreateAsync(user.Id);
            return user;
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> SearchUsersByName(string query)
        {
            query = query.ToLower();
            
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.FirstName.ToLower().Contains(query) || u.LastName.ToLower().Contains(query))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }
        
    }
    
    
}