using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace AvenueClothing.Search
{
    public class CategoryWithAllSubCatProducts : IAdorn<Category>
    {
        [Mandatory] public IFetcher<Category> CategoryFetcher { get; set; }

        public void Adorn(IEnumerable<Category> items)
        {
            foreach (var category in items)
            {
                category["ProductsInAllSubcategories"] = category.Products.Concat(SubCatProducts(category)).Distinct().ToList();
            }
        }

        private IEnumerable<Guid> SubCatProducts(Category category)
        {
            List<Category> subCategories = CategoryFetcher.InBatches(category.Categories).SelectMany(kvp => kvp.Values)
                .SelectMany(d => d).ToList();
            
            return subCategories.SelectMany(c => c.Products).Concat(subCategories.SelectMany(SubCatProducts));
        }

        public void Adorn(IEnumerable<Category> localizedItems, CultureInfo culture)
        {
        }
    }
}