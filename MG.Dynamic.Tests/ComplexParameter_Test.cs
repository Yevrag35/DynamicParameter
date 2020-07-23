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
    public class ComplexParameterTest
    {
        [TestMethod]
        public void TestConstruction()
        {
            var complex = new ComplexDynamicParameter<IEmployee, int>("Employee");
            complex.Name = null;
            Assert.ThrowsException<InvalidOperationException>(() => complex.AsRuntimeDefinedParameter());
        }

        [TestMethod]
        public void TestParameter()
        {
            Assembly gotcha = Assembly.GetAssembly(typeof(RuntimeDefinedParameter));

            var complex = new ComplexDynamicParameter<IEmployee, string>("Employee");
            Assert.IsFalse(string.IsNullOrEmpty(complex.Name));
            Assert.AreEqual("Employee", complex.Name);
            Assert.IsNotNull(complex.ValidatedItems);
            Assert.IsTrue(complex.BackingItemType == typeof(IEmployee));

            var employees = new List<IEmployee>(3)
            {
                this.GetEmployee1(),
                this.GetEmployee2(),
                this.GetEmployee3()
            };

            complex.SetDynamicItems(employees, true, x => x.Name);
            Assert.IsTrue(complex.BackingItemType == typeof(IEmployee));
            Assert.IsTrue(complex.ParameterType == typeof(string[]));
            Assert.IsTrue(employees.TrueForAll(x => complex.BackingItems.Contains(x)));

            Assert.IsNotNull(complex.PropertyFunction);

            Assert.AreEqual(employees.Find(x => x.Name == "Jim Jones"), complex.GetPropertyFromValue("Jim Jones"));
            Assert.AreEqual(1, complex.GetPropertiesFromValue("Jim Jordan").Count());
            Assert.AreEqual(1, complex.GetPropertiesFromValues(new string[1] { "Go Home" }).Count());

            RuntimeDefinedParameter rtParam = complex.AsRuntimeDefinedParameter();
            Assert.IsNotNull(rtParam);
            Assert.AreEqual("Employee", rtParam.Name);
            Assert.AreEqual(3, rtParam.Attributes.Count);
        }

        [TestMethod]
        public void TestParameter2()
        {
            var employees = new List<IEmployee>(3)
            {
                this.GetEmployee1(),
                this.GetEmployee2(),
                this.GetEmployee3()
            };

            Type intType = typeof(int);

            var complex = new ComplexDynamicParameter<IEmployee, int>("Employee");
            Assert.AreEqual(intType, complex.ParameterType);

            complex.SetDynamicItems(employees, x => x.Id);
            Assert.AreEqual(intType, complex.ParameterType);
        }

        public IEmployee GetEmployee1()
        {
            var mock = new Mock<IEmployee>();
            mock.SetupGet(x => x.Id).Returns(781726);
            mock.SetupGet(x => x.Name).Returns("Jim Jones");
            return mock.Object;
        }
        public IEmployee GetEmployee2()
        {
            var mock = new Mock<IEmployee>();
            mock.SetupGet(x => x.Id).Returns(237215);
            mock.SetupGet(x => x.Name).Returns("Jim Jordan");
            return mock.Object;
        }
        public IEmployee GetEmployee3()
        {
            var mock = new Mock<IEmployee>();
            mock.SetupGet(x => x.Id).Returns(2413);
            mock.SetupGet(x => x.Name).Returns("Go Home");
            return mock.Object;
        }
    }
}
