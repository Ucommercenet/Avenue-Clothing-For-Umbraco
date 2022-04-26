using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreateOrderNumberSerieTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var exampleOrderNumberSerie = CreateSettingsHelper.CreateOrderNumberSerieIfNotExist("Example","TEST-");
            
            return PipelineExecutionResult.Success;
        }
    }
}