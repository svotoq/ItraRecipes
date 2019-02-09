using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItraRecipes.Data
{
    public class CommentsModel
    {
        public int Id { get; set; }
        public int RecipesId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public int Likes { get; set; }
    }
}
