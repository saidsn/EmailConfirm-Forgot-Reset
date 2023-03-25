﻿using JuanProject.Services.Interfaces;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Configuration;
using MailKit.Net.Smtp;

namespace JuanProject.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string to, string subject, string body, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ??  _configuration.GetSection("Smtp:FromAddress").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("Smtp:Server").Value, int.Parse(_configuration.GetSection("Smtp:Port").Value), SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("Smtp:FromAddress").Value, _configuration.GetSection("Smtp:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
