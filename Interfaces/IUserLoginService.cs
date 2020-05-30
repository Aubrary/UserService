using System;
using System.Collections.Generic;
using UserService.Entities;

namespace UserService.Interfaces
{
    public interface IUserLoginService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
    }
}