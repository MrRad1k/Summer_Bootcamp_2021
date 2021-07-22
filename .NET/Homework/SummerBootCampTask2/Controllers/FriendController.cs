using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask2.Contexts;
using SummerBootCampTask2.CoreModels;
using System.Collections.Generic;
using System.Linq;

namespace SummerBootCampTask2.Controllers
{
    public class FriendController : Controller
    {
        private readonly BootCampDbContext dbContext;

        public FriendController(BootCampDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var users = dbContext.Users.ToList();
            List<User> Friends = new List<User>();
            int UserId = default;

            if (User.Identity.IsAuthenticated)
            {
                foreach (var user in users)
                {
                    if (User.Identity.Name == user.Email)
                    {
                        UserId = user.Id;
                    }
                }

                var userFriends = dbContext.UserFriends.ToList();
                foreach (var userFriend in userFriends)
                {
                    if (UserId == userFriend.UserId)
                    {
                        if (userFriend.IsVerified == true)
                        {
                            foreach (var friend in users)
                            {
                                if (userFriend.FriendId == friend.Id)
                                {
                                    Friends.Add(friend);
                                }
                            }
                        }
                    }

                    if (UserId == userFriend.FriendId)
                    {
                        if (userFriend.IsVerified == true)
                        {
                            foreach (var friend in users)
                            {
                                if (userFriend.UserId == friend.Id)
                                {
                                    Friends.Add(friend);
                                }
                            }
                        }
                    }
                }
            }

            return View(Friends);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var userFriend = dbContext.UserFriends.FirstOrDefault(userFriend => userFriend.FriendId != id);

            dbContext.UserFriends.Remove(userFriend);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "Friend");
        }
    }
}
