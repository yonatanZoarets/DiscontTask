using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscontTask.Crm.Plugins.Contact1
{
    public class PostCreateContact1AsyncPlugin : IPlugin
    {
        public PostCreateContact1AsyncPlugin()
        {
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {


                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                if(!entity.Attributes.Contains("new_birthdate")|| entity.Attributes["new_birthdate"]==null)
                    return;

                Entity toParty = new Entity("activityparty");
                toParty["partyid"] = new EntityReference("new_contact1", entity.Id); // Target Contact ID
                toParty["addressused"] = entity.Attributes["new_emailaddress"];

                EntityReference createdByRef = (EntityReference)entity.Attributes["createdby"];
                Entity createdBy = service.Retrieve(createdByRef.LogicalName, createdByRef.Id, new ColumnSet("fullname"));


                // 2. Create the Email Activity Record instance
                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["subject"] = "אימייל מהמערכת";
                email["description"] = $"שלום {entity.Attributes["new_firstname"].ToString()} {entity.Attributes["new_lastname"].ToString()}," +
                    $"\r\nברוכים הבאים לקהל לקוחותינו" +
                    $".\r\nבכבוד רב,\r\n{createdBy.Attributes["fullname"]?.ToString() ?? null}.";
                email["directioncode"] = true; // Outbound email

                // 3. Track record instantiation in Dynamics CRM
                Guid emailId = service.Create(email);

                // 4. Fire the native Send Request pipeline
                SendEmailRequest sendEmailRequest = new SendEmailRequest
                {
                    EmailId = emailId,
                    TrackingToken = "",
                    IssueSend = true
                };

                SendEmailResponse response = (SendEmailResponse)service.Execute(sendEmailRequest);


            }
        }
    }
}
