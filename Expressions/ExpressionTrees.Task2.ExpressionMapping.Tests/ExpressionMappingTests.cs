using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    [TestClass]
    public class ExpressionMappingTests
    {
        [TestMethod]
        public void Test_ObjectFieldsAreCopied_WithoutParameters()
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();

            var foo = new Foo();

            var bar = mapper.Map(foo);

            Assert.AreEqual(foo.A, bar.A);
            Assert.AreEqual(foo.B, bar.B);
            Assert.AreEqual(foo.C, bar.C);
            Assert.AreEqual(bar.D, 0);
        }

        [TestMethod]
        public void Test_ObjectFieldsAreCopied_WithParameters()
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();

            var foo = new Foo
            {
                A = 3,
                B = "2",
                C = true
            };

            var bar = mapper.Map(foo);

            Assert.AreEqual(foo.A, bar.A);
            Assert.AreEqual(foo.B, bar.B);
            Assert.AreEqual(foo.C, bar.C);
            Assert.AreEqual(bar.D, 0);
        }

        [TestMethod]
        public void Test_ObjectFieldsAreCopied_WithParametersOverflow()
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Bar, Foo>();

            var bar = new Bar
            {
                A = 3,
                B = "2",
                C = true,
                D = 10
            };

            var foo = mapper.Map(bar);

            Assert.AreEqual(foo.A, bar.A);
            Assert.AreEqual(foo.B, bar.B);
            Assert.AreEqual(foo.C, bar.C);
            Assert.AreEqual(bar.D, 10);
        }
    }
}
