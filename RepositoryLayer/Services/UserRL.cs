using CommonLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.AppContext;
using RepositoryLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace RepositoryLayer.Services
{
    public class UserRl : IUserRL
    {
        public readonly AppContext.UserContext ucontext;

        public readonly IConfiguration Iconfiguration;
        public UserRl(AppContext.UserContext ucontext, IConfiguration Iconfiguration)
        {
            this.ucontext = ucontext;
            this.Iconfiguration = Iconfiguration;
        }

        public UserEntity Registration(UserRegistration user)
        {
            try
            {
                UserEntity entity = new UserEntity();


                entity.FirstName = user.FirstName;
                entity.LastName = user.LastName;
                entity.Email = user.Email;
                entity.Password = user.Password;
                entity.Password = EncryptPass(user.Password);
                ucontext.Users.Add(entity);
                int result = this.ucontext.SaveChanges();
                if (result > 0)
                {
                    return entity;
                }
                return null;

            }
            catch (Exception)
            {
                throw;
            }

        }
        public string EncryptPass(string password)
        {
            try
            {
                string msg = "";
                byte[] encode = new byte[password.Length];
                encode = Encoding.UTF8.GetBytes(password);
                msg = Convert.ToBase64String(encode);
                return msg;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string Decrpt(string encodedData)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(encodedData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string LoginUser(LoginUser loginUser)
        {
            try
            {
                UserEntity userEntity = new UserEntity();
                userEntity = this.ucontext.Users.FirstOrDefault(x => x.Email == loginUser.Email);
                string dPass = Decrpt(userEntity.Password);
                var id = userEntity.UserId;
                var email = userEntity.Email;
                if (dPass == loginUser.Password && userEntity != null)
                {
                    var token = GenerateJWTToken(id, email);
                    return token;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public string GenerateJWTToken(long userid, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Iconfiguration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", userid.ToString()),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string ForgetPassword(string email)
        {
            try
            {
                var result = this.ucontext.Users.FirstOrDefault(x => x.Email == email);
                if (result != null)
                {
                    var token = this.GenerateJWTToken(result.UserId, email);
                    new MSMQ().sendData2Queue(token);
                    return token;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ResetPassword(string email, string password, string confirmpassword)
        {
            try
            {
                if (password.Equals(confirmpassword))
                {
                    UserEntity user = ucontext.Users.Where(e => e.Email == email).FirstOrDefault();
                    user.Password = confirmpassword;
                    ucontext.SaveChanges();
                    return true;
                }
                return false;
            }

            catch (Exception)
            {
                throw;
            }
        }

    }
}