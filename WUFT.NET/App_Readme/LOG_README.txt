=============
DB Setup
=============
1. Create/choose database to store the errors
2. Run the App_Readme/Create Logging Tables.sql script locally
3. Update connection string in nlog.config file (should be in root of project)

======
Step 1
======
Add to global.asax or equivelant

ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("utc_date", typeof(SharedCodeLibrary.Core.Services.Logging.NLog.Renderers.UtcDateRenderer));
ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("application_name", typeof(SharedCodeLibrary.Core.Services.Logging.NLog.Renderers.ApplicationNameRenderer));

=======
Step 2:
=======
Add following application setting to app/web.config:

<appSettings>
	<add key="loggingApplicationName" value="Name Of Application" />
</appSettings>

=======
Step 3:
=======
Add following connection string to app/web.config called "Logging.DB" with the providerName set as below

<add name="Logging.DB" connectionString="Data Source=(LocalDb)\v11.0;Initial Catalog=<YOUR ERROR DATABASE NAME>;Integrated Security=True" providerName="System.Data.SqlClient" />