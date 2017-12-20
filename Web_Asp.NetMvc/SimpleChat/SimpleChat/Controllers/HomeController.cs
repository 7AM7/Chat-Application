using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SimpleChat.Controllers
{
    public class User
    {
        public string myId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public Gender Gander { get; set; }
    }

    public class HomeController : Controller
    {
        private UserContext db = new UserContext();
        ApplicationDbContext tp = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyFriend()
        {
            var t = new List<string>();
            var userid = User.Identity.GetUserId();
            var users = tp.Users.ToList();
            foreach (var item2 in users)
            {
                if (item2.Id == userid.ToString())
                {

                    var friends = db.MyFriends.Where(x => x.ToUserId == userid.ToString() && x.Status == Status.Pending).ToList();
                    foreach (var item in friends)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item.FromUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            t.Add(item1.FullName);
                        }
                    }
                }
                if (item2.Id == userid.ToString())
                {
                    
                    var friends = db.MyFriends.Where(x => x.FromUserId == userid.ToString() && x.Status == Status.Accepted).ToList();
                    foreach (var item in friends)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item.ToUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            t.Add(item1.FullName);
                        }
                    }

                }
                else{

                    var friends = db.MyFriends.Where(x => x.ToUserId == userid.ToString() && x.Status == Status.Accepted).ToList();
                    foreach (var item in friends)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item.FromUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            t.Add(item1.FullName);
                        }
                    }
                }
            }
            ViewBag.name = t;
            return View();
        }
        [HttpPost]
        public JsonResult GetSearchValue(string search)
        {
            try
            {
            var userid = User.Identity.GetUserId();
            var user = db.UserModels.FirstOrDefault(x => x.UserId == userid);
            List<User> allsearch = db.UserModels.Where(x => x.FullName.Contains(search.ToLower())&&x.FullName!= user.FullName.ToLower()).Select(x => new User
            {
                myId = userid.ToString(),
                 FullName = x.FullName.ToLower(),
                ProfileImage = x.ProfileImage,
                UserName = x.UserName,
                UserId = x.UserId,
                Status=x.Status,
                Gander=x.Gander,
            }).ToList();
            return new JsonResult { Data = allsearch, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = "Problem! Check Your Login....", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [HttpPost]
        public JsonResult CheckingRequest(MyFriends data1)
        {
            var response = "";
            data1 = new MyFriends
            {
                FromUserId = data1.FromUserId,
                ToUserId = data1.ToUserId,
            };

            if (CheckFriendShip(data1.FromUserId, data1.ToUserId, "Pending") == 1)
            {
                response = "Request Pending";
            }
            else
            {
                if (CheckFriendShip(data1.FromUserId, data1.ToUserId, "isFriends") == 0)
                {
                    response = "Add as friend";

                }
                else
                {
                    response = "Unfriend";
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        private int CheckFriendShip(string fromid, string toid,string type)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(fromid) && !string.IsNullOrEmpty(toid))
            {
                switch(type)
                {
                    case "Pending":
                       count= db.MyFriends.Count(usr => usr.FromUserId == fromid && usr.ToUserId==toid&&usr.Status== Status.Pending || usr.FromUserId == toid && usr.ToUserId == fromid && usr.Status == Status.Pending);
                        break;
                    case "isFriends":
                        count = db.MyFriends.Count(usr => usr.FromUserId == fromid && usr.ToUserId == toid && usr.Status == Status.Accepted || usr.FromUserId == toid && usr.ToUserId == fromid && usr.Status == Status.Accepted);
                        break;
                }
            }
            return count;
        }
        [HttpPost]
        public JsonResult AddFriend(MyFriends data)
        {
            var response="";
          
            data = new MyFriends
            {
             
                FromUserId = data.FromUserId,
                ToUserId = data.ToUserId,
                Status = data.Status,
                Data = DateTime.Now,
            };
            if (!string.IsNullOrEmpty(data.FromUserId) && !string.IsNullOrEmpty(data.ToUserId))
            {
               if(data.FromUserId!= data.ToUserId)
                {
                    var check = CheckFriendShip(data.FromUserId, data.ToUserId, "isFriends");
                    if (check == 0)
                    {
                        db.MyFriends.Add(data);
                        db.SaveChanges();
                        response = "Request Sent!";
                    }
                    else
                    {
                        response = "Already Friends!";
                    }
                }
               else
                {
                    response = "You Cant friend yourself";
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AcceptFriend(MyFriends data)
        {
            var response = "";
            var user = db.MyFriends.FirstOrDefault(x => x.FromUserId == data.FromUserId && x.ToUserId == data.ToUserId && x.Status == Status.Pending);
            if (!string.IsNullOrEmpty(data.FromUserId) && !string.IsNullOrEmpty(data.ToUserId))
            {
                if (data.FromUserId != data.ToUserId)
                {
                    var check = CheckFriendShip(data.FromUserId, data.ToUserId, "Pending");
                    if (check == 1)
                    {
                        data.Id = user.Id;
                        data.Status = Status.Accepted;
                        using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            try
                            {
                                connection.Open();
                                using (SqlCommand command = new SqlCommand(@"UPDATE [dbo].[MyFriends] SET FromUserId = @FromUserId, ToUserId = @ToUserId , Status = @Status, Data = @Data Where Id=" + data.Id + "", connection))
                                {
                                    command.Parameters.AddWithValue("@FromUserId", data.FromUserId);
                                    command.Parameters.AddWithValue("@ToUserId", data.ToUserId);
                                    command.Parameters.AddWithValue("@Status", data.Status);
                                    command.Parameters.AddWithValue("@Data", DateTime.Now);
                                    command.ExecuteNonQuery();
                                    response = "Done";
                                }
                            }
                            catch (Exception ex)
                            {
                                response = ex.Message;
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
