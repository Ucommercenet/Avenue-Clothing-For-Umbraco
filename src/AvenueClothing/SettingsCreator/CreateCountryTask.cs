using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreateCountryTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var denmarkCountry = CreateSettingsHelper.CreateCountryIfNotExist(name: "Denmark", culture: "da-DK");
            var ukCountry = CreateSettingsHelper.CreateCountryIfNotExist(name: "United Kingdom", culture: "en-GB");

            return PipelineExecutionResult.Success;
        }
    }
}
