﻿// <auto-generated />
using System;
using IFoundBackend.SqlModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IFoundBackend.Migrations
{
    [DbContext(typeof(IfoundContext))]
    partial class IfoundContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("IFoundBackend.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cnic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("imageID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageId"));

                    b.Property<byte[]>("Pic")
                        .HasColumnType("image")
                        .HasColumnName("pic");

                    b.HasKey("ImageId")
                        .HasName("PK__Images__336E9B75B4FC25C9");

                    b.ToTable("Images", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.MxFaceIdentity", b =>
                {
                    b.Property<int>("FaceIdentityId")
                        .HasColumnType("int")
                        .HasColumnName("faceIdentityID");

                    b.Property<int>("PostId")
                        .HasColumnType("int")
                        .HasColumnName("post_Id");

                    b.HasKey("FaceIdentityId")
                        .HasName("PK__MxFaceId__EDC99EAA2564CFF4");

                    b.HasIndex("PostId");

                    b.ToTable("MxFaceIdentities", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.PostPerson", b =>
                {
                    b.Property<int>("PostPersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("postPersonID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostPersonId"));

                    b.Property<int?>("ImageId")
                        .HasColumnType("int")
                        .HasColumnName("imageID");

                    b.Property<int>("PersonId")
                        .HasColumnType("int")
                        .HasColumnName("personID");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)")
                        .HasColumnName("phone");

                    b.Property<DateTime?>("PostDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("postDate")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("statusID")
                        .HasDefaultValueSql("((3))");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userID");

                    b.HasKey("PostPersonId")
                        .HasName("PK__Post_Per__850A6D7310051343");

                    b.HasIndex("ImageId");

                    b.HasIndex("PersonId");

                    b.HasIndex("StatusId");

                    b.ToTable("Post_Person", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.PostStatus", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("statusID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StatusId"));

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("statusName");

                    b.HasKey("StatusId")
                        .HasName("PK__Post_Sta__36257A38F2CB034E");

                    b.HasIndex(new[] { "StatusName" }, "UQ__Post_Sta__6A50C212570C10E4")
                        .IsUnique();

                    b.ToTable("Post_Status", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.Target", b =>
                {
                    b.Property<int>("TargetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("targetID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TargetId"));

                    b.Property<string>("TargetName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("targetName");

                    b.HasKey("TargetId")
                        .HasName("PK__Target__30088013F51C99F1");

                    b.HasIndex(new[] { "TargetName" }, "UQ__Target__7A3CBD8B02E9E95B")
                        .IsUnique();

                    b.ToTable("Target", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.TargetPerson", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("personID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PersonId"));

                    b.Property<int?>("Age")
                        .HasColumnType("int")
                        .HasColumnName("age");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("description");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("gender");

                    b.Property<string>("Location")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(266)
                        .IsUnicode(false)
                        .HasColumnType("varchar(266)");

                    b.Property<string>("Relation")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("relation");

                    b.Property<int>("TargetId")
                        .HasColumnType("int")
                        .HasColumnName("targetID");

                    b.HasKey("PersonId")
                        .HasName("PK__Target_P__EC7D7D6D78B743FB");

                    b.HasIndex("TargetId");

                    b.ToTable("Target_Person", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.MxFaceIdentity", b =>
                {
                    b.HasOne("IFoundBackend.SqlModels.PostPerson", "Post")
                        .WithMany("MxFaceIdentities")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__MxFaceIde__post___06CD04F7");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.PostPerson", b =>
                {
                    b.HasOne("IFoundBackend.SqlModels.Image", "Image")
                        .WithMany("PostPeople")
                        .HasForeignKey("ImageId")
                        .HasConstraintName("FK__Post_Pers__image__48CFD27E");

                    b.HasOne("IFoundBackend.SqlModels.TargetPerson", "Person")
                        .WithMany("PostPeople")
                        .HasForeignKey("PersonId")
                        .IsRequired()
                        .HasConstraintName("FK__Post_Pers__perso__04E4BC85");

                    b.HasOne("IFoundBackend.SqlModels.PostStatus", "Status")
                        .WithMany("PostPeople")
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK__Post_Pers__statu__45F365D3");

                    b.Navigation("Image");

                    b.Navigation("Person");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.TargetPerson", b =>
                {
                    b.HasOne("IFoundBackend.SqlModels.Target", "Target")
                        .WithMany("TargetPeople")
                        .HasForeignKey("TargetId")
                        .IsRequired()
                        .HasConstraintName("FK__Target_Pe__targe__4D5F7D71");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("IFoundBackend.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("IFoundBackend.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IFoundBackend.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("IFoundBackend.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.Image", b =>
                {
                    b.Navigation("PostPeople");
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.PostPerson", b =>
                {
                    b.Navigation("MxFaceIdentities");
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.PostStatus", b =>
                {
                    b.Navigation("PostPeople");
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.Target", b =>
                {
                    b.Navigation("TargetPeople");
                });

            modelBuilder.Entity("IFoundBackend.SqlModels.TargetPerson", b =>
                {
                    b.Navigation("PostPeople");
                });
#pragma warning restore 612, 618
        }
    }
}
