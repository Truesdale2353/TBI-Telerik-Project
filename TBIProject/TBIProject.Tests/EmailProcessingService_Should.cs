using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Services.Contracts;
using TBIProject.Services.Implementation;
using TBIProject.Services.Models;
using TBIProject.Services.Providers.Encryption;
using TBIProject.Services.Providers.Validation;

namespace TBIProject.Tests
{
    [TestClass]
    public class EmailProcessingService_Should
    {
        [TestMethod]
        public async Task GetEmailFullInfoReturnsInfo()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var encrypter = new Mock<IEncrypter>();
            var validator = new Mock<IValidator>();
            var emailService = new Mock<IEmailService>().Object;

            var date = DateTime.Now;
            var email = new Application
            {
                Id = 1,
                Received = date,
                Email = "kaloyan@abv.bg",
                ApplicationStatus = ApplicationStatus.NotReviewed,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Manager")).Returns(Task.FromResult(true));

            encrypter.Setup(d => d.Decrypt("kaloyan@abv.bg")).Returns("kaloyan@abv.bg");
            encrypter.Setup(d => d.Decrypt("test text")).Returns("test text");

            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.GetEmailFullInfo(1, "testUser");
                Assert.AreEqual(1, executionResult.EmailId);
                Assert.AreEqual(date, executionResult.Emailreceived);
                Assert.AreEqual("kaloyan@abv.bg", executionResult.EmailSender);
                Assert.AreEqual("NotReviewed", executionResult.EmailStatus.ToString());
                Assert.AreEqual("test text", executionResult.Body);
                Assert.AreEqual("1", executionResult.OperatorId);
                Assert.AreEqual(2, executionResult.PermitedOperations.Count);
                Assert.AreEqual(true, executionResult.AllowedToWork);
                Assert.AreEqual(date.Ticks.ToString(), executionResult.CurrentDataStamp);
            }
        }

        [TestMethod]
        public async Task ValidateEmailTimeStampShouldReturnFalse()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var encrypter = new Mock<IEncrypter>();
            var validator = new Mock<IValidator>();
            var emailService = new Mock<IEmailService>().Object;

            var date = DateTime.Now;
            var email = new Application
            {
                Id = 1,
                Received = date,
                Email = "kaloyan@abv.bg",
                ApplicationStatus = ApplicationStatus.NotReviewed,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };
                   

            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.ValidateEmailTimeStamp(1, "1");
                Assert.AreEqual(false, executionResult);
            }
        }

        [TestMethod]
        public async Task ValidateEmailTimeStampShouldReturnTrue()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var encrypter = new Mock<IEncrypter>();
            var validator = new Mock<IValidator>();
            var emailService = new Mock<IEmailService>().Object;

            var date = DateTime.Now;
            var email = new Application
            {
                Id = 1,
                Received = date,
                Email = "kaloyan@abv.bg",
                ApplicationStatus = ApplicationStatus.NotReviewed,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };


            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.ValidateEmailTimeStamp(1, date.Ticks.ToString());
                Assert.AreEqual(true, executionResult);
            }
        }

        [TestMethod]
        public async Task ProcessEmailUpdateShouldReturnFalseWhenWrongStatusIsPassed()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var encrypter = new Mock<IEncrypter>();
            var validator = new Mock<IValidator>();
            var emailService = new Mock<IEmailService>().Object;

            var date = DateTime.Now;
            var email = new Application
            {
                Id = 1,
                Received = date,
                Email = "kaloyan@abv.bg",
                ApplicationStatus = ApplicationStatus.NotReviewed,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Invalid status"
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Manager")).Returns(Task.FromResult(true));          

            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.ProcessEmailUpdate(updateParameters);
                Assert.AreEqual(false, executionResult);
             
            }
        }

        [TestMethod]
        public async Task ProcessEmailUpdateShouldReturnFalseWhenTheUserIsNotPermitedToUpdateTheEmail()
        {
            var options = new DbContextOptionsBuilder<TBIContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var encrypter = new Mock<IEncrypter>();
            var validator = new Mock<IValidator>();
            var emailService = new Mock<IEmailService>().Object;

            var date = DateTime.Now;
            var email = new Application
            {
                Id = 1,
                Received = date,
                Email = "kaloyan@abv.bg",
                ApplicationStatus = ApplicationStatus.NotReviewed,
                Body = "test text",
                OperatorId = "2",
                LastChange = date
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Invalid status"
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Operator")).Returns(Task.FromResult(true));

            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.ProcessEmailUpdate(updateParameters);
                Assert.AreEqual(false, executionResult);

            }
        }
    }
}
