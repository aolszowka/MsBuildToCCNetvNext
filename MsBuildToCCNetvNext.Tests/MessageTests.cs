/// <copyright file="MessageTests.cs" company="Ace Olszowka">
/// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
/// </copyright>

namespace MsBuildToCCNetvNext.Tests
{
    using System;
    using Microsoft.Build.Framework;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class MessageTests
    {
        [Test]
        public void Message_XmlFragment_InvalidXmlCharacters()
        {
            string expected = "WARNING This message contained invalid XML character(s) which have been removed: This contains ENQUIRY (0x5) ";
            string input = "This contains ENQUIRY (0x5) \u0005";

            Message testMessage = BuildMessage(input, MessageImportance.High);
            string actual = testMessage.XmlFragement.Value;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Message_XmlFragment_ValidXMLCharacters()
        {
            string expected = "This contains all valid XML Characters";

            // Round tripping the above string should be the same
            Message testMessage = BuildMessage(expected, MessageImportance.High);
            string actual = testMessage.XmlFragement.Value;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Message_XmlFragment_EmptyString()
        {
            string expected = string.Empty;

            //Round tripping the above string should be the same
            Message testMessage = BuildMessage(expected, MessageImportance.High);
            string actual = testMessage.XmlFragement.Value;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Message_NullArgumentException()
        {
            ActualValueDelegate<object> testDelegate = () => new Message(null);

            Assert.That(testDelegate, Throws.TypeOf<ArgumentNullException>());
        }

        private static Message BuildMessage(BuildMessageEventArgs message)
        {
            return new Message(message);
        }

        private static Message BuildMessage(string text, MessageImportance importance)
        {
            BuildMessageEventArgs bmea = new BuildMessageEventArgs(text, "HelpKeyword", "SenderName", importance);
            return new Message(bmea);
        }


    }
}
