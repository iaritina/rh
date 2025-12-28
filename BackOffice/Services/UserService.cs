using Microsoft.EntityFrameworkCore;
using BackOffice.Data;
using BackOffice.Models;
using BackOffice.ViewModels;
using System.Text;
using Shared;

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
            
            var schedules = new List<Schedule>();
            
            for (int day = 1; day <= 5; day++)
            {
                schedules.Add(new Schedule { Day = day, Start = "08:00", End = "12:00", Working = true, UserId = user.Id });
                schedules.Add(new Schedule { Day = day, Start = "12:00", End = "13:00", Working = false, UserId = user.Id }); 
                schedules.Add(new Schedule { Day = day, Start = "13:00", End = "17:00", Working = true, UserId = user.Id });
            }
            
            schedules.Add(new Schedule { Day = 6, Start = "00:00", End = "23:59", Working = false, UserId = user.Id });
            schedules.Add(new Schedule { Day = 7, Start = "00:00", End = "23:59", Working = false, UserId = user.Id });
            
            _context.Schedules.AddRange(schedules);
            await _context.SaveChangesAsync();
            
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
        
        
        public async Task<int> ImportUsersFromCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Fichier CSV vide");

            int imported = 0;
            int lineNumber = 0;

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

               
                char separator = line.Contains(';') ? ';' : ',';
                var parts = line.Split(separator);

                
                if (lineNumber == 1 && parts[0].ToLower().Contains("nom"))
                    continue;

                if (parts.Length < 4)
                    throw new Exception($"Ligne {lineNumber} invalide : colonnes insuffisantes");

                var firstName = parts[0].Trim();
                var lastName = parts[1].Trim();
                var email = parts[2].Trim();
                var phone = parts[3].Trim();

                if (string.IsNullOrEmpty(email))
                    throw new Exception($"Email vide à la ligne {lineNumber}");

                if (await _context.Users.AnyAsync(u => u.Email == email))
                    continue; // on ignore les doublons proprement

                DateTime? hiringDate = null;
                if (parts.Length >= 5 && DateTime.TryParse(parts[4], out var parsed))
                    hiringDate = parsed;

                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    HiringDate = hiringDate ?? DateTime.Now
                };

                await CreateUser(user);
                imported++;
            }

            return imported;
        }

        
    }
    
    
}