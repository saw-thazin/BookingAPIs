
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeTest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CodeTest.Models
{
    public class BookingSystem : DbContext
    {
        internal object ClassBooking;

        public BookingSystem(DbContextOptions<BookingSystem> options) : base(options)
        {

        }


        //public DbSet<Item> TODOItem { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Package> Package { get; set; }

        public DbSet <Class> Class { get; set; }
        public DbSet <ClassBookingDTO> BookingClass { get; set; }

        public DbSet <Waitlist> WaitList { get; set; }
        //public DbSet<AdminInfo> AdminInformation { get; set; }
        //public DbSet<Department> DepartmentInformation { get; set; }

    }
}
