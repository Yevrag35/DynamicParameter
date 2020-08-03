using MG.Dynamic.Parameter;
using MG.Dynamic.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic.Tests
{
    [TestClass]
    public class RuntimeParameterTest
    {
        [TestMethod]
        public void TestConstruction()
        {
            var runtime = new RuntimeParameter();
            Assert.IsTrue(string.IsNullOrEmpty(runtime.Name));
            Assert.IsFalse(runtime.Mandatory);
            Assert.IsNull(runtime.ParameterType);
            Assert.IsNull(runtime.Position);

            Assert.IsNotNull(runtime.Aliases);
            Assert.AreEqual(0, runtime.Aliases.Count);
            runtime.Aliases.Add("test");
            Assert.AreEqual(1, runtime.Aliases.Count);
        }

        [TestMethod]
        public void TestProperties1()
        {
            var runtime = new RuntimeParameter("test")
            {
                AllowEmptyCollection = true,
                AllowEmptyString = true,
                AllowNull = true,
                DontShow = true,
                HelpMessage = "Help me",
                HelpMessageBaseName = "whatever",
                HelpMessageResourceId = "huh?",
                Key = "the key",
                Mandatory = true,
                ParameterSetName = "TheOne",
                ParameterType = typeof(string[]),
                Position = 14,
                SupportsWildcards = true,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true,
                ValueFromRemainingArguments = true
            };

            ParameterAttribute pAtt = runtime.MakeParameterAttribute();

            // Test
            Assert.AreEqual(runtime.DontShow, pAtt.DontShow);
            Assert.AreEqual(runtime.HelpMessage, pAtt.HelpMessage);
            Assert.AreEqual(runtime.HelpMessageBaseName, pAtt.HelpMessageBaseName);
            Assert.AreEqual(runtime.HelpMessageResourceId, pAtt.HelpMessageResourceId);
            Assert.AreEqual(runtime.ParameterSetName, pAtt.ParameterSetName);
            Assert.AreEqual(runtime.Position, pAtt.Position);
            Assert.AreEqual(runtime.ValueFromPipeline, pAtt.ValueFromPipeline);
            Assert.AreEqual(runtime.ValueFromPipelineByPropertyName, pAtt.ValueFromPipelineByPropertyName);
            Assert.AreEqual(runtime.ValueFromRemainingArguments, pAtt.ValueFromRemainingArguments);

            var rtParam = runtime.AsRuntimeDefinedParameter();
            Assert.IsNotNull(rtParam.Attributes);
            Assert.AreEqual(5, rtParam.Attributes.Count);
            Assert.IsTrue(rtParam.Attributes.All(x =>
                x is AllowEmptyCollectionAttribute
                ||
                x is AllowEmptyStringAttribute
                ||
                x is AllowNullAttribute
                ||
                x is SupportsWildcardsAttribute
                ||
                x is ParameterAttribute));

            Assert.AreEqual(runtime.ParameterType, rtParam.ParameterType);
            Assert.IsNull(rtParam.Value);
            Assert.IsFalse(rtParam.IsSet);
            Assert.AreEqual(runtime.Name, rtParam.Name);
        }

        [TestMethod]
        public void TestProperties2()
        {
            var parameter = new RuntimeParameter("hi");
            Assert.IsNotNull(parameter.ValidatedItems);

            parameter.ParameterType = typeof(string[]);
            parameter.ValidatedItems.UnionWith(new string[3] { "hi", "what", "WHAT" });
            Assert.AreEqual(3, parameter.ValidatedItems.Count);

            var rtParam = parameter.AsRuntimeDefinedParameter();
            Assert.AreEqual(2, rtParam.Attributes.Count);
            Assert.IsTrue(rtParam.Attributes.Any(x => x is ValidateSetAttribute));
            Assert.AreEqual(3, rtParam.Attributes.Where(x => x is ValidateSetAttribute).Cast<ValidateSetAttribute>().FirstOrDefault().ValidValues.Count);
        }
    }
}
