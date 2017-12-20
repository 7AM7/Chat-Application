using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SimpleChat.Models;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using System.Web.Mvc;
using SimpleChat.Controllers;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SimpleChat.Classes;
using System.IO;
using System.Text;

namespace SimpleChat.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public static string emailIDLoaded = "";

        //private UserContext db = new UserContext();
        public void Connect(string userName, string email)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                emailIDLoaded = email;
                var Conid = Context.ConnectionId;
                var Authid = Context.User.Identity.GetUserId().ToString();
                using (UserContext dc = new UserContext())
                {
                    var item = dc.ChatUserDetail.FirstOrDefault(x => x.EmailID == email);
                    if (item != null)
                    {
                        dc.ChatUserDetail.Remove(item);
                        dc.SaveChanges();

                        // Disconnect
                        Clients.All.onUserDisconnectedExisting(item.ConnectionId, item.FullName);
                    }

                    var Users = dc.ChatUserDetail.ToList();
                    if (Users.Where(x => x.EmailID == email).ToList().Count == 0)
                    {
                        var userdetails = new ChatUserDetail
                        {
                            ConnectionId = Conid,
                            AuthId = Authid,
                            FullName = userName,
                            EmailID = email
                        };
                        dc.ChatUserDetail.Add(userdetails);
                        dc.SaveChanges();

                        // send to caller
                        var connectedUsers = dc.ChatUserDetail.ToList();
                        var CurrentMessage = dc.ChatAllMessageDetail.ToList();
                        Clients.Caller.onConnected(Conid, userName, connectedUsers, CurrentMessage);
                    }

                    // send to all except caller client
                    Clients.AllExcept(Conid).onNewUserConnected(Conid, userName, email);

                }
            }
            else
            {
                if (Context.User.Identity.Name == null)
                {
                    OnDisconnected(true);
                }
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var Conid = Context.ConnectionId;
            using (UserContext dc = new UserContext())
            {
                var item = dc.ChatUserDetail.FirstOrDefault(x => x.ConnectionId == Conid);
                if (item != null)
                {
                    dc.ChatUserDetail.Remove(item);
                    dc.SaveChanges();

                    Clients.All.onUserDisconnected(Conid, item.FullName);

                }
            }
            return base.OnDisconnected(stopCalled);
        }
        public void AddPrivateMessageinCache(string fromEmail, string chatToEmail, string message)
        {
            using (UserContext dc = new UserContext())
            {
                var resultDetails = new ChatPrivateMessageDetail
                {
                    MasterEmailID = fromEmail,
                    ChatToEmailID = chatToEmail,
                    Message = message,
                    MessageData = DateTime.Now,
                };
                dc.ChatPrivateMessageDetail.Add(resultDetails);
                dc.SaveChanges();
            }
        }

        //public List<PrivateChatMessage> GetScrollingChatData(string fromid, string toid, int start = 10, int length = 1)
        //{
        //    takeCounter = (length * start); // 20
        //    skipCounter = ((length - 1) * start); // 10

        //    using (UserContext dc = new UserContext())
        //    {
        //        List<PrivateChatMessage> msg = new List<PrivateChatMessage>();
        //        var v = (from a in dc.UserModels
        //                 join b in dc.ChatPrivateMessageDetail on a.UserName equals b.MasterEmailID into cc
        //                 from c in cc
        //                 where (c.MasterEmailID.Equals(fromid) && c.ChatToEmailID.Equals(toid)) || (c.MasterEmailID.Equals(toid) && c.ChatToEmailID.Equals(fromid))
        //                 orderby c.MessageData descending
        //                 select new
        //                 {
        //                     UserName = a.UserName,
        //                     Message = c.Message,
        //                     ID = c.ID,
        //                     MessageData=c.MessageData
        //                 }).Take(takeCounter).Skip(skipCounter).ToList();

        //        foreach (var a in v)
        //        {
        //            var res = new PrivateChatMessage()
        //            {
        //                userName = a.UserName,
        //                message = a.Message,
        //                msgData = a.MessageData,

        //            };
        //            msg.Add(res);
        //        }
        //        return msg;
        //    }
        //}
        public List<PrivateChatMessage> GetPrivateMessage(string fromid, string toid, int take)
        {
            using (UserContext dc = new UserContext())
            {
                List<PrivateChatMessage> msg = new List<PrivateChatMessage>();

                var v = (from a in dc.UserModels
                         join b in dc.ChatPrivateMessageDetail on a.UserName equals b.MasterEmailID into cc
                         from c in cc
                         where (c.MasterEmailID.Equals(fromid) && c.ChatToEmailID.Equals(toid)) || (c.MasterEmailID.Equals(toid) && c.ChatToEmailID.Equals(fromid))
                         orderby c.MessageData descending
                         select new
                         {
                             UserName = a.UserName,
                             Message = c.Message,
                             ID = c.ID,
                             MessageData = c.MessageData,
                             Image = a.ProfileImage
                         }).Take(take).ToList();
                v = v.OrderBy(s => s.MessageData).ToList();

                foreach (var a in v)
                {
                    var res = new PrivateChatMessage()
                    {
                        userName = a.UserName,
                        message = a.Message,
                        msgData = a.MessageData.ToString("hh:mm tt"),
                        Image = a.Image
                    };
                    msg.Add(res);
                }
                return msg;
            }
        }
        public void SendPrivateMessage(string toUserId, string message, string status)
        {
            string fromUserId = Context.User.Identity.GetUserId().ToString();
            string fromConId = Context.ConnectionId;
            DateTime msgDate = DateTime.Now; 
            using (UserContext dc = new UserContext())
            {
                var toUser = dc.ChatUserDetail.FirstOrDefault(x => x.ConnectionId == toUserId);
                var fromUser = dc.ChatUserDetail.FirstOrDefault(x => x.AuthId == fromUserId);
                // var topic = dc.UserModels.FirstOrDefault(x => x.UserName == toUser.EmailID).ProfileImage;
                var frompic = dc.UserModels.FirstOrDefault(x => x.UserName == fromUser.EmailID).ProfileImage;
                if (toUser != null && fromUser != null)
                {
                    if (status == "Click")
                        AddPrivateMessageinCache(fromUser.EmailID, toUser.EmailID, message);

                    // send to 
                    // Clients.All.sendPrivateMessage(fromUserId, fromUser.FullName, message, fromUser.EmailID, toUser.EmailID, status, fromUserId);
                    Clients.Client(toUserId).sendPrivateMessage(fromUser.ConnectionId, fromUser.FullName, message, fromUser.EmailID, toUser.EmailID, status, fromUser.ConnectionId, msgDate.ToString("hh:mm tt"), frompic);

                    // send to caller user
                    Clients.Caller.sendPrivateMessage(toUserId, fromUser.FullName, message, fromUser.EmailID, toUser.EmailID, status, fromUser.ConnectionId, msgDate.ToString("hh:mm tt"), frompic);
                }
            }
        }

        //------------------Group-----------------------//

        public void CreateGroup(string groupName, string groupImage, string groupCreatorEmail)
        {
            using (UserContext dc = new UserContext())
            {

                var groupdet = dc.GroupDetails.Count(x => x.GroupName == groupName && x.GroupImage == groupImage);
                if (groupdet == 0)
                {
                    var groupDetail = new GroupDetails
                    {
                        GroupName = groupName,
                        CreatorEmail = groupCreatorEmail,
                        CreatedAt = DateTime.Now,
                        GroupImage = groupImage

                    };
                    dc.GroupDetails.Add(groupDetail);
                    dc.SaveChanges();
                }
            }
            Clients.Group(groupName).creatGroup(groupName);
        }
        public async Task AddMemberGroup(string groupName, string useremail)
        {

            using (UserContext dc = new UserContext())
            {
                var groupid = dc.GroupDetails.FirstOrDefault(x => x.GroupName == groupName);
                var groupmember = dc.GroupMember.Count(x => x.GroupId == groupid.GroupID && x.UserEmail == useremail);
                if (groupid != null && groupmember == 0)
                {
                    var groupMember = new GroupMember
                    {
                        GroupId = groupid.GroupID,
                        JoinedAt = DateTime.Now,
                        UserEmail = useremail,
                    };
                    dc.GroupMember.Add(groupMember);
                    dc.SaveChanges();
                    var toUser = dc.ChatUserDetail.FirstOrDefault(x => x.EmailID == useremail);
                    if (toUser != null)
                    {
                        await Groups.Add(toUser.ConnectionId, groupName);
                        Clients.Group(groupName).addMemberGroup(groupName);
                    }
                }
            }
        }

        public async Task SendRoomMessage(string groupName, string message)
        {
            DateTime msgDate = DateTime.Now; 
            using (UserContext dc = new UserContext())
            {
                
                var fromUsr = dc.ChatUserDetail.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                var groupid = dc.GroupDetails.FirstOrDefault(x => x.GroupName == groupName);
                var frompic = dc.UserModels.FirstOrDefault(x => x.UserName == fromUsr.EmailID).ProfileImage;
                
                if (fromUsr != null && groupid!=null && frompic!=null)
                {
                    AddGroupMessageinCache(fromUsr.EmailID, groupid.GroupID, message);
                   await Groups.Add(Context.ConnectionId, groupName);
                    Clients.Group(groupName).addChatMessage(groupName, message, fromUsr.FullName, frompic, msgDate.ToString("hh:mm tt"));
                    //Clients.Caller.addChatMessage(groupName, message, fromUsr.FullName, frompic);
                }
            }
        }
        public void AddGroupMessageinCache(string fromEmail, int GroupId, string message)
        {
            using (UserContext dc = new UserContext())
            {
                var groupmessage = new GroupMessage
                {
                    Message = message,
                    SenderEmail = fromEmail,
                    MessageData = DateTime.Now,
                    GroupID = GroupId,
                };
                dc.GroupMessage.Add(groupmessage);
                dc.SaveChanges();
            }
        }
        public async Task<List<PrivateChatMessage>> GetGroupMessage(string grouName, int take)
        {
            await Groups.Add(Context.ConnectionId, grouName);
            using (UserContext dc = new UserContext())
            {
                List<PrivateChatMessage> msg = new List<PrivateChatMessage>();
                var groupId = dc.GroupDetails.FirstOrDefault(x => x.GroupName == grouName);
                var mesg = dc.GroupMessage.Where(x => x.GroupID == groupId.GroupID).ToList();
                foreach (var item in mesg)
                {
                    var usermodel =dc.UserModels.FirstOrDefault(x=>x.UserName==item.SenderEmail);
                    var res = new PrivateChatMessage()
                    {
                        msgData = item.MessageData.ToString("hh:mm tt"),
                        userName=usermodel.FullName,
                        message=item.Message,
                        Image=usermodel.ProfileImage,
                    };
                    msg.Add(res);
                }
                return msg;
            }
        }
    }

    public class PrivateChatMessage
    {
        public string userName { get; set; }
        public string message { get; set; }
        public string msgData { get; set; }
        public string Image { get; set; }
    }
}