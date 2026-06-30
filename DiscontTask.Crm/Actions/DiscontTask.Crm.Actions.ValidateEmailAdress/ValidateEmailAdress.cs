using Microsoft.Xrm.Sdk;
using System;
using System.Text.RegularExpressions;

namespace DiscontTask.Crm.Actions.ValidateEmailAdress
{
    public class ValidateEmailAdress : IPlugin
    {
        public ValidateEmailAdress()
        {
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("emailAddress"))
            {
                var emailAddress = context.InputParameters["emailAddress"].ToString();

                Regex EmailRegex = new Regex(@"^[a-z0-9]+@[a-z]+\.[a-z]{2,3}$", RegexOptions.IgnoreCase);
                var isValid = true;
                var message = "כתובת דואר אלקטרוני תקינה";

                if (string.IsNullOrWhiteSpace(emailAddress))
                {
                    isValid = false;
                    message = "כתובת דואר אלקטרוני חייבת להכיל ערך";
                }
                else if (!EmailRegex.IsMatch(emailAddress))
                {
                    isValid = false;
                    message = "כתובת דואר אלקטרוני לא תואמת לתבנית";
                }

                context.OutputParameters["isValid"] = isValid;
                context.OutputParameters["message"] = message;
            }
        }
    }
}
