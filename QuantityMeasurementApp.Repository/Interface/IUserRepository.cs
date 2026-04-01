using System.Collections.Generic;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Interface.Repository
{
    public interface IUserRepository
    {
        void              Save(UserEntity user);
        UserEntity?       GetByEmail(string email);
        UserEntity?       GetById(int id);
        IList<UserEntity> GetAll();
        bool              EmailExists(string email);
        void              Update(UserEntity user);
    }
}