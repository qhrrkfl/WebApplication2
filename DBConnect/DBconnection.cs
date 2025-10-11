using DBconnect.Models;
using DBConnect.TranslateDBTABLE;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DBconnection
{
    public class TranslateDbContext : DbContext
    {
        public TranslateDbContext(DbContextOptions<TranslateDbContext> options)
            : base(options) { }


        // 👇 이 줄이 핵심: EF가 User 클래스를 DB 테이블로 인식하게 함
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<EmailValidations> EmailValidations { get; set; }    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users")          // 실제 테이블명
                .HasKey(u => u.Email);     // 기본키 지정

            base.OnModelCreating(modelBuilder);
        }

      

    }
}
