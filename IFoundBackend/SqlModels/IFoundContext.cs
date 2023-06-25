using System;
using System.Collections.Generic;
using IFoundBackend.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IFoundBackend.SqlModels;

public partial class IfoundContext : IdentityDbContext<ApplicationUser>
{
    public IfoundContext()
    {
    }

    public IfoundContext(DbContextOptions<IfoundContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<MxFaceIdentity> MxFaceIdentities { get; set; }

    public virtual DbSet<PostPerson> PostPeople { get; set; }

    public virtual DbSet<PostStatus> PostStatuses { get; set; }

    public virtual DbSet<Target> Targets { get; set; }

    public virtual DbSet<TargetPerson> TargetPeople { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=sql-server-algo.database.windows.net;Initial Catalog=IFound;User ID=usman-admin;Password=webdir123R@");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__336E9B750F1C6527");

            entity.Property(e => e.ImageId).HasColumnName("imageID");
            entity.Property(e => e.Pic)
                .HasColumnType("image")
                .HasColumnName("pic");
        });

        modelBuilder.Entity<MxFaceIdentity>(entity =>
        {
            entity.HasKey(e => e.FaceIdentityId).HasName("PK__MxFaceId__EDC99EAA216889E1");

            entity.Property(e => e.FaceIdentityId)
                .ValueGeneratedNever()
                .HasColumnName("faceIdentityID");
            entity.Property(e => e.PostId).HasColumnName("post_Id");

            entity.HasOne(d => d.Post).WithMany(p => p.MxFaceIdentities)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__MxFaceIde__post___06CD04F7");
        });

        modelBuilder.Entity<PostPerson>(entity =>
        {
            entity.HasKey(e => e.PostPersonId).HasName("PK__Post_Per__850A6D739E4824C6");

            entity.ToTable("Post_Person");

            entity.Property(e => e.PostPersonId).HasColumnName("postPersonID");
            entity.Property(e => e.ImageId).HasColumnName("imageID");
            entity.Property(e => e.PersonId).HasColumnName("personID");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.PostDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("postDate");
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("((3))")
                .HasColumnName("statusID");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnName("userID");

            entity.HasOne(d => d.Image).WithMany(p => p.PostPeople)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK__Post_Pers__image__6D0D32F4");

            entity.HasOne(d => d.Person).WithMany(p => p.PostPeople)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post_Pers__perso__6B24EA82");

            entity.HasOne(d => d.Status).WithMany(p => p.PostPeople)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Post_Pers__statu__6C190EBB");
        });

        modelBuilder.Entity<PostStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Post_Sta__36257A38DD07402C");

            entity.ToTable("Post_Status");

            entity.HasIndex(e => e.StatusName, "UQ__Post_Sta__6A50C212E6928435").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("statusName");
        });

        modelBuilder.Entity<Target>(entity =>
        {
            entity.HasKey(e => e.TargetId).HasName("PK__Target__30088013D92D344A");

            entity.ToTable("Target");

            entity.HasIndex(e => e.TargetName, "UQ__Target__7A3CBD8BD071F0C8").IsUnique();

            entity.Property(e => e.TargetId).HasColumnName("targetID");
            entity.Property(e => e.TargetName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("targetName");
        });

        modelBuilder.Entity<TargetPerson>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Target_P__EC7D7D6D33CE2A6E");

            entity.ToTable("Target_Person");

            entity.Property(e => e.PersonId).HasColumnName("personID");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(266)
                .IsUnicode(false);
            entity.Property(e => e.Relation)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("relation");
            entity.Property(e => e.TargetId).HasColumnName("targetID");

            entity.HasOne(d => d.Target).WithMany(p => p.TargetPeople)
                .HasForeignKey(d => d.TargetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Target_Pe__targe__6E01572D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
