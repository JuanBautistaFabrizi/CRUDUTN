using System;
using System.Collections.Generic;
using System.Linq;
using Infraestructura.Models;
using Infraestructura.Properties;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositories
{
    public class UserRepository : IUserRepository
    {
        
        private readonly Context _context;
        public UserRepository(Context context)
        {
            _context = context;
        }
        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }
        public User GetById(int UserID)
        {
            return _context.Users.Find(UserID);
        }
        public void Insert(User User)
        {
            _context.Users.Add(User);
        }
        public void Update(User User)
        {
            _context.Entry(User).State = EntityState.Modified;
        }
        public void Delete(int UserID)
        {
            User User = _context.Users.Find(UserID);
            _context.Users.Remove(User);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
    
