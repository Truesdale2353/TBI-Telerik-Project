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

        [TestMethod]
        public async Task ProcessEmailUpdateShouldReturnFalseWhenInvalidEGNIsPassed()
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
                ApplicationStatus = ApplicationStatus.New,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Open",
                EGN = "1111111111",
                PhoneNumber = "0876281932"
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Operator")).Returns(Task.FromResult(true));

            validator.Setup(d => d.ValidateEGN("1111111111")).Returns(Task.FromResult(false));
            validator.Setup(d => d.ValidatePhone("0876281932")).Returns(Task.FromResult(true));

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
        public async Task ProcessEmailUpdateShouldReturnFalseWhenInvalidPhoneIsPassed()
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
                ApplicationStatus = ApplicationStatus.New,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Open",
                EGN = "1111111111",
                PhoneNumber = "0876281932"
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Operator")).Returns(Task.FromResult(true));

            validator.Setup(d => d.ValidateEGN("1111111111")).Returns(Task.FromResult(true));
            validator.Setup(d => d.ValidatePhone("0876281932")).Returns(Task.FromResult(false));

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
        public async Task ProcessEmailUpdateShouldPassOnStatusOpen()
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
                ApplicationStatus = ApplicationStatus.New,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Open",
                EGN = "1111111111",
                PhoneNumber = "0876281932"
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Operator")).Returns(Task.FromResult(true));

            validator.Setup(d => d.ValidateEGN("1111111111")).Returns(Task.FromResult(true));
            validator.Setup(d => d.ValidatePhone("0876281932")).Returns(Task.FromResult(true));

            encrypter.Setup(d => d.Encrypt("1111111111")).Returns("encryptedEGN");
            encrypter.Setup(d => d.Encrypt("0876281932")).Returns("encryptedPhone");

            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.ProcessEmailUpdate(updateParameters);
                Assert.AreEqual(true, executionResult);      
               
                

            }
        }

        [TestMethod]
        public async Task ProcessEmailUpdateShouldReturnFalseWhenInvalidNameIsPassed()
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
                ApplicationStatus = ApplicationStatus.Open,
                Body = "test text",
                OperatorId = "1",
                LastChange = date
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Accepted",
                EGN = "1111111111",
                PhoneNumber = "0876281932",
                FullName="Ivan Petrov3123"
            };

            var user = new User { Id = "1", UserName = "testUser" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.IsInRoleAsync(user, "Operator")).Returns(Task.FromResult(true));

            validator.Setup(d => d.ValidateEGN("1111111111")).Returns(Task.FromResult(true));
            validator.Setup(d => d.ValidatePhone("0876281932")).Returns(Task.FromResult(true));

            validator.Setup(d => d.ValidateName("Ivan Petrov3123")).Returns(Task.FromResult(false));

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
        public async Task ProcessEmailUpdateShouldPassOnStatusAccepted()
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
                ApplicationStatus = ApplicationStatus.Open,
                Body = "test text",
                OperatorId = "1",
                LastChange = date,
                EGN= "1111111111",
                Phone= "0876281932"
            };
            var updateParameters = new EmailUpdateModel
            {
                EmailId = 1,
                LoggedUserUsername = "testUser",
                Amount = 23,
                NewStatus = "Accepted",               
                FullName = "Ivan Petrov"
            };

            var user = new User { Id = "1", UserName = "testUser" };
            var registeredUser = new User { Id = "2", UserName = "kaloyan@abv.bg" };

            userManager.Setup(g => g.FindByNameAsync("testUser")).Returns(Task.FromResult(user));
            userManager.Setup(g => g.FindByEmailAsync("kaloyan@abv.bg")).Returns(Task.FromResult(registeredUser));

            userManager.Setup(g => g.IsInRoleAsync(user, "Operator")).Returns(Task.FromResult(true));

            encrypter.Setup(d => d.Decrypt("kaloyan@abv.bg")).Returns("<kaloyan@abv.bg>");

            validator.Setup(d => d.ValidateEGN("1111111111")).Returns(Task.FromResult(true));
            validator.Setup(d => d.ValidatePhone("0876281932")).Returns(Task.FromResult(true));

            validator.Setup(d => d.ValidateName("Ivan Petrov")).Returns(Task.FromResult(true));

            using (var arrangeContex = new TBIContext(options))
            {
                arrangeContex.Applications.Add(email);
                await arrangeContex.SaveChangesAsync();
            }
            using (var assertContext = new TBIContext(options))
            {
                var sut = new EmailProcessingService(assertContext, encrypter.Object, userManager.Object, validator.Object, emailService);
                var executionResult = await sut.ProcessEmailUpdate(updateParameters);
                Assert.AreEqual(true, executionResult);
                Assert.AreEqual(1, await assertContext.Loans.CountAsync());
                Assert.AreEqual(2, await assertContext.Users.CountAsync());
            }
        }
    }
}
