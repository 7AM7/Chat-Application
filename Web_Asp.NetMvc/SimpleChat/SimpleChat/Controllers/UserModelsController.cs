using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimpleChat.Models;
using SimpleChat.Classes;
using Microsoft.AspNet.Identity;
using System.IO;

namespace SimpleChat.Controllers
{
    public class UserModelsController : Controller
    {
        private UserContext db = new UserContext();
        
        public IQueryable<UserModels> GetDataForCurrentUser()
        {
            string userId = User.Identity.GetUserId();

            return db.UserModels.Where(usr => usr.UserId == userId);
        }


        [Authorize]
        // GET: UserModels
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            return View(db.UserModels.Where(usr => usr.UserId == userId).ToList());
        }
        [Authorize]
        // GET: UserModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModels userModels = db.UserModels.Find(id);
            if (userModels == null)
            {
                return HttpNotFound();
            }

            return View(userModels);
        }

        // GET: UserModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserView view)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Images";

                if (view.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.ImageFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }
                string userId = User.Identity.GetUserId();
                view.UserId = userId;
                string userName = User.Identity.GetUserName();
                view.UserName = userName;
                var user = ToUser(view);
                user.FullName= string.Format("{0} {1}", user.FirstName, user.LastName);
                user.ProfileImage = pic;
                db.UserModels.Add(user);
                db.SaveChanges();
                return RedirectToAction("Chat", "Chat");
            }

            return View(view);
        }

        private UserModels ToUser(UserView view)
        {
            return new UserModels
            {
                Id = view.Id,
                FirstName = view.FirstName,
                LastName = view.LastName,
                ProfileImage = view.ProfileImage,
                UserId = view.UserId,
                UserName = view.UserName,
                BirthDay = view.BirthDay,
                Gander = view.Gander,
                Phone = view.Phone,
                Status = view.Status,
                

            };
        }
        [Authorize]
        // GET: UserModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModels userModels = db.UserModels.Find(id);
            if (userModels == null)
            {
                return HttpNotFound();
            }
            var view = ToView(userModels);
            return View(view);
            // return View(userModels);
        }
        private UserView ToView(UserModels usermodels)
        {
            return new UserView
            {
                Id = usermodels.Id,
                FirstName = usermodels.FirstName,
                LastName = usermodels.LastName,
                ProfileImage = usermodels.ProfileImage,
                UserId = usermodels.UserId,
                UserName = usermodels.UserName,
                Gander = usermodels.Gander,
                Phone = usermodels.Phone,
                Status = usermodels.Status,
                FullName = usermodels.FullName,
            };
        }
        
        // POST: UserModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserView view)
        {
            if (ModelState.IsValid)
            {
                var pic = view.ProfileImage;
                var folder = "~/Images";

                if (view.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.ImageFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }
                var userId = User.Identity.GetUserId();
                view.UserId = userId;
                string userName = User.Identity.GetUserName();
                view.UserName = userName;
              //  
                if (userId != view.UserId)
                {
                    return HttpNotFound();
                }
                if (userName != view.UserName)
                {
                    return HttpNotFound();
                }
                var user = ToUser(view);
                if (pic != null&&user.ProfileImage == null)
                {          
                       user.ProfileImage = pic;
                }

                user.FullName = string.Format("{0} {1}", user.FirstName, user.LastName);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Chat", "Chat");
            }
            return View(view);
        }

        // GET: UserModels/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModels userModels = db.UserModels.Find(id);
            if (userModels == null)
            {
                return HttpNotFound();
            }
            return View(userModels);
        }

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserModels userModels = db.UserModels.Find(id);
            db.UserModels.Remove(userModels);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
