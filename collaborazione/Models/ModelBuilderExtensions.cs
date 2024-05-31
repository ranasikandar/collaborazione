using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            //SEED ROLES
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "972cda86-389e-443c-a9e1-06fb0ef7f62e",
                Name = "admin",
                NormalizedName = "admin"
            }
            ,
            new IdentityRole
            {
                Id = "772cda87-389e-443c-a7e1-05fb0ef7f67c",
                Name = "user",
                NormalizedName = "user"
            }
            );
            //SEED ROLES END
            //SEED LOOKINGFORITEMS
            modelBuilder.Entity<Progress>().HasData(new Progress
            {
                ProgressId = 1,
                ProgressName = "Da contattare"//To contact
            },
            new Progress
            {
                ProgressId = 2,
                ProgressName = "In elaborazione"//in processing
            },
            new Progress
            {
                ProgressId = 3,
                ProgressName = "Confermata"//confirmed
            },
            new Progress
            {
                ProgressId = 4,
                ProgressName = "Commissione pagata"//Commitssion Paid
            },
            new Progress
            {
                ProgressId = 5,
                ProgressName = "Calcolo commissione"//Commitssion calculate
            },
            new Progress
            {
                ProgressId = 6,
                ProgressName = "Da ricevere profitto"//to recive profit
            }
            );
            //SEED LOOKINGFORITEMS
        }
    }
}
