using System.Web.Http;
using System.Web.Http.Filters;
using CommonFramework.Mvc.Filter;
using CT.Common.Mvc.Filter;

namespace ApprovalProject.Web.Code
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new StationAuthorizeFilter());
            config.Filters.Add(new CommonFramework.Mvc.Filter.ExceptionFilter("系统繁忙，请稍后再试"));
            config.Filters.Add(new RequestLoggerFilter());
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}"
            );
        }
    }
}