using System;
using System.Collections.Generic;
using System.Linq;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Interface.Repository;

namespace QuantityMeasurementApp.Repository.Repositories
{
    public class UserEFRepository : IUserRepository
    {
        private readonly QuantityMeasurementDbContext _context;

        public UserEFRepository(QuantityMeasurementDbContext context)
        {
            _context = context;
        }

        // Save new user to database
        public void Save(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Add(user);
            _context.SaveChanges();

            Console.WriteLine("[UserEFRepository] Saved user: " + user.Email);
        }

        // Find user by email — case insensitive
        public UserEntity? GetByEmail(string email)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        // Find user by primary key
        public UserEntity? GetById(int id)
        {
            return _context.Users
                .FirstOrDefault(u => u.Id == id);
        }

        // Get all users
        public IList<UserEntity> GetAll()
        {
            return _context.Users.ToList();
        }

        // Check if email already exists — used during registration
        public bool EmailExists(string email)
        {
            return _context.Users
                .Any(u => u.Email.ToLower() == email.ToLower());
        }

        // Update existing user
        public void Update(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Update(user);
            _context.SaveChanges();

            Console.WriteLine("[UserEFRepository] Updated user: " + user.Email);
        }
    }
}