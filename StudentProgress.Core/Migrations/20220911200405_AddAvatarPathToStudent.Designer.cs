﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudentProgress.Core.Entities;

#nullable disable

namespace StudentProgress.Core.Migrations
{
    [DbContext(typeof(ProgressContext))]
    [Migration("20220911200405_AddAvatarPathToStudent")]
    partial class AddAvatarPathToStudent
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("ProgressTagProgressUpdate", b =>
                {
                    b.Property<int>("TagsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UpdatesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TagsId", "UpdatesId");

                    b.HasIndex("UpdatesId");

                    b.ToTable("ProgressTagProgressUpdate");
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.Milestone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Artefact")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LearningOutcome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("StudentGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("StudentGroupId", "Artefact", "LearningOutcome")
                        .IsUnique();

                    b.ToTable("Milestone", (string)null);
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.MilestoneProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("MilestoneId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProgressUpdateId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MilestoneId");

                    b.HasIndex("ProgressUpdateId");

                    b.ToTable("MilestoneProgress", (string)null);
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.ProgressTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ProgressTag", (string)null);
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.ProgressUpdate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Feedback")
                        .HasColumnType("TEXT");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProgressFeeling")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StudentId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("StudentId");

                    b.ToTable("ProgressUpdate", (string)null);
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Student", (string)null);
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.StudentGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mnemonic")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Period")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue(new DateTime(1, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name", "Period")
                        .IsUnique();

                    b.ToTable("StudentGroup", (string)null);
                });

            modelBuilder.Entity("StudentStudentGroup", b =>
                {
                    b.Property<int>("StudentGroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StudentsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("StudentGroupsId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("StudentStudentGroup");
                });

            modelBuilder.Entity("ProgressTagProgressUpdate", b =>
                {
                    b.HasOne("StudentProgress.Core.Entities.ProgressTag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudentProgress.Core.Entities.ProgressUpdate", null)
                        .WithMany()
                        .HasForeignKey("UpdatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.Milestone", b =>
                {
                    b.HasOne("StudentProgress.Core.Entities.StudentGroup", "StudentGroup")
                        .WithMany("Milestones")
                        .HasForeignKey("StudentGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StudentGroup");
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.MilestoneProgress", b =>
                {
                    b.HasOne("StudentProgress.Core.Entities.Milestone", "Milestone")
                        .WithMany()
                        .HasForeignKey("MilestoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudentProgress.Core.Entities.ProgressUpdate", "ProgressUpdate")
                        .WithMany("MilestonesProgress")
                        .HasForeignKey("ProgressUpdateId");

                    b.Navigation("Milestone");

                    b.Navigation("ProgressUpdate");
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.ProgressUpdate", b =>
                {
                    b.HasOne("StudentProgress.Core.Entities.StudentGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudentProgress.Core.Entities.Student", "Student")
                        .WithMany("ProgressUpdates")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("StudentStudentGroup", b =>
                {
                    b.HasOne("StudentProgress.Core.Entities.StudentGroup", null)
                        .WithMany()
                        .HasForeignKey("StudentGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudentProgress.Core.Entities.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.ProgressUpdate", b =>
                {
                    b.Navigation("MilestonesProgress");
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.Student", b =>
                {
                    b.Navigation("ProgressUpdates");
                });

            modelBuilder.Entity("StudentProgress.Core.Entities.StudentGroup", b =>
                {
                    b.Navigation("Milestones");
                });
#pragma warning restore 612, 618
        }
    }
}
