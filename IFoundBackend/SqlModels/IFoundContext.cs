using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IFoundBackend.SqlModels
{
    public partial class IFoundContext : DbContext
    {
        public IFoundContext()
        {
        }

        public IFoundContext(DbContextOptions<IFoundContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<MxFaceIdentity> MxFaceIdentities { get; set; }
        public virtual DbSet<PostPerson> PostPeople { get; set; }
        public virtual DbSet<PostStatus> PostStatuses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Target> Targets { get; set; }
        public virtual DbSet<TargetPerson> TargetPeople { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=LHE-LT-UKABIR;Initial Catalog=IFound;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>(entity =>
            {
                entity.Property(e => e.ImageId).HasColumnName("imageID");

                entity.Property(e => e.Pic)
                    .HasColumnType("image")
                    .HasColumnName("pic");
            });

            modelBuilder.Entity<MxFaceIdentity>(entity =>
            {
                entity.HasKey(e => e.FaceIdentityId)
                    .HasName("PK__MxFaceId__EDC99EAA2564CFF4");

                entity.Property(e => e.FaceIdentityId)
                    .ValueGeneratedNever()
                    .HasColumnName("faceIdentityID");

                entity.Property(e => e.PostId).HasColumnName("post_Id");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.MxFaceIdentities)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MxFaceIde__post___06CD04F7");
            });

            modelBuilder.Entity<PostPerson>(entity =>
            {
                entity.ToTable("Post_Person");

                entity.Property(e => e.PostPersonId).HasColumnName("postPersonID");

                entity.Property(e => e.ImageId).HasColumnName("imageID");

                entity.Property(e => e.PersonId).HasColumnName("personID");

                entity.Property(e => e.PostDate)
                    .HasColumnType("datetime")
                    .HasColumnName("postDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StatusId)
                    .HasColumnName("statusID")
                    .HasDefaultValueSql("((3))");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.PostPeople)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK__Post_Pers__image__48CFD27E");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PostPeople)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Post_Pers__perso__04E4BC85");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PostPeople)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__Post_Pers__statu__45F365D3");
            });

            modelBuilder.Entity<PostStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__Post_Sta__36257A38F2CB034E");

                entity.ToTable("Post_Status");

                entity.HasIndex(e => e.StatusName, "UQ__Post_Sta__6A50C212570C10E4")
                    .IsUnique();

                entity.Property(e => e.StatusId).HasColumnName("statusID");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("statusName");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__B1947861BE2C7470")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasColumnName("roleID");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("roleName");
            });

            modelBuilder.Entity<Target>(entity =>
            {
                entity.ToTable("Target");

                entity.HasIndex(e => e.TargetName, "UQ__Target__7A3CBD8B02E9E95B")
                    .IsUnique();

                entity.Property(e => e.TargetId).HasColumnName("targetID");

                entity.Property(e => e.TargetName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("targetName");
            });

            modelBuilder.Entity<TargetPerson>(entity =>
            {
                entity.HasKey(e => e.PersonId)
                    .HasName("PK__Target_P__EC7D7D6D78B743FB");

                entity.ToTable("Target_Person");

                entity.Property(e => e.PersonId).HasColumnName("personID");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Description)
                    .HasMaxLength(400)
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

                entity.HasOne(d => d.Target)
                    .WithMany(p => p.TargetPeople)
                    .HasForeignKey(d => d.TargetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Target_Pe__targe__02084FDA");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Cnic)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cnic");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("gender");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_no");

                entity.Property(e => e.State)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.RoleId).HasColumnName("roleID");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.Role)
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__UserRoles__roleI__2F10007B");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserRoles__userI__2E1BDC42");
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
