using NUnit.Framework;
using SmartApp.Models;
using SmartApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartApp.UnitTest
{
    [TestFixture]
    public class GeneralClassTests 
    {
        [TestCase("<td class=\"strength\">37435</td>\r\n<td class=\"rating\"><strong>4767</strong></td>", 21, 25, 37435)]
        [TestCase("1234", 0, 3, 1234)]
        [TestCase(" 100 ", 0, 4, 100)]
        [TestCase("5", 0, 0, 5)]
        public void GetNumericFromText_SuccessParsing_ReturnNumeric(string text, int startIndex, int endIndex, int excpectedResult)
        {
            int result = GeneralClass.GetNumericFromText(text, startIndex, endIndex);

            Assert.AreEqual(excpectedResult, result); 
        }

        [TestCase("", 0, 0, 0)]
        [TestCase("0", 0, 0, 0)]
        public void GetNumericFromText_FaledParsing_ReturnZero(string text, int startIndex, int endIndex, int excpectedResult)
        {
            int result = GeneralClass.GetNumericFromText(text, startIndex, endIndex);

            Assert.AreEqual(excpectedResult, result);
        }

        [TestCase("f100", -1, 1)]
        [Test] 
        public void GetNumericFromText_IncorrectStartIndex_Throws(string text, int startIndex, int endIndex)
        {
            var ex = Assert.Catch<Exception>(() => GeneralClass.GetNumericFromText(text, startIndex, endIndex));
            StringAssert.Contains("start index is negative", ex.Message);
        }

        [TestCase("f100", 1, 0)]
        [Test]
        public void GetNumericFromText_IncorrectEndIndex_Throws(string text, int startIndex, int endIndex)
        {
            var ex = Assert.Catch<Exception>(() => GeneralClass.GetNumericFromText(text, startIndex, endIndex));
            StringAssert.Contains("end index less than start index", ex.Message);
        }

    }

}
