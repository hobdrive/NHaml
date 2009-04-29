using System.Linq;
using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParserTests
    {
        private static void AssertAttribute(AttributeParser parser, string name, string value, NHamlAttributeType type)
        {
            var attribute = parser.Attributes.FirstOrDefault(a => a.Name == name);

            Assert.IsNotNull(attribute, string.Format("Attribute '{0}' not found.", name));

            Assert.AreEqual(value, attribute.Value);
            Assert.AreEqual(type, attribute.Type);
        }

        [Test]
        public void DoubleAndSingleQuotesEncoded()
        {
            var parser = new AttributeParser(@"b='a\'b\'' d=""\""d\""e""");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "b", "a\\'b\\'", NHamlAttributeType.String);
            AssertAttribute(parser, "d", "\\\"d\\\"e", NHamlAttributeType.String);
        }

        [Test]
        public void DoubleQuotes()
        {
            var parser = new AttributeParser("a=\"b\" c=\"d\" e=\"f\" ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", NHamlAttributeType.String);
            AssertAttribute(parser, "c", "d", NHamlAttributeType.String);
            AssertAttribute(parser, "e", "f", NHamlAttributeType.String);
        }

        [Test]
        public void ExpressionInsideOfDoubleSingleQuotesAndEncodedQuotes()
        {
            var parser = new AttributeParser("a='#{\"a\"}' c=\"#{a}\"");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "a", "#{\"a\"}", NHamlAttributeType.String);
            AssertAttribute(parser, "c", "#{a}", NHamlAttributeType.String);
        }

        [Test]
        public void Expressions()
        {
            var parser = new AttributeParser("a=#{1+1} b=#{\"t\"} c=#{f.ToString()}");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "1+1", NHamlAttributeType.Dynamic);
            AssertAttribute(parser, "b", "\"t\"", NHamlAttributeType.Dynamic);
            AssertAttribute(parser, "c", "f.ToString()", NHamlAttributeType.Dynamic);
        }

        [Test]
        public void OnlyReference()
        {
            var parser = new AttributeParser("a c e");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", NHamlAttributeType.Reference);
            AssertAttribute(parser, "c", "c", NHamlAttributeType.Reference);
            AssertAttribute(parser, "e", "e", NHamlAttributeType.Reference);
        }

        [Test]
        public void ReferenceAsValue()
        {
            var parser = new AttributeParser("a=b c=d e=f");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", NHamlAttributeType.Reference);
            AssertAttribute(parser, "c", "d", NHamlAttributeType.Reference);
            AssertAttribute(parser, "e", "f", NHamlAttributeType.Reference);
        }


        [Test]
        public void SingleQuotes()
        {
            var parser = new AttributeParser("a='b' c='d' e='f'");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", NHamlAttributeType.String);
            AssertAttribute(parser, "c", "d", NHamlAttributeType.String);
            AssertAttribute(parser, "e", "f", NHamlAttributeType.String);
        }

        [Test]
        public void SpacesBettwenKeyAndValue()
        {
            var parser = new AttributeParser("a =a b= b c = 'c'  d  =  #{d} ");

            parser.Parse();

            Assert.AreEqual(4, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", NHamlAttributeType.Reference);
            AssertAttribute(parser, "b", "b", NHamlAttributeType.Reference);
            AssertAttribute(parser, "c", "c", NHamlAttributeType.String);
            AssertAttribute(parser, "d", "d", NHamlAttributeType.Dynamic);
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnEmtpyAfterEqual()
        {
            new AttributeParser(@" a= ").Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnForgottenSingleQuoteClose()
        {
            new AttributeParser( @" a='text " ).Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnForgottenSingleQuoteOpen()
        {
            new AttributeParser( @" a=text' " ).Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnForgottenDoubleSingleQuoteClose()
        {
            new AttributeParser( @" a=""text " ).Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnForgottenDoubleSingleQuoteOpen()
        {
            new AttributeParser( @" a=text"" " ).Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnForgottenDynamicClose()
        {
            new AttributeParser( @" a=#{text " ).Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnForgottenDynamicOpen()
        {
            new AttributeParser( @" a=text} " ).Parse();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ) )]
        public void ThrowExceptionOnOnlyShema()
        {
            new AttributeParser( @" a: " ).Parse();
        }
    }
}