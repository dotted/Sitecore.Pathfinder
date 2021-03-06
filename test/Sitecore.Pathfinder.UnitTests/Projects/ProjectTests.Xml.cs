﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public partial class ProjectTests
    {
        [TestMethod]
        public void XmlContentItemTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Xml/Home");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("Home", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Xml/Home", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("Home", item.ItemName);
            Assert.AreEqual("/sitecore/content/Xml/Home", item.ItemIdOrPath);
            Assert.AreEqual("Sample Item", item.TemplateIdOrPath);
            Assert.AreNotEqual(item.ItemNameProperty.SourceTextNode, TextNode.Empty);
            Assert.IsInstanceOfType(item.TemplateIdOrPathProperty.SourceTextNode, typeof(AttributeNameTextNode));
            Assert.AreEqual("SampleItem", TraceHelper.GetTextNode(item.TemplateIdOrPathProperty).Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Welcome to Sitecore", field.Value);
            Assert.IsInstanceOfType(field.ValueProperty.SourceTextNode, typeof(TextNode));
            Assert.AreEqual("Welcome to Sitecore", field.ValueProperty.SourceTextNode.Value);
            Assert.AreEqual("Text", field.ValueProperty.SourceTextNode.Key);

            var textDocument = projectItem.Snapshot as ITextSnapshot;
            Assert.IsNotNull(textDocument);

            var treeNode = textDocument.Root;
            Assert.AreEqual("SampleItem", treeNode.Key);
            Assert.AreEqual(4, treeNode.Attributes.Count());
        }

        [TestMethod]
        public void XmlSubitemTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Xml/Home/Articles/TestingPathfinder");
            Assert.IsNotNull(projectItem);
        }

        /*
        [TestMethod]
        public void XmlLayoutTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/YamlLayout");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("YamlLayout", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/YamlLayout", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);

            var layout = item.Fields.FirstOrDefault(f => f.FieldName == "__Renderings");
            Assert.IsNotNull(layout);
            Assert.AreEqual(@"<r>  <d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{5B2B5845-4D8A-FBB3-08D3-9A6065C35D1E}"">    <r id=""{663E1E86-C959-7A70-8945-CFCEA79AFAC2}"" ds=""{11111111-1111-1111-1111-111111111111}"" par="""" ph=""Page.Body"" />    <r id=""{663E1E86-C959-7A70-8945-CFCEA79AFAC2}"" par="""" ph=""Page.Body"" />    <r id=""{663E1E86-C959-7A70-8945-CFCEA79AFAC2}"" par="""" ph=""Page.Body"" />  </d></r>", layout.CompiledValue.Replace("\r", string.Empty).Replace("\n", string.Empty));
        }
        */

        [TestMethod]
        public void WriteAsXmlTest()
        {
            var item = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/client/Applications/SitecoreWorks/content/content-editor/ribbon1/Home");
            Assert.IsNotNull(item);

            var writer = new StringWriter();

            item.WriteAsXml(writer);

            var result = writer.ToString();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
