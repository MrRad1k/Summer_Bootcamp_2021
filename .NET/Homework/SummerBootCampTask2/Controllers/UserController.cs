using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SummerBootCampTask2.Contexts;
using SummerBootCampTask2.CoreModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask2.Controllers
{
    public class UserController : Controller
    {
        private readonly BootCampDbContext dbContext;

        public UserController(BootCampDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //public IActionResult Index()
        //{
        //    var users = dbContext.Users.ToList();
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        users = users.Where(x => x.Email != User.Identity.Name).ToList();
        //    }
        //    return View(users);
        //}

        public async Task<IActionResult> Index(string searchUser)
        {
            var users = dbContext.Users.Select(x => x);

            if (User.Identity.IsAuthenticated)
            {
                users = users.Where(x => x.Email != User.Identity.Name);
            }

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                int identifier = default;
                if (int.TryParse(searchUser, out identifier))
                {
                    users = users.Where(x => x.Identifier == identifier);
                }
                else
                {
                    users = users.Where(x => x.UserName.Contains(searchUser) && x.Visible);
                }
            }
            else
            {
                users = users.Where(x => x.Visible);
            }

            return View(await users.ToListAsync());
        }

        [Authorize]
        public IActionResult Friends(string searchUser)
        {
            var userFriends = dbContext.UserFriends.ToList();
            var user = dbContext.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            var friendsFirst = userFriends.Where(x => x.UserId == user.Id && x.IsVerified);
            var friendsSecond = userFriends.Where(x => x.FriendId == user.Id && x.FriendId != x.UserId && x.IsVerified);

            var model = new List<FriendsViewModel>();

            GetFriends(model, friendsFirst, false);
            GetFriends(model, friendsSecond, true);

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                model = model.Where(x => x.UserName.Contains(searchUser)).ToList();
            }

            return View(model.OrderByDescending(x => x.UserName));
        }
        private void GetFriends(List<FriendsViewModel> model, IEnumerable<UserFriend> friends, bool bl)
        {
            foreach (var item in friends)
            {
                var idfriend = bl ? item.UserId : item.FriendId;
                var friend = dbContext.Users.FirstOrDefault(x => x.Id == idfriend);

                model.Add(new FriendsViewModel
                {
                    FriendId = friend.Id,
                    UserName = friend.UserName,
                });
            }
        }
    }
}
