using System;
using System.Collections.Generic;
using System.Linq;

using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Factories;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Security;

namespace UCommerce.RazorStore.Installer
{
    public class ConfigurationInstaller
    {
        public void Configure()
        {
            CreateDataTypes();
            CreateProductDefinitions();
        }

        public void AssignAccessPermissionsToDemoStore()
        {
            var userService = ObjectFactory.Instance.Resolve<IUserService>();
            var user = userService.GetCurrentUser();

            var roleService = ObjectFactory.Instance.Resolve<IRoleService>();
            var roles = roleService.GetAllRoles();

            roleService.AddUserToRoles(user, roles);
        }

        private void CreateDataTypes()
        {
            CreateColourDropDownList();
        }

        private void CreateColourDropDownList()
        {
            var dataTypeEnum = CreateDataType("Colour", "Enum");

            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Blue", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Green", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Red", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("White", dataTypeEnum));
        }

        private static DataType CreateDataType(string name, string dataType)
        {
            var dataTypeEnum = DataType.SingleOrDefault(x => x.TypeName == name) ?? new DataTypeFactory().NewWithDefaults(name);

            dataTypeEnum.TypeName = "Colour";
            dataTypeEnum.DefinitionName = dataType;
            dataTypeEnum.Nullable = false;
            dataTypeEnum.ValidationExpression = string.Empty;
            dataTypeEnum.BuiltIn = false;

            dataTypeEnum.Save();

            return dataTypeEnum;
        }

        private DataTypeEnum GenerateColourDataTypeEnum(string colour, DataType parentDataType)
        {
            var dataTypeEnum = DataTypeEnum.SingleOrDefault(x => x.Value == colour && x.DataType.DataTypeId == parentDataType.DataTypeId) ?? new DataTypeEnumFactory().NewWithDefaults(colour);
            dataTypeEnum.Deleted = false;
            dataTypeEnum.DataType = DataType.Get(parentDataType.DataTypeId);
            dataTypeEnum.Save();

            Helpers.DoForEachCulture(language =>
            {
                if (dataTypeEnum.GetDescription(language) == null)
                    dataTypeEnum.AddDescription(new DataTypeEnumDescription { CultureCode = language, DisplayName = colour, Description = colour });
            });

            return dataTypeEnum;
        }

        private void CreateProductDefinitions()
        {
            CreateShirtProductDefinition();
            CreateShoeProductDefinition();
            CreateAccessoryProductDefinition();
        }

        private static ProductDefinition CreateProductDefinition(string name)
        {
            var productDefinition = ProductDefinition.SingleOrDefault(d => d.Name == name) ?? new ProductDefinition();

            productDefinition.Name = name;

            productDefinition.Save();
            return productDefinition;
        }

        private void CreateShirtProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shirt");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "CollarSize", "Number", true, true, "Collar Inches");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "Colour", "Colour", true, true, "Colour");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", true, true, "Show On Homepage");
        }

        private void CreateShoeProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shoe");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShoeSize", "ShortText", true, true, "Shoe Size");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", true, true, "Show On Homepage");
        }

        private void CreateAccessoryProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Accessory");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", true, true, "Show On Homepage");
        }

        private void AddProductDefinitionFieldIfDoesntExist(ProductDefinition definition, string name, string typeName, bool displayOnWebsite, bool variantProperty, string displayName)
        {
            if (definition.GetDefinitionFields().Any(f => f.Name == name))
                return;

            var field = ProductDefinitionField.SingleOrDefault(f => f.Name == name && f.ProductDefinition.ProductDefinitionId == definition.ProductDefinitionId) ?? new ProductDefinitionFieldFactory().NewWithDefaults(name);
            field.Name = name;
            field.DataType = DataType.SingleOrDefault(d => d.TypeName == typeName);
            field.Deleted = false;
            field.Multilingual = false;
            field.DisplayOnSite = displayOnWebsite;
            field.IsVariantProperty = variantProperty;
            field.RenderInEditor = true;

            //DoForEachCulture(language =>
            //    {
            //        if (field.GetDescription(language) == null)
            //            field.AddProductDefinitionFieldDescription(new ProductDefinitionFieldDescription { CultureCode = language, DisplayName = displayName, Description = displayName });
            //    });

            definition.AddProductDefinitionField(field);
            definition.Save();
        }
    }
}