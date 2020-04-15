using System;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;

namespace AvenueClothing.Installer.Helpers
{
    public class GenericHelpers
    {
        public static void DoForEachCulture(Action<string> toDo)
        {
            foreach (Language language in ObjectFactory.Instance.Resolve<ILanguageService>().GetAllLanguages())
            {
                toDo(language.CultureCode);
            }
        }
    }
}