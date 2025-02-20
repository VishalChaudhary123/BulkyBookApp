﻿using Bulky.Models.Models;
using Bulky.Utility;
using BulkyWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,ApplicationDbContext db)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._db = db;
        }
        public void Initialize()
        {
            // push migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0) 
                { 
                  _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //create roles if they are created
            // Checking if role does not exist then create
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();



                //if roles are not created, then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Vishal20059@gmail.com",
                    Email = "Vishal20059@gmail.com",
                    Name = "Vishal Chaudhary",
                    PhoneNumber = "8802137681",
                    StreetAddress = " test 123 Ave",
                    State = "DL",
                    PostalCode = "223344",
                    City = "Delhi",

                },"Admin@123").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Vishal20059@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return; // Return back to the application

            
        }
    }
}
