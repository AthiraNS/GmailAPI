using GmailAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GmailAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // OAuth 2.0 credentials
            string[] scopes = { GmailService.Scope.MailGoogleCom };
            string clientId = "761807872257-k2kkdmik2ctp7qg8jpiaeo0m1uuu6d8e.apps.googleusercontent.com";
            string clientSecret = "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";

            // Authenticate
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore("GmailAPI")).Result;

            // Create Gmail API service
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail API Test",
            });

            // Create email message
            var msg = new MimeKit.MimeMessage();
            msg.From.Add(new MimeKit.MailboxAddress("Sender", "sender@example.com"));
            msg.To.Add(new MimeKit.MailboxAddress("Recipient", "recipient@example.com"));
            msg.Subject = "Test Email";
            msg.Body = new MimeKit.TextPart("plain") { Text = "This is a test email." };

            // Convert MimeMessage to Google.Apis.Gmail.v1.Data.Message
            MemoryStream ms = new MemoryStream();
            msg.WriteTo(ms);
            var gmailMessage = new Message();
            gmailMessage.Raw = Convert.ToBase64String(ms.GetBuffer()).Replace("+", "-").Replace("/", "_").Replace("=", "");

            // Send email
            service.Users.Messages.Send(gmailMessage, "me").Execute();

            return Content("Email sent successfully!");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
