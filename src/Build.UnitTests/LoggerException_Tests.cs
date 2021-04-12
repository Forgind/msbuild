// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Exceptions;
using Xunit;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Build.BackEnd;

namespace Microsoft.Build.UnitTests
{
    public class InternalLoggerExceptionTests
    {
        /// <summary>
        /// Verify I implemented ISerializable correctly
        /// </summary>
        [Fact]
        public void SerializeDeserialize()
        {
            InternalLoggerException e = new InternalLoggerException(
                "message",
                new Exception("innerException"),
                new BuildStartedEventArgs("evMessage", "evHelpKeyword"),
                "errorCode",
                "helpKeyword",
                false);

            using (MemoryStream memstr = new MemoryStream())
            {
                Errors.TranslatorHelpers.Translate(BinaryTranslator.GetWriteTranslator(memstr), ref e);
                memstr.Position = 0;
                InternalLoggerException e2 = null;
                Errors.TranslatorHelpers.Translate(BinaryTranslator.GetReadTranslator(memstr, buffer: null), ref e2);

                Assert.Equal(e.BuildEventArgs.Message, e2.BuildEventArgs.Message);
                Assert.Equal(e.BuildEventArgs.HelpKeyword, e2.BuildEventArgs.HelpKeyword);
                Assert.Equal(e.ErrorCode, e2.ErrorCode);
                Assert.Equal(e.HelpKeyword, e2.HelpKeyword);
                Assert.Equal(e.Message, e2.Message);
                Assert.Equal(e.InnerException.Message, e2.InnerException.Message);
            }
        }
    }
}
