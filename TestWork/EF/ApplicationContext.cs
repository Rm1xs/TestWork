using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestWork.Models;

namespace TestWork.EF
{
    public class ApplicationContext : DbContext
    {
        //connect to db postgres
        private static string Host = "ec2-176-34-123-50.eu-west-1.compute.amazonaws.com";
        private static string User = "hzdbcicmufsizi";
        private static string DBname = "d77hhtgaq736ls";
        private static string Password = "8ce7ab17f3cda608772895c45932b6bc2dcae08d79cfcc61f51f90a365b9e25d";
        private static string Port = "5432";
        public DbSet<History> Histories { get; set; }
        public DbSet<Map> Maps { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Map>().Property(p => p.Id).ValueGeneratedOnAdd();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer;Trust Server Certificate=true", Host, User, DBname, Port, Password);
            optionsBuilder.UseNpgsql(connString);
        }
    }
}