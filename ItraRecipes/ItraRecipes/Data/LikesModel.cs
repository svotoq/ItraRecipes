using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItraRecipes.Data
{
    public class LikesModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool Like { get; set; }
        public int CommentId { get; set; }
    }
}
