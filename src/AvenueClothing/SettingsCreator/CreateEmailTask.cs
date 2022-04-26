using System.Linq;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;
using Umbraco.Core;
using Umbraco.Web.Composing;
using Umbraco.Core.Services;

namespace AvenueClothing.SettingsCreator
{
    public class CreateEmailTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        
        // private ContentTypeService ContentTypeService = ObjectFactory.Instance.Resolve<ContentTypeService>();
        //
        // private ContentService ContentService = ObjectFactory.Instance.Resolve<ContentService>();
        
        public IContentTypeService ContentTypeService => Umbraco.Core.Composing.Current.Services.ContentTypeService;
        public IContentService ContentService => Umbraco.Core.Composing.Current.Services.ContentService;
        
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var docType = ContentTypeService.Get("Email");
            if (docType == null)
            {
                
                Serilog.Log.ForContext(this.GetType()).Information("{0}", "failed CreateEmailTask doctype");
                return PipelineExecutionResult.Warning;
            }

            var emails = ContentService.GetPagedOfType(docType.Id, 0, int.MaxValue, out var b, null);
            var emailContent = emails.FirstOrDefault(e => e.Name == "Order Confirmation Email");
            if (emailContent == null)
            {
                Serilog.Log.ForContext(this.GetType()).Information("{0}", "failed CreateEmailTask emailcontent");
                return PipelineExecutionResult.Warning;
            }
            
            var emailCmsContentId = emailContent.GetUdi().Guid.ToString();
            
            var confirmationEmailType = CreateSettingsHelper.CreateEmailTypeIfNotExist(name: "OrderConfirmation", description:"");
            
            var defaultEmailProfile = CreateSettingsHelper.CreateEmailProfileIfNotExist(name:"Default");

            var uCommerceDemoShopEmailProfileInformation = CreateSettingsHelper.CreateEmailProfileInformationIfNotExist(emailType:confirmationEmailType, emailProfile:defaultEmailProfile, fromName:"uCommerce Demo Shop", fromAddress: "demo@uCommerce.dk");
            
            var orderConfirmationEmailContentGB = CreateSettingsHelper.CreateEmailContentIfNotExist(emailType: confirmationEmailType, emailProfile: defaultEmailProfile, subject:"Order Confirmation for your GB purchase", contentId:emailCmsContentId, cultureCode:"en-GB");
            var orderConfirmationEmailContentUS = CreateSettingsHelper.CreateEmailContentIfNotExist(emailType: confirmationEmailType, emailProfile: defaultEmailProfile, subject:"", contentId:emailCmsContentId, cultureCode:"en-US");
            orderConfirmationEmailContentUS.ContentId = emailCmsContentId;
            orderConfirmationEmailContentUS.Subject = "Order Confirmation for your US purchase";
            orderConfirmationEmailContentUS.Save();
            
            Serilog.Log.ForContext(this.GetType()).Information("{0}", "completed CreateEmailTask");

            return PipelineExecutionResult.Success;
        }
    }
}
