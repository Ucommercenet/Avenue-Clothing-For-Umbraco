using System;

using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;

namespace UCommerce.RazorStore.Installer
{
    public class Helpers
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