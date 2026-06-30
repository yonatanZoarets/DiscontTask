using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscontTask.Crm.Plugins.Contact1
{
    public class PreValidationCreateContact1Plugin : IPlugin
    {
        public PreValidationCreateContact1Plugin()
        {
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {

                Entity entity = (Entity)context.InputParameters["Target"];

                //in this case I made only regex validation
                //no new_emailaddress attribute - key will not be found.
                //if we want it will never be empty - we will make this field required.
                if (entity.Attributes.Contains("new_emailaddress"))
                {
                    var emailAddress = entity.Attributes["new_emailaddress"].ToString();

                    Regex EmailRegex = new Regex(@"^[a-z0-9]+@[a-z]+\.[a-z]{2,3}$", RegexOptions.IgnoreCase);

                    if (!EmailRegex.IsMatch(emailAddress))
                    {
                        throw new InvalidPluginExecutionException("כתובת דואר אלקטרוני לא תואמת לתבנית");

                    }
                }
            }
        }
    }
}
