using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ExampleDynamicsPlugin
{
    public class CreateTaskOnAccountCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtaon the target entity from the input parameters
                Entity entity = (Entity)context.InputParameters["Target"];

                // Verify that the target entity represents an entity type you are expecting.
                // For example, an account. If not, the plug-in was not registered correctly.
                if (entity.LogicalName != "account")
                    return;

                try
                {
                    // Plug-in based logic goes here

                    // An accountnumber should not already exist because
                    // it is system generated
                    if (entity.Attributes.Contains("accountnumber") == false)
                    {
                        // create a new accountnumber attribute, set its value, and add
                        // the attribute to the entity's attribute collection
                        Random rndgen = new Random();
                        entity.Attributes.Add("accountnumber", string.Concat("TLD-", rndgen.Next().ToString()));
                    }
                    else
                    {
                        // Throw an error, because the accountnumber must be system generated
                        // Throwing an InvalidPluginExecutionEception will cause the error message
                        // to be displayed in a dialog of the web application
                        throw new InvalidPluginExecutionException("The account number can only be set by the system");
                    }
                }

                catch (Exception ex)
                {
                    tracingService.Trace("CreateTaskOnAccountCreate plug-in: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
