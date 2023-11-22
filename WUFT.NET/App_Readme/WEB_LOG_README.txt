======
Step 1
======
Add to global.asax

ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(SharedCodeLibrary.Core.Services.Logging.Web.NLog.Renderers.WebVariablesRenderer));


=======
Step 2:
=======
Add to App_Start/FilterConfig:

filters.Add(new ElmahErrorHandlerAttribute());