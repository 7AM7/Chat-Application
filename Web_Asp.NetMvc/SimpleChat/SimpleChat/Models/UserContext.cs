using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class UserContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public UserContext() : base("name=DefaultConnection")
        {
        }

        public System.Data.Entity.DbSet<SimpleChat.Models.UserModels> UserModels { get; set; }

        public System.Data.Entity.DbSet<SimpleChat.Models.MyFriends> MyFriends { get; set; }
        public System.Data.Entity.DbSet<SimpleChat.Models.ChatAllMessageDetail> ChatAllMessageDetail { get; set; }
       //public System.Data.Entity.DbSet<SimpleChat.Models.ChatPrivateMessageMaster> ChatPrivateMessageMaster { get; set; }
        public System.Data.Entity.DbSet<SimpleChat.Models.ChatPrivateMessageDetail> ChatPrivateMessageDetail { get; set; }
        public System.Data.Entity.DbSet<SimpleChat.Models.ChatUserDetail> ChatUserDetail { get; set; }
        public System.Data.Entity.DbSet<SimpleChat.Models.GroupDetails> GroupDetails { get; set; }
        public System.Data.Entity.DbSet<SimpleChat.Models.GroupMessage> GroupMessage { get; set; }
        public System.Data.Entity.DbSet<SimpleChat.Models.GroupMember> GroupMember { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
