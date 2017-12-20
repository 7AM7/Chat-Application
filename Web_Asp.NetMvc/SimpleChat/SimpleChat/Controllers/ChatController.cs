using Microsoft.AspNet.Identity;
using SimpleChat.Classes;
using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleChat.Controllers
{
    public class Friendinfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string Status { get; set; }
    }
    public class GroupInfo
    {
        public string GroupName { get; set; }
        public string GroupImage { get; set; }
    }
    public class GroupCreate
    {
        public string GroupName { get; set; }
        public string  GroupImage { get; set; }
        public string UserEmail { get; set; }
        public string GroupCreatorEmail { get; set; }

    }
    public class ChatController : Controller
    {
        // GET: Chat
        private UserContext db = new UserContext();
        ApplicationDbContext tp = new ApplicationDbContext();
        public ActionResult Chat()
        {
            string userName = User.Identity.GetUserName();
            var item = db.UserModels.FirstOrDefault(x => x.UserName == userName);
            if (item != null)
            {
                ViewBag.Pic = item.ProfileImage;
                ViewBag.Name = item.UserName;
                ViewBag.FullName = item.FullName;
            }
            MyFriend();
            MyGroup();
            return View();
        }
        public void MyFriend()
        {
            var Friends = new HashSet<Friendinfo>();
            var PendingFriends = new HashSet<Friendinfo>();
            var userid = User.Identity.GetUserId();
            var users = tp.Users.ToList();
            var fr = db.MyFriends.ToList();
            if (User.Identity.IsAuthenticated)
            {
                foreach (var item2 in fr)
                {
                    if (item2.FromUserId == userid.ToString() && item2.Status == Status.Pending)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item2.ToUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            var frnd = new Friendinfo
                            {
                                Email = item1.UserName,
                                FullName = item1.FullName,
                                ProfileImage = item1.ProfileImage,
                                Status = item1.Status,
                                Id = item1.UserId,
                            };
                            if (item2.FromUserId != userid.ToString())
                                PendingFriends.Add(frnd);
                        }
                        Debug.WriteLine("Pending" + item2.ToUserId);
                    }
                    if (item2.ToUserId == userid.ToString() && item2.Status == Status.Pending)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item2.FromUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            var frnd = new Friendinfo
                            {
                                Email = item1.UserName,
                                FullName = item1.FullName,
                                ProfileImage = item1.ProfileImage,
                                Status = item1.Status,
                                Id = item1.UserId,
                            };
                            PendingFriends.Add(frnd);
                        }
                        Debug.WriteLine("Pending" + item2.FromUserId);
                    }
                    if (item2.FromUserId == userid.ToString() && item2.Status == Status.Accepted)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item2.ToUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            var frnd = new Friendinfo
                            {
                                Email = item1.UserName,
                                FullName = item1.FullName,
                                ProfileImage = item1.ProfileImage,
                                Status = item1.Status,
                                Id = item1.UserId,
                            };
                            Friends.Add(frnd);
                        }

                        Debug.WriteLine("Accepted" + item2.ToUserId);
                    }
                    if (item2.ToUserId == userid.ToString() && item2.Status == Status.Accepted)
                    {
                        var datauser = db.UserModels.Where(x => x.UserId == item2.FromUserId).ToList();
                        foreach (var item1 in datauser)
                        {
                            var frnd = new Friendinfo
                            {
                                Email = item1.UserName,
                                FullName = item1.FullName,
                                ProfileImage = item1.ProfileImage,
                                Status = item1.Status,
                                Id = item1.UserId,
                            };
                            Friends.Add(frnd);
                        }
                        Debug.WriteLine("Accepted" + item2.FromUserId);
                    }
                }
                ViewBag.Friends = Friends;
                ViewBag.PendingFriends = PendingFriends;
                Debug.WriteLine("friend: " + Friends.Count);

                Debug.WriteLine("PendingFriends: " + PendingFriends.Count);
            }
        }
        public void MyGroup()
        {
            var Group = new HashSet<GroupInfo>();
            string userName = User.Identity.GetUserName();
            //admin
            var admingroup = db.GroupDetails.Where(x => x.CreatorEmail == userName).ToList();
            if (admingroup != null)
            {
                foreach (var item in admingroup)
                {
                    var grpoinfo = new GroupInfo
                    {
                        GroupName = item.GroupName,
                        GroupImage=item.GroupImage,
                    };
                    Group.Add(grpoinfo);
                }

               
            }
            var group = db.GroupMember.Where(x => x.UserEmail == userName).ToList();
            if (group != null)
            {
                foreach (var item in group)
                {
                    var gropname = db.GroupDetails.Where(x => x.GroupID == item.GroupId).ToList();
                    if (gropname != null)
                    {
                        foreach (var item2 in gropname)
                        {
                            var grpoinfo = new GroupInfo
                            {
                                GroupName = item2.GroupName,
                                GroupImage = item2.GroupImage,
                            };

                            Group.Add(grpoinfo);
                        }

                    }
                }

            }
            ViewBag.Groups = Group;
        }
        [HttpPost]
        public JsonResult UploadGroupImage()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpPostedFileBase userImage = Request.Files["imgInp"];

                    var pic = string.Empty;
                    var folder = "~/Images";

                    if (userImage != null)
                    {
                        pic = FilesHelper.UploadPhoto(userImage, folder);
                        pic = string.Format("{0}/{1}", folder, pic);
                    }
                    return new JsonResult { Data = pic, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                catch (Exception ex)
                {

                    return new JsonResult { Data = "Error", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                return new JsonResult { Data = "No Item Selected", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


          

        public ActionResult Login()
        {
            return View();
        }
    }
}