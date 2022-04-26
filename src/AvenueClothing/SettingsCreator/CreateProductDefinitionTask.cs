using System.Collections.Generic;
using System.Linq;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreateProductDefinitionTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            //possible DataTypeEditor options:
            var definitionEditors = new List<string>
            {
                "DatePicker",
                "DateTime",
                "Number",
                "RichText",
                "Boolean",
                "EnumMultiSelect",
                "Enum",
                "LongText",
                "ShortText",
                "ContentPickerMultiSelect",
                "ImagePickerMultiSelect",
                "DefinitionTypePicker",
                "DefinitionPicker",
                "ComponentTypePicker",
                "List",
                "EmailContent",
                "Media",
                "Content"
            };
            
            var shortTextDefinitionEditor = definitionEditors.FirstOrDefault(x => x == "ShortText");
            var booleanDefinitionEditor = definitionEditors.FirstOrDefault(x => x == "Boolean");
            var shortTextDataType = CreateSettingsHelper.CreateDataTypeIfNotExist(shortTextDefinitionEditor, "", "");
            var booleanDataType = CreateSettingsHelper.CreateDataTypeIfNotExist(booleanDefinitionEditor, "", "");
            var colourDataType = CreateSettingsHelper.CreateDataTypeIfNotExist("Colour", "", "");

            var shirtDefinition = CreateSettingsHelper.CreateProductDefinitionIfNotExist("Shirt");
            
            var collarSizeField = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(shirtDefinition, "CollarSize", shortTextDataType, false, true, true, true);
            var colourField = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(shirtDefinition, "Colour", colourDataType, false, true, true, true);
            var showOnHomepageField1 = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(shirtDefinition, "ShowOnHomepage", booleanDataType, false, false, false, true);
            
            
            var shoeDefinition = CreateSettingsHelper.CreateProductDefinitionIfNotExist("Shoe");
            
            var shoeSizeField = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(shoeDefinition, "ShoeSize", shortTextDataType, false, true, true, true);
            var showOnHomepageField2 = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(shoeDefinition, "ShowOnHomepage", booleanDataType, false, false, false, true);
            
            
            var accessoriesDefinition = CreateSettingsHelper.CreateProductDefinitionIfNotExist("Accessory");
            
            var showOnHomepageField3 = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(accessoriesDefinition, "ShowOnHomepage", booleanDataType, false, false, false, true);

            // var allProductsParentDefinition = CreateSettingsHelper.CreateProductDefinitionIfNotExist("All Products");
            // var showOnHomepageField = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(allProductsParentDefinition, "ShowOnHomepage", booleanDataType, false, false, false, true);

            // var shirtProductDefinitionRelation = CreateSettingsHelper.CreateProductDefinitionRelationIfNotExist(shirtDefinition, allProductsParentDefinition);
            // var shoeProductDefinitionRelation = CreateSettingsHelper.CreateProductDefinitionRelationIfNotExist(shoeDefinition, allProductsParentDefinition);
            // var accessoriesProductDefinitionRelation = CreateSettingsHelper.CreateProductDefinitionRelationIfNotExist(accessoriesDefinition, allProductsParentDefinition);
            
            var colourProductDefinitionFieldDescriptionEnglish = CreateSettingsHelper.CreateProductDefinitionFieldDescriptionIfNotExist(colourField, "en-US", "Colour", "");
            var colourProductDefinitionFieldDescriptionDanish = CreateSettingsHelper.CreateProductDefinitionFieldDescriptionIfNotExist(colourField, "da-DK", "Farve", "");
            
            return PipelineExecutionResult.Success;
        }
    }
}
