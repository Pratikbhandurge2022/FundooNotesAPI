using Experimental.System.Messaging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonLayer.Models
{
    public class MSMQ
    {
        MessageQueue message = new MessageQueue();

        public void sendData2Queue(string Token)
        {
            message.Path = @".\private$\Token";
            if (!MessageQueue.Exists(message.Path))
            {
                MessageQueue.Create(message.Path);

            }

            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            message.ReceiveCompleted += Message_ReceiveCompleted;


            message.Send(Token);

            message.BeginReceive();
            message.Close();


        }

        private void Message_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                var msg = message.EndReceive(e.AsyncResult);
                string token = msg.Body.ToString();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                //string Subject = "Forget Password Token";
                //string Body = token;
                //string jwt = jwtToken(token);
                //var smtpClient = new SmtpClient("smtp.gmail.com")

                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("pratiktestex@gmail.com", "fnczrkbrcboibusq"),
                    EnableSsl = true,
                };
                mailMessage.From = new MailAddress("pratiktestex@gmail.com");
                mailMessage.To.Add(new MailAddress("pratiktestex@gmail.com"));

                //smtpClient.Send("pratiktestex@gmail.com", "pratiktestex@gmail.com", Subject, Body);
                //message.BeginReceive();


                mailMessage.Body = $"<!DOCTYPE html>" +
                                  $"<html>" +
                                  $"<html lang=\"en\">" +
                                   $"<head>" +
                                   $"<meta charset=\"UTF - 8\">" +
                                   $"</head>" +
                                   $"<body>" +
                                   $"<h2> Dear Fundoo User, </h2>\n" +
                                   $"<h3> Please click on the below link to reset password</h3>" +
                                   $"<a href='http://localhost:4200/reset/{token}'> ClickHere </a>\n " +
                                   $"<h3 style = \"color: #EA4335\"> \nThe link is valid for 24 hour </h3>" +
                                   $"</body>" +
                                  $"</html>";

                mailMessage.Subject = "Fundoo Notes Password Reset Link";
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);

            }
            catch
            {

                throw;
            }
        }


        //public string jwtToken(string token)
        //{
        //    var decodedtoken = token;
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadJwtToken((decodedtoken));
        //    var result = jsonToken.Claims.FirstOrDefault().Value;
        //    return result;
        //}
    }
}