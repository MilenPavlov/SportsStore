using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "orders@example.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "MySmtpUsername";
        public string Password = "MySmtpPassword";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = true;
        public string FileLocation = @"c:\sports_store_emails";
    }
    public class EmailOrderProcessor : IOrderProcessor
    {
        private readonly EmailSettings _settings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            _settings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = _settings.UseSsl;
                smtpClient.Host = _settings.ServerName;
                smtpClient.Port = _settings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_settings.Username, _settings.Password);

                if (_settings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _settings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                var body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("-----")
                    .AppendLine("Items:");
                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price*line.Quantity;
                    body.AppendFormat("{0} x {1} subtotal: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                 .AppendLine("---")
                 .AppendLine("Ship to:")
                 .AppendLine(shippingInfo.Name)
                 .AppendLine(shippingInfo.Line1)
                 .AppendLine(shippingInfo.Line2 ?? "")
                 .AppendLine(shippingInfo.Line3 ?? "")
                 .AppendLine(shippingInfo.City)
                 .AppendLine(shippingInfo.State ?? "")
                 .AppendLine(shippingInfo.Country)
                 .AppendLine(shippingInfo.Zip)
                 .AppendLine("---")
                 .AppendFormat("Gift wrap: {0}",
                 shippingInfo.GiftWrap ? "Yes" : "No");

                var mailMessage = new MailMessage(_settings.MailFromAddress, _settings.MailToAddress,
                    "New order submitted", body.ToString());

                if (_settings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }
}
