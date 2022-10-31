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
                string Subject = "Forget Password Token";
                string Body = token;
                string jwt = jwtToken(token);
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("pratiktestex@gmail.com", "fnczrkbrcboibusq"),
                    EnableSsl = true,
                };


                smtpClient.Send("pratiktestex@gmail.com", "pratiktestex@gmail.com", Subject, Body);
                message.BeginReceive();

            }
            catch
            {

                throw;
            }
        }


        public string jwtToken(string token)
        {
            var decodedtoken = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken((decodedtoken));
            var result = jsonToken.Claims.FirstOrDefault().Value;
            return result;
        }
    }
}