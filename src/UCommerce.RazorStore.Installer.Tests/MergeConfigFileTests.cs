using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.RazorStore.Installer.Tests
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    using NUnit.Framework;

    using Shouldly;

    using UCommerce.RazorStore.Installer.PackageActions;

    class MergeConfigFileTests
    {
        public class ExecuteTests
        {
            private MergeConfigFile _merger;

            private string _targetXmlPath;
            private string _sourceXmlPath;
            private string _sourceIIS7XmlPath;

            [SetUp]
            public void Initialise()
            {
                _merger = new MergeConfigFile();
                _targetXmlPath = createTargetXmlDocument();
                _sourceXmlPath = createSourceXmlDocument();
                _sourceIIS7XmlPath = createSourceIIS7XmlDocument();
            }

            [TearDown]
            public void Dispose()
            {
                deleteXmlDocument(_targetXmlPath);
                deleteXmlDocument(_sourceXmlPath);
                deleteXmlDocument(_sourceIIS7XmlPath);
            }

            [Test]
            public void MergesSimpleConfig()
            {
                var expectedFile = createExpectedXmlDocument();
                var expected = File.ReadAllText(expectedFile);

                var ranOk = _merger.Execute("TEST", SettingsXmlNode().ChildNodes[0]);
                Assert.IsTrue(ranOk);

                var actual = File.ReadAllText(_targetXmlPath);
                actual.ShouldBe(expected);
            }

            public XmlNode SettingsXmlNode()
            {
                var element = new XElement("Action",
                        new XAttribute("runat", "install"),
                        new XAttribute("undo", "true"),
                        new XAttribute("alias", _merger.Alias()),
                        new XAttribute("targetConfig", _targetXmlPath),
                        new XAttribute("sourceConfig", _sourceXmlPath),
                        new XAttribute("sourceConfigIntegratedMode", _sourceIIS7XmlPath));

                return GetXmlNode(element);
            }

            public static XmlNode GetXmlNode(XElement element)
            {
                using (var xmlReader = element.CreateReader())
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlReader);
                    return xmlDoc;
                }
            }
            
            private string createExpectedXmlDocument()
            {
                var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <appSettings>
        <add key=""umbracoReservedPaths"" value=""~/umbraco,~/install/,~/ucommerceapi/"" />
    </appSettings>
    <location path=""ucommerceapi"">
        <system.web>
            <httpHandlers>
                <add path=""*"" type=""ServiceStack.WebHost.Endpoints.ServiceStackHttpHandlerFactory, ServiceStack"" verb=""*""/>
            </httpHandlers>
        </system.web>
        <system.webServer>
            <modules runAllManagedModulesForAllRequests=""true""/>
            <validation validateIntegratedModeConfiguration=""false"" />
            <handlers>
                <add path=""*"" name=""ServiceStack.Factory"" type=""ServiceStack.WebHost.Endpoints.ServiceStackHttpHandlerFactory, ServiceStack"" verb=""*"" preCondition=""integratedMode"" resourceType=""Unspecified"" allowPathInfo=""true"" />
            </handlers>
        </system.webServer>
    </location>
</configuration>";
                var doc = createXmlDocument(xml);
                return saveXmlDocument(doc);
            }

            private string createTargetXmlDocument()
            {
                var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <appSettings>
        <add key=""umbracoReservedPaths"" value=""~/umbraco,~/install/,~/ucommerceapi/"" />
    </appSettings>
</configuration>";
                var doc = createXmlDocument(xml);
                return saveXmlDocument(doc);
            }

            private string createSourceXmlDocument()
            {
                var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <location path=""ucommerceapi"">
        <system.web>
            <httpHandlers>
                <add path=""*"" type=""ServiceStack.WebHost.Endpoints.ServiceStackHttpHandlerFactory, ServiceStack"" verb=""*""/>
            </httpHandlers>
        </system.web>
    </location>
</configuration>";
                var doc = createXmlDocument(xml);
                return saveXmlDocument(doc);
            }

            private string createSourceIIS7XmlDocument()
            {
                var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <location path=""ucommerceapi"">
        <system.webServer>
            <modules runAllManagedModulesForAllRequests=""true""/>
            <validation validateIntegratedModeConfiguration=""false"" />
            <handlers>
                <add path=""*"" name=""ServiceStack.Factory"" type=""ServiceStack.WebHost.Endpoints.ServiceStackHttpHandlerFactory, ServiceStack"" verb=""*"" preCondition=""integratedMode"" resourceType=""Unspecified"" allowPathInfo=""true"" />
            </handlers>
        </system.webServer>
    </location>
</configuration>";
                var doc = createXmlDocument(xml);
                return saveXmlDocument(doc);
            }

            private XmlDocument createXmlDocument(string xmlToLoad)
            {
                var doc = new XmlDocument();

                doc.LoadXml(xmlToLoad);
                return doc;
            }

            private string saveXmlDocument(XmlDocument doc)
            {
                var tempDir = Path.GetTempPath();
                var tempPath = Path.Combine(tempDir, String.Format("{0}.xml", Guid.NewGuid()));
                doc.Save(tempPath);
                return tempPath;
            }

            private void deleteXmlDocument(string path)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}
