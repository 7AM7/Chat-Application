using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimpleChat.Models;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Configuration;

namespace SimpleChat.Controllers
{
    public class FriendsController : Controller
    {
        private UserContext db = new UserContext();

        // GET: Friends
        public ActionResult Index()
        {

            return View(db.MyFriends.ToList());
           
        }

        // GET: Friends/Details/5
        public ActionResult Details()
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //var friend = db.Friends.Where(c => c.FromUserId == id || c.ToUserId == id).ToList();
            //foreach (var item in friend)
            //{
            //    friends = db.Friends.Find(item.ToUserId);
            //}
            string userid = User.Identity.GetUserId();
            string t = "";
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT * FROM [dbo].[AspNetUsers] where Id IN (SELECT ToUserId  FROM [dbo].[MyFriends] WHERE FromUserId='" + userid + "' AND Status=1)  ", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        ViewBag.t = reader.GetString(2).ToString();
                    }
                }
            }
            
                    // string query = "Select * FROM [Register]  where RegisterId IN (SELECT MyId  FROM Friends WHERE FriendId='" + Session["UserId"] + "' AND Status=0) ";
                    return View(ViewBag.t);
        }

        // GET: Friends/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Friends/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,FromUserId,ToUserId,Status")] MyFriends friends)
        {
            if (ModelState.IsValid)
            {
                db.MyFriends.Add(friends);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(friends);
        }

        // GET: Friends/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyFriends friends = db.MyFriends.Find(id);

            if (friends == null)
            {
                return HttpNotFound();
            }
            return View(friends);
        }

        // POST: Friends/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FromUserId,ToUserId,Status")] MyFriends friends)
        {
            if (ModelState.IsValid)
            {
                db.Entry(friends).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(friends);
        }

        // GET: Friends/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyFriends friends = db.MyFriends.Find(id);
            if (friends == null)
            {
                return HttpNotFound();
            }
            return View(friends);
        }

        // POST: Friends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MyFriends friends = db.MyFriends.Find(id);
            db.MyFriends.Remove(friends);
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
