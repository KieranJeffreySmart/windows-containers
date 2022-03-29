using System.Web;
using System.Web.Mvc;

namespace hello_world_aspnet_framework_4_6_1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
