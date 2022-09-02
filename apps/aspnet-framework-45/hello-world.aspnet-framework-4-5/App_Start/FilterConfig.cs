using System.Web;
using System.Web.Mvc;

namespace hello_world.aspnet_framework_4_5
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
