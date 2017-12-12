/// <copyright file="UtilitiesTests.cs" company="Ace Olszowka">
/// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
/// </copyright>
/// 
namespace MsBuildToCCNetvNext.Tests
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class UtilitiesTests
    {
        [Test]
        public void Utilities_MessageNeedsSanitation_RequiresSanitation()
        {
            bool expected = true;
            string input = "This contains ENQUIRY (0x5) \u0005";

            bool actual = Utilities.MessageNeedsSanitation(input);

            Assert.That(actual, Is.EqualTo(expected), "The given string containing unprintable character \\u0005 (ENQUIRY) should have required sanitation.");
        }

        [Test]
        public void Utilities_MessageNeedsSanitation_ValidString()
        {
            bool expected = false;
            string input = "This is a valid Xml String";

            bool actual = Utilities.MessageNeedsSanitation(input);

            Assert.That(actual, Is.EqualTo(expected), "A Valid string should not need sanitation.");
        }

        [Test]
        public void Utilities_MessageNeedsSanitation_EmptyString()
        {
            bool expected = false;
            string input = string.Empty;

            bool actual = Utilities.MessageNeedsSanitation(input);

            Assert.That(actual, Is.EqualTo(expected), "An Empty string should not require sanitation.");
        }

        [Test]
        public void Utilities_MessageNeedsSanitation_NullArgument()
        {
            ActualValueDelegate<object> testDelegate = () => Utilities.MessageNeedsSanitation(null);

            Assert.That(testDelegate, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Utilities_SanitizeMessageForXml_ValidString()
        {
            string expected = "This is a valid string.";
            string input = expected;

            string actual = Utilities.SanitizeMessageForXml(input);

            Assert.That(actual, Is.EqualTo(expected), "Round tripping an valid string should return the same string.");
        }

        [Test]
        public void Utilities_SanitizeMessageForXml_InvalidXmlString()
        {
            string expected = "WARNING This message contained invalid XML character(s) which have been removed: This contains ENQUIRY (0x5) ";
            string input = "This contains ENQUIRY (0x5) \u0005";

            string actual = Utilities.SanitizeMessageForXml(input);

            Assert.That(actual, Is.EqualTo(expected), "Invalid strings should be sanitized with the message");
        }

        [Test]
        public void Utilities_SanitizeMessageForXml_EmptyString()
        {
            string expected = string.Empty;
            string input = expected;

            string actual = Utilities.SanitizeMessageForXml(input);

            Assert.That(actual, Is.EqualTo(expected), "Round tripping an empty string should return an empty string.");
        }

        [Test]
        public void Utilities_SanitizeMessageForXml_NullArgument()
        {
            ActualValueDelegate<object> testDelegate = () => Utilities.SanitizeMessageForXml(null);

            Assert.That(testDelegate, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
