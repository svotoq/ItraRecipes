using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItraRecipes.Data
{
    public class RecipesModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string PreviewImg { get; set; }
        public string PreviewText { get; set; }
        public string CategoryName { get; set; }
        public string Ingredients { get; set; }
        public string Difficulty { get; set; }
        public string CoockingTime { get; set; }
        public uint Portions { get; set; }
        public float Rating { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public string Date { get; set; }
        public int Comments { get; set; }
    }
}
