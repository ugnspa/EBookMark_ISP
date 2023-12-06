using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EBookMark_ISP.Models;

public partial class EbookmarkContext : DbContext
{
    public EbookmarkContext(DbContextOptions<EbookmarkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Guardian> Guardians { get; set; }

    public virtual DbSet<Homework> Homeworks { get; set; }

    public virtual DbSet<Mark> Marks { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<SchoolTeacher> SchoolTeachers { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectTime> SubjectTimes { get; set; }

    public virtual DbSet<SubjectType> SubjectTypes { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.FkUser).HasName("PRIMARY");

            entity.ToTable("admins");

            entity.Property(e => e.FkUser).HasColumnName("fk_User");

            entity.HasOne(d => d.FkUserNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.FkUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("admins_ibfk_1");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PRIMARY");

            entity.ToTable("classes");

            entity.HasIndex(e => e.FkSchool, "school_idx");

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.FkSchool).HasColumnName("fk_School");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StudentsCount).HasColumnName("students_count");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.FkSchoolNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.FkSchool)
                .HasConstraintName("school");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("classrooms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Building)
                .HasMaxLength(255)
                .HasColumnName("building");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.Floor).HasColumnName("floor");
            entity.Property(e => e.UseCase)
                .HasMaxLength(255)
                .HasColumnName("use_case");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("files");

            entity.HasIndex(e => e.FkHomework, "makes_up");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkHomework).HasColumnName("fk_Homework");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Size)
                .HasPrecision(20)
                .HasColumnName("size");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");

            entity.HasOne(d => d.FkHomeworkNavigation).WithMany(p => p.Files)
                .HasForeignKey(d => d.FkHomework)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("makes_up");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("genders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(7)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Guardian>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("guardians");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phone_number");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Homework>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("homeworks");

            entity.HasIndex(e => e.FkSubject, "užduodamas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationDate)
                .HasColumnType("date")
                .HasColumnName("creation_date");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DueToDate)
                .HasColumnType("date")
                .HasColumnName("due_to_date");
            entity.Property(e => e.FilesCount).HasColumnName("files_count");
            entity.Property(e => e.FkSubject).HasColumnName("fk_Subject");
            entity.Property(e => e.UploadDate)
                .HasColumnType("date")
                .HasColumnName("upload_date");

            entity.HasOne(d => d.FkSubjectNavigation).WithMany(p => p.Homeworks)
                .HasForeignKey(d => d.FkSubject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("užduodamas");
        });

        modelBuilder.Entity<Mark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("marks");

            entity.HasIndex(e => e.FkStudent, "fk_Student");

            entity.HasIndex(e => e.FkSubjectTime, "fk_Subject_time");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.FkStudent).HasColumnName("fk_Student");
            entity.Property(e => e.FkSubjectTime).HasColumnName("fk_Subject_time");
            entity.Property(e => e.Mark1)
                .HasMaxLength(255)
                .HasColumnName("mark");
            entity.Property(e => e.RegistrationDate)
                .HasColumnType("datetime")
                .HasColumnName("registration_date");

            entity.HasOne(d => d.FkStudentNavigation).WithMany(p => p.Marks)
                .HasForeignKey(d => d.FkStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gauna");

            entity.HasOne(d => d.FkSubjectTimeNavigation).WithMany(p => p.Marks)
                .HasForeignKey(d => d.FkSubjectTime)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("įvertinama");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("schedules");

            entity.HasIndex(e => e.FkClass, "turi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkClass).HasColumnName("fk_Class");
            entity.Property(e => e.SemesterEnd)
                .HasColumnType("date")
                .HasColumnName("semester_end");
            entity.Property(e => e.SemesterStart)
                .HasColumnType("date")
                .HasColumnName("semester_start");

            entity.HasOne(d => d.FkClassNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FkClass)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("turi");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("schools");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SchoolTeacher>(entity =>
        {
            entity.HasKey(e => new { e.FkSchool, e.FkTeacher }).HasName("PRIMARY");

            entity.ToTable("school_teachers");

            entity.Property(e => e.FkSchool).HasColumnName("fk_School");
            entity.Property(e => e.FkTeacher).HasColumnName("fk_Teacher");

            entity.HasOne(d => d.FkSchoolNavigation).WithMany(p => p.SchoolTeachers)
                .HasForeignKey(d => d.FkSchool)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dirba");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.FkUser).HasName("PRIMARY");

            entity.ToTable("students");

            entity.HasIndex(e => e.Gender, "gender");

            entity.HasIndex(e => e.FkGuardian, "globoja");

            entity.HasIndex(e => e.FkSchool, "mokosi");

            entity.HasIndex(e => e.FkClass, "priklauso");

            entity.Property(e => e.FkUser).HasColumnName("fk_User");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("birth_date");
            entity.Property(e => e.Document).HasColumnName("document");
            entity.Property(e => e.FkClass).HasColumnName("fk_Class");
            entity.Property(e => e.FkGuardian).HasColumnName("fk_Guardian");
            entity.Property(e => e.FkSchool).HasColumnName("fk_School");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PersonalCode)
                .HasMaxLength(255)
                .HasColumnName("personal_code");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.FkClassNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.FkClass)
                .HasConstraintName("priklauso");

            entity.HasOne(d => d.FkGuardianNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.FkGuardian)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("globoja");

            entity.HasOne(d => d.FkSchoolNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.FkSchool)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mokosi");

            entity.HasOne(d => d.FkUserNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.FkUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("students_ibfk_2");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Gender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("students_ibfk_1");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PRIMARY");

            entity.ToTable("subjects");

            entity.HasIndex(e => e.FkTeacher, "dėsto");

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FkTeacher).HasColumnName("fk_Teacher");
            entity.Property(e => e.Language)
                .HasMaxLength(255)
                .HasColumnName("language");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.FkTeacherNavigation).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.FkTeacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dėsto");
        });

        modelBuilder.Entity<SubjectTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subject_times");

            entity.HasIndex(e => e.FkClassroom, "nurodytas");

            entity.HasIndex(e => e.FkSchedule, "sudaro");

            entity.HasIndex(e => e.Type, "type");

            entity.HasIndex(e => e.FkSubject, "vyksta");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descrtiption)
                .HasMaxLength(255)
                .HasColumnName("descrtiption");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.FkClassroom).HasColumnName("fk_Classroom");
            entity.Property(e => e.FkSchedule).HasColumnName("fk_Schedule");
            entity.Property(e => e.FkSubject).HasColumnName("fk_Subject");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.FkClassroomNavigation).WithMany(p => p.SubjectTimes)
                .HasForeignKey(d => d.FkClassroom)
                .HasConstraintName("nurodytas");

            entity.HasOne(d => d.FkScheduleNavigation).WithMany(p => p.SubjectTimes)
                .HasForeignKey(d => d.FkSchedule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sudaro");

            entity.HasOne(d => d.FkSubjectNavigation).WithMany(p => p.SubjectTimes)
                .HasForeignKey(d => d.FkSubject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vyksta");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.SubjectTimes)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subject_times_ibfk_1");
        });

        modelBuilder.Entity<SubjectType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subject_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.FkUser).HasName("PRIMARY");

            entity.ToTable("teachers");

            entity.HasIndex(e => e.Gender, "gender");

            entity.Property(e => e.FkUser).HasColumnName("fk_User");
            entity.Property(e => e.EmploymentDate)
                .HasColumnType("date")
                .HasColumnName("employment_date");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PersonalCode)
                .HasMaxLength(255)
                .HasColumnName("personal_code");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phone_number");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.FkUserNavigation).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.FkUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teachers_ibfk_2");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.Gender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teachers_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
