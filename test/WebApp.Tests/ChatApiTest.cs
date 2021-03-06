using System;
using Xunit;
using WebApp.Controllers;
using WebApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Tests
{
    public class ChatApiTest
    {
        private static int database = 0;
        private static MyContext _context;
        private static Mock<UserManager<ApplicationUser>> _userManagerMock;

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((x, y) => { _context.Users.Add(x); _context.SaveChanges(); });
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.Users).Returns(_context.Users);

            return mgr;
        }

        private static ChatApi CreateController() {
            database++;
            _context = new MyContext( new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("TemporaryDatabase" + database).Options );
            _userManagerMock = MockUserManager();
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "1" });
            _userManagerMock.Object.CreateAsync(new ApplicationUser(){ Id = "2" });
            _context.Users.Single(u => u.Id == "1")
                .PrivateChats
                .Add(new PrivateChat()
                {
                    Id = 1,
                    Name = "TestChat",
                    Users = _context.Users.ToList(),
                    Messages = new List<Message>()
                    {
                        new Message()
                        {
                            DateTimeSent = DateTime.Now,
                            Sender = _context.Users.Single(u => u.Id == "1"),
                            Text = "Test1"
                        },
                        new Message()
                        {
                            DateTimeSent = DateTime.Now,
                            Sender = _context.Users.Single(u => u.Id == "2"),
                            Text = "Test2"
                        }
                    }
                });
            _context.SaveChanges();
            return new ChatApi(_context, _userManagerMock.Object);
        }

        [Fact]
        public void GetChatTest() 
        {
            var chatapi = CreateController();

            var viewResult = Xunit.Assert.IsType<ViewResult>(chatapi.GetChat(1));
            var viewModel = Xunit.Assert.IsType<Chat>(viewResult.Model);
            Xunit.Assert.Equal("TestChat", viewModel.Name);
        }

        [Fact]
        public void PutChatTest() 
        {
            
        }

        [Fact]
        public void PostChatTest() 
        {

        }

        [Fact]
        public void PostMessageTest() 
        {

        }

        [Fact]
        public void DeleteChatTest() 
        {

        }

        [Fact]
        public void ChatExists()
        {
            
        }

        [Fact]
        public void GetChatsTest()
        {

        }

    }
}
