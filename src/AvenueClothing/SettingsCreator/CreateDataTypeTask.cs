using System.Collections.Generic;
using System.Linq;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreateDataTypeTask : IPipelineTask<InitializeArgs>
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
            var enumDefinitionEditor = definitionEditors.FirstOrDefault(x => x == "Enum");
            
            var colourDataType = CreateSettingsHelper.CreateDataTypeIfNotExist("Colour", "", enumDefinitionEditor);

            //if created Data Type is an "Enum" we need to add values to it:
            var blueColourType = CreateSettingsHelper.CreateDataTypeEnumIfNotExist(colourDataType, "Blue");
            var redColourType = CreateSettingsHelper.CreateDataTypeEnumIfNotExist(colourDataType, "Red");
            var greenColourType = CreateSettingsHelper.CreateDataTypeEnumIfNotExist(colourDataType, "Green");
            var whiteColourType = CreateSettingsHelper.CreateDataTypeEnumIfNotExist(colourDataType, "White");

            var blueUK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(blueColourType, "en-UK", "Blue");
            var blueDK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(blueColourType, "da-DK", "Blå");
            
            var redUK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(redColourType, "en-UK", "Red");
            var redDK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(redColourType, "da-DK", "Rød");
            
            var greenUK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(greenColourType, "en-UK", "Green");
            var greenDK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(greenColourType, "da-DK", "Grøn");
            
            var whiteUK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(whiteColourType, "en-UK", "White");
            var whiteDK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(whiteColourType, "da-DK", "Hvid");
            
            return PipelineExecutionResult.Success;
        }


    }
}
