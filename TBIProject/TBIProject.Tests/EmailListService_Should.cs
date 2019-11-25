using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Services.Implementation;
using TBIProject.Services.Providers.Encryption;

namespace TBIProject.Tests
{
    [TestClass]
    public class EmailListService_Should
    {
        [TestMethod]
        public async Task Add_Newly_ReceivedMessage_Should_Add_Message_To_The_Database()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var encrypter = new Mock<IEncrypter>();

            encrypter.Setup(e => e.Encrypt("telerik.tbi@gmail.com")).Returns("harambe");

            var db = new TBIContext(options);

            var service = new EmailListService(db, encrypter.Object);

            await service.AddNewlyReceivedMessage("123", "telerik.tbi@mgmail.com", "telerik.tbi@gmail.com", new List<int> { 1, 2 });

            Assert.AreEqual(await db.Applications.CountAsync(), 1);
        }

        [TestMethod]
        public async Task Add_Newly_ReceivedMessage_Should_Not_Add_The_Same_Message_To_The_Database()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var encrypter = new Mock<IEncrypter>();

            encrypter.Setup(e => e.Encrypt("telerik.tbi@gmail.com")).Returns("harambe");

            var db = new TBIContext(options);

            var service = new EmailListService(db, encrypter.Object);

            await service.AddNewlyReceivedMessage("123", "telerik.tbi@mgmail.com", "telerik.tbi@gmail.com", new List<int> { 1, 2 });

            await service.AddNewlyReceivedMessage("123", "telerik.tbi@mgmail.com", "telerik.tbi@gmail.com", new List<int> { 1, 2 });

             Assert.AreEqual(await db.Applications.CountAsync(), 1);
        }

        [TestMethod]
        public async Task ListEmailsShouldListFirst10Emails()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var encrypter = new Mock<IEncrypter>();

            encrypter.Setup(e => e.Encrypt("telerik.tbi@gmail.com")).Returns("harambe");

            var db = new TBIContext(options);

            var service = new EmailListService(db, encrypter.Object);

            await this.SeedWithEmails(10, db);

            var emails = await service.ListEmails(0, 1);

            Assert.AreEqual(emails.Count, 10);
        }

        [TestMethod]
        public async Task ListEmailsShouldNotListAnyEmails()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var encrypter = new Mock<IEncrypter>();

            encrypter.Setup(e => e.Encrypt("telerik.tbi@gmail.com")).Returns("harambe");

            var db = new TBIContext(options);

            var service = new EmailListService(db, encrypter.Object);

            await this.SeedWithEmails(10, db);

            var emails = await service.ListEmails(0, 3);

            Assert.AreEqual(emails.Count, 0);
        }

        [TestMethod]
        public async Task ListEmailsShouldListFirst10EmailsWhichAreNotReviewed()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var encrypter = new Mock<IEncrypter>();

            encrypter.Setup(e => e.Encrypt("telerik.tbi@gmail.com")).Returns("harambe");

            var db = new TBIContext(options);

            var service = new EmailListService(db, encrypter.Object);

            await this.SeedWithEmails(10, db);

            await this.SetEmailFilters(db, "NotReviewed");

            var emails = await service.ListEmails(1, 1);

            Assert.AreEqual(emails.Count, 10);
        }

        [TestMethod]
        public async Task ListEmailsShouldNotListAnyEmailsWithFilter()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var encrypter = new Mock<IEncrypter>();

            encrypter.Setup(e => e.Encrypt("telerik.tbi@gmail.com")).Returns("harambe");

            var db = new TBIContext(options);

            var service = new EmailListService(db, encrypter.Object);

            await this.SeedWithEmails(10, db);

            await this.SetEmailFilters(db, "Accepted");

            var emails = await service.ListEmails(4, 1);

            Assert.AreEqual(emails.Count, 0);
        }

        private async Task SetEmailFilters(TBIContext context, string status)
        {
            foreach(var app in context.Applications)
            {
                app.ApplicationStatus = Enum.Parse<ApplicationStatus>(status, true);
            }

            await context.SaveChangesAsync();
        }

        private async Task SeedWithEmails(int count, TBIContext context)
        {
            for(int i = 0; i < count; i++)
            {
                await context.Applications.AddAsync(new Application
                {
                    GmailId = i.ToString(),
                    Email = "telerik.tbi@gmail.com",
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
