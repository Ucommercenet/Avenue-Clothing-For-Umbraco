using Castle.Windsor;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Component = Castle.MicroKernel.Registration.Component;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(AvenueClothing.Search.CustomSearch), "Setup")]
namespace AvenueClothing.Search
{
    public class CustomSearch
    {
        // ReSharper disable once UnusedMember.Global
        public static void Setup()
        {
            var childContainer = new WindsorContainer();

            childContainer.Register(
                Component.For<IIndexDefinition<Product>>().ImplementedBy<AvenueProductIndexDefinition>().IsDefault(),
                Component.For<IAdorn<Category>>().ImplementedBy<CategoryWithAllSubCatProducts>()
            );

            ObjectFactory.Instance.AddChildContainer(childContainer);
        }
    }
}