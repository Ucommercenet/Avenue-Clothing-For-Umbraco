using System.Collections.Generic;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreateDefinitionTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var shortTextDataType = CreateSettingsHelper.CreateDataTypeIfNotExist("ShortText","","");
            
            //payment method definition
            var paymentMethodDefinitionType = CreateSettingsHelper.CreateDefinitionTypeIfNotExist("PaymentMethod Definitions");
            
            var myPaymentMethodDefinition = CreateSettingsHelper.CreateDefinitionIfNotExist("My Payment Method Definition", paymentMethodDefinitionType, "Configuration for my Custom Payment Method");

            var myPaymentMethodDefinitionField = CreateSettingsHelper.CreateDefinitionFieldIfNotExist("CallbackUrl", myPaymentMethodDefinition, shortTextDataType);

            var list = new List<DefinitionField>
            {
                myPaymentMethodDefinitionField
            };


            //category definition
            var categoryDefinitionType = CreateSettingsHelper.CreateDefinitionTypeIfNotExist("Category Definitions");
            
            var myCategoryDefinition = CreateSettingsHelper.CreateDefinitionIfNotExist("My Category Definition", categoryDefinitionType, "Configuration for my Category");

            var myCategoryDefinitionField = CreateSettingsHelper.CreateDefinitionFieldIfNotExist("ShortTextOnCategory", myCategoryDefinition, shortTextDataType);
            
            
            var myParentCategoryDefinition = CreateSettingsHelper.CreateDefinitionIfNotExist("My Parent Category Definition", categoryDefinitionType, "Configuration for my ParentCategory");

            var myDefinitionRelation = CreateSettingsHelper.CreateDefinitionRelationIfNotExist(myCategoryDefinition, myParentCategoryDefinition);

            return PipelineExecutionResult.Success;
        }
    }
}
