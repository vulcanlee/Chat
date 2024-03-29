﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainData.Models;

public partial class ChatDBContext : DbContext
{
    public ChatDBContext()
    {
    }

    public ChatDBContext(DbContextOptions<ChatDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<ExceptionRecord> ExceptionRecord { get; set; }
    public virtual DbSet<ChatRoom> ChatRoom { get; set; }
    public virtual DbSet<ChatRoomMember> ChatRoomMember { get; set; }
    public virtual DbSet<ChatRoomMessage> ChatRoomMessage { get; set; }
    public virtual DbSet<OtpVerifyHistory> OtpVerifyHistory { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CI_AS");

        #region 設定階層級的刪除政策(預設若關聯子資料表有紀錄，父資料表不可強制刪除
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        #endregion

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
