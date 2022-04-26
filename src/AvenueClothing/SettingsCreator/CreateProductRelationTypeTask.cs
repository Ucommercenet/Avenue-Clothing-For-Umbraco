using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreateProductRelationTypeTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            //var myProductRelationType = CreateSettingsHelper.CreateProductRelationTypeIfNotExist(name: "MyProductRelationType", description:"Goes well with");

            return PipelineExecutionResult.Success;
        }
    }
}
