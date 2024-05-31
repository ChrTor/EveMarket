using System.Net.Mail;
using System.Net;

namespace EveMarket.Common.SendEmail
{
    public class SendEmail
    {
        public void Handle()
        {
            try
            {

                var fromAddress = new MailAddress("chrickan90@gmail.com", "From Name");
                var toAddress = new MailAddress("chrickan90@gmail.com", "To Name");
                const string fromPassword = "eqrw fvgu jbot kgsq";
                const string subject = "Subject";
                const string body = "Body";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
