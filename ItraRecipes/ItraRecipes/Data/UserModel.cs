using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItraRecipes.Data
{
    public class UserModel : IdentityUser
    {
        public float UserRating { get; set; }
        public int Posts { get; set; }
    }
}
