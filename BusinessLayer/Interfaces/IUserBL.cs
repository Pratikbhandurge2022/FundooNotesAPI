using CommonLayer.Models;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        public UserEntity Registration(UserRegistration userRegistration);

        public string LoginUser(LoginUser loginUser);

        public string GenerateJWTToken(long emailid, string email);

        public string ForgetPassword(string email);

        public bool ResetPassword(string email, string password, string confirmpassword);
    }

}