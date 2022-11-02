using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserBL userBL;

        private readonly ILogger<UsersController> logger;
        public UsersController(IUserBL userBL,ILogger<UsersController> logger)
        {
            this.userBL = userBL;
            this.logger = logger;
        }
        [HttpPost("Register")]

        public IActionResult AddUser(UserRegistration userRegistration)
        {
            try
            {
                var reg = this.userBL.Registration(userRegistration);
                if (reg != null)
                {
                    logger.LogInformation("Registration is sucess");
                    return this.Ok(new { Success = true, message = "Registration Sucessfull", Response = reg });
                }
                else
                {
                    logger.LogInformation("Registration is unsucess");
                    return this.BadRequest(new { Success = false, message = "Registration Unsucessfull" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("LoginUser")]
        public IActionResult LoginUser(LoginUser loginUser)
        {
            try
            {
                string Login = this.userBL.LoginUser(loginUser);
                if (Login != null)
                {
                    logger.LogInformation("Login is sucess");
                    return this.Ok(new { success = true, message = $"login successful for {loginUser.Email}", data = Login });
                }
                else
                {
                    logger.LogInformation("This is an error message");
                    return this.BadRequest(new { Success = false, message = $"login failed for {loginUser.Email}" });

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("ForgetPassword")]
        public IActionResult ForgetPassword(string email)
        {
            try
            {
                string token = userBL.ForgetPassword(email);
                if (token != null)
                {
                    return Ok(new { success = true, Message = "Please check your Email Token sent succesfully." });
                }
                else
                {
                    return this.BadRequest(new { Success = false, Message = "Email not registered.register your Email" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPost("Reset")]
        public IActionResult ResetPassword(string password, string confirmPassword)
        {
            try
            {
                var Email = User.FindFirst(ClaimTypes.Email).Value.ToString();

                if (userBL.ResetPassword(Email, password, confirmPassword))
                {
                    return this.Ok(new { Success = true, message = "Your password has been changed sucessfully" });
                }
                else
                {
                    return this.BadRequest(new { Success = false, message = "Unable to reset password.Please try again" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}