using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItraRecipes.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string ChangePassword => "ChangePassword";

        public static string ExternalLogins => "ExternalLogins";

        public static string MyRecipes => "MyRecipes";

        public static string AddRecipes => "AddRecipe";

        public static string AdminPanel => "AdminPanel";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string AddRecipesNavClass(ViewContext viewContext) => PageNavClass(viewContext, AddRecipes);

        public static string MyRecipesNavClass(ViewContext viewContext) => PageNavClass(viewContext, MyRecipes);

        public static string AdminPanelNavClass(ViewContext viewContext) => PageNavClass(viewContext, AdminPanel);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}