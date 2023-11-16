﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudentProgress.Web.Lib.Data;

#nullable disable

namespace StudentProgress.Web.Migrations
{
    [DbContext(typeof(WebContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("AdventurePerson", b =>
                {
                    b.Property<int>("AdventuresId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PeopleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AdventuresId", "PeopleId");

                    b.HasIndex("PeopleId");

                    b.ToTable("AdventurePerson");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Adventure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mnemonic")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name", "DateStart")
                        .IsUnique();

                    b.ToTable("Adventures");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Objective", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OptimalTargetDeadLine")
                        .HasColumnType("TEXT");

                    b.Property<int?>("QuestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("QuestId");

                    b.ToTable("Objective");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.ObjectiveProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AchievedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ObjectiveId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PersonId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ObjectiveId");

                    b.HasIndex("PersonId");

                    b.ToTable("ObjectiveProgress");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Initials")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Quest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ObjectiveMain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("QuestLineId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("QuestLineId");

                    b.ToTable("Quest");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.QuestLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdventureId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MainObjective")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AdventureId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Order")
                        .IsUnique();

                    b.ToTable("QuestLines");
                });

            modelBuilder.Entity("AdventurePerson", b =>
                {
                    b.HasOne("StudentProgress.Web.Models.Adventure", null)
                        .WithMany()
                        .HasForeignKey("AdventuresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudentProgress.Web.Models.Person", null)
                        .WithMany()
                        .HasForeignKey("PeopleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Objective", b =>
                {
                    b.HasOne("StudentProgress.Web.Models.Quest", null)
                        .WithMany("Objectives")
                        .HasForeignKey("QuestId");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.ObjectiveProgress", b =>
                {
                    b.HasOne("StudentProgress.Web.Models.Objective", null)
                        .WithMany("Progresses")
                        .HasForeignKey("ObjectiveId");

                    b.HasOne("StudentProgress.Web.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Quest", b =>
                {
                    b.HasOne("StudentProgress.Web.Models.QuestLine", null)
                        .WithMany("Quests")
                        .HasForeignKey("QuestLineId");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.QuestLine", b =>
                {
                    b.HasOne("StudentProgress.Web.Models.Adventure", "Adventure")
                        .WithMany("QuestLines")
                        .HasForeignKey("AdventureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventure");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Adventure", b =>
                {
                    b.Navigation("QuestLines");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Objective", b =>
                {
                    b.Navigation("Progresses");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.Quest", b =>
                {
                    b.Navigation("Objectives");
                });

            modelBuilder.Entity("StudentProgress.Web.Models.QuestLine", b =>
                {
                    b.Navigation("Quests");
                });
#pragma warning restore 612, 618
        }
    }
}
