using SpongeBob_Mall.Filter;
using System.Web;
using System.Web.Mvc;

namespace SpongeBob_Mall
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new IsAuthorizeAttribute());
        }
    }
}
