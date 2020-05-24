﻿using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Web;
using System.Web.Http;


namespace MutticoFleet.Server
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.Add
(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
        //    {
        //        Response.StatusCode = 200;
        //        Response.End();
        //    }
        //}
        public void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            var mvcApplication = sender as WebApiApplication;
            HttpRequest request = null;
            if (mvcApplication != null) request = mvcApplication.Request;
        }

    }
}
