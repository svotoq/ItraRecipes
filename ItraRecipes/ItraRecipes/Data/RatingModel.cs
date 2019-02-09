using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItraRecipes.Data
{
    public class RatingModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool Rate { get; set; }
        public int RecipesId { get; set; }
    }
}
