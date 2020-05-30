using System;
using System.Collections.Generic;
using UserService.Entities;

namespace UserService.Interfaces
{
    public interface IUserLoginService
    {
        User Authenticate(string username, string password);

        User MapToEntity(User user, User entity = null);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
    }
}