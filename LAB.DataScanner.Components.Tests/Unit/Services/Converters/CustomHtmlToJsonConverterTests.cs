using LAB.DataScanner.Components.Services.Converters.ConvertStrategies;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Converters
{
    public class CustomHtmlToJsonConverterTests
    {
        const string html = @"
            <html>
                <head></head>
                <body>
                    <product>
                        <h1 id='title'>Product 1</h1>
                        <h2 id='prop'>Prop of prod 1</h2>
                        <h3 id='style'>Style of prod 1</h2>
                    </product>
                    <product>
                        <h1 id='title'>Product 2</h1>
                        <h2 id='prop'>Prop of prod 2</h2>
                    </product>
                </body>";

        [Test]
        public void Constructor_NullExpressions_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new CustomHtmlToJsonConverter(null));

        [Test]
        public void Constructor_InvalidExpression_ThrowsArgumentException() =>
            Assert.Throws<ArgumentException>(() => new CustomHtmlToJsonConverter(new Dictionary<string, string>()));

        [Test]
        public void ConvertAsync_InvalidExpression_ThrowsXPathException()
        {
            var sut = new CustomHtmlToJsonConverter(new Dictionary<string, string>() { { "item", "invalid expression!" } });
            Assert.ThrowsAsync<XPathException>(async () => await sut.ConvertAsync(html));
        }
            
        [Test]
        public async Task Convert_MultipleFields_ReturnCorrectJson()
        {
            var expressions = new Dictionary<string, string>() {
                { "DataItem1", "//h1"},
                { "DataItem2", "//h2"}
            };

            var expectedjson = "[{\"DataItem1\":\"Product 1\",\"DataItem2\":\"Prop of prod 1\"},{\"DataItem1\":\"Product 2\",\"DataItem2\":\"Prop of prod 2\"}]";
                        
            var converter = new CustomHtmlToJsonConverter(expressions);

            var result = await converter.ConvertAsync(html);

            Assert.AreEqual(expectedjson, result);
        }

        [Test]
        public async Task Convert_ValidHtmlSingleField_ReturnCorrectJson()
        {
            var expressions = new Dictionary<string, string>() {
                { "DataItem1", "//h1"}
            };

            var expectedjson = "[{\"DataItem1\":\"Product 1\"},{\"DataItem1\":\"Product 2\"}]";

            var converter = new CustomHtmlToJsonConverter(expressions);

            var result = await converter.ConvertAsync(html);

            Assert.AreEqual(expectedjson, result);
        }

        [Test]
        public async Task Convert_PartlyMatchesHeaderExist_ReturnCorrectJson()
        {
            var expressions = new Dictionary<string, string>() {
                { "DataItem1", "//h1"},
                { "DataItem2", "//h2"},
                { "DataItem3", "//h3"},
            };

            var expectedjson = "[{\"DataItem1\":\"Product 1\",\"DataItem2\":\"Prop of prod 1\",\"DataItem3\":\"Style of prod 1\"},{\"DataItem1\":\"Product 2\",\"DataItem2\":\"Prop of prod 2\",\"DataItem3\":null}]";
            var converter = new CustomHtmlToJsonConverter(expressions);

            var result = await converter.ConvertAsync(html);

            Assert.AreEqual(expectedjson, result);
        }

        [Test]
        public async Task Convert_ValidHtmlPartlyMatchesHeaderNotExist_ReturnCorrectJson()
        {
            var expressions = new Dictionary<string, string>() {
                { "DataItem1", "//h1"},
                { "DataItem2", "//h5"}
            };

            var expectedjson = "[{\"DataItem1\":\"Product 1\",\"DataItem2\":null},{\"DataItem1\":\"Product 2\",\"DataItem2\":null}]";
            var converter = new CustomHtmlToJsonConverter(expressions);

            var result = await converter.ConvertAsync(html);

            Assert.AreEqual(expectedjson, result);
        }

        [Test]
        public async Task Convert_ValidHtmlNoMatches_ReturnCorrectJson()
        {
            var expressions = new Dictionary<string, string>() {
                { "DataItem1", "//h5"},
                { "DataItem2", "//h6"}
            };

            var expectedjson = "[]";
            var converter = new CustomHtmlToJsonConverter(expressions);

            var result = await converter.ConvertAsync(html);

            Assert.AreEqual(expectedjson, result);
        }
    }
}
