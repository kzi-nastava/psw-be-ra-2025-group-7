using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly StakeholdersContext _context;

        public AccountRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public Account Create(Account account)
        {
            // 1. Mapiramo rolu iz string-a u enum
            var role = ParseRole(account.Role);

            // 2. Kreiramo User (za login/autorizaciju)
            var user = new User(
                username: account.Username,
                password: account.Password,
                role: role,
                isActive: true
            );

            _context.Users.Add(user);
            _context.SaveChanges();

            // 3. Kreiramo Person (za ime/prezime/email)
            var person = new Person(
                userId: user.Id,
                name: account.Username,         // za sada koristimo username kao name
                surname: account.Username,      // isto i za prezime (nema boljih podataka)
                email: account.Email
            );

            _context.People.Add(person);
            _context.SaveChanges();

            // 4. Vraćamo Account "view" koji frontend koristi
            return new Account
            {
                Id = (int)user.Id,
                Username = user.Username,
                Email = person.Email,
                Role = user.Role.ToString(),
                IsBlocked = !user.IsActive,
                Password = string.Empty // nikad ne vraćamo lozinku
            };
        }

        public IEnumerable<AccountDto> GetAll()
        {
            var users = _context.Users.AsNoTracking().ToList();
            var people = _context.People.AsNoTracking().ToList();

            var result =
                from u in users
                join p in people on u.Id equals p.UserId into up
                from p in up.DefaultIfEmpty()
                select new AccountDto
                {
                    Id = (int)u.Id,
                    Username = u.Username,
                    Email = p?.Email ?? string.Empty,
                    Role = u.Role.ToString(),
                    IsBlocked = !u.IsActive,
                 //   Password = string.Empty,
                    FirstName = p?.Name ?? string.Empty,
                    LastName = p?.Surname ?? string.Empty
                };

            return result.ToList();
        }

        public Account? Get(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            var person = _context.People.FirstOrDefault(p => p.UserId == user.Id);

            return new Account
            {
                Id = (int)user.Id,
                Username = user.Username,
                Email = person?.Email ?? string.Empty,
                Role = user.Role.ToString(),
                IsBlocked = !user.IsActive,
                Password = string.Empty
            };
        }

        public void Update(Account account)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == account.Id);
            if (user == null) return;

            // samo menjamo aktivnost korisnika
            user.IsActive = !account.IsBlocked;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        private static UserRole ParseRole(string role)
        {
            // pokušavamo da parsiramo string u enum UserRole (case-insensitive)
            if (!System.Enum.TryParse<UserRole>(role, true, out var parsed))
            {
                throw new System.ArgumentException($"Invalid role: {role}");
            }
            return parsed;
        }
    }
}
