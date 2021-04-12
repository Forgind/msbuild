// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Build.Shared;

namespace Microsoft.Build.Framework
{
    /// <summary>
    /// Arguments for error events
    /// </summary>
    // WARNING: marking a type [Serializable] without implementing
    // ISerializable imposes a serialization contract -- it is a
    // promise to never change the type's fields i.e. the type is
    // immutable; adding new fields in the next version of the type
    // without following certain special FX guidelines, can break both
    // forward and backward compatibility
    [Serializable]
    public class BuildErrorEventArgs : LazyFormattedBuildEventArgs
    {
        /// <summary>
        /// Subcategory of the error
        /// </summary>
        internal string subcategory;

        /// <summary>
        /// Error code
        /// </summary>
        internal string code;

        /// <summary>
        /// File name
        /// </summary>
        internal string file;

        /// <summary>
        /// The project which issued the event
        /// </summary>
        internal string projectFile;

        /// <summary>
        /// Line number
        /// </summary>
        internal int lineNumber;

        /// <summary>
        /// Column number
        /// </summary>
        internal int columnNumber;

        /// <summary>
        /// End line number
        /// </summary>
        internal int endLineNumber;

        /// <summary>
        /// End column number
        /// </summary>
        internal int endColumnNumber;

        /// <summary>
        /// A link pointing to more information about the error
        /// </summary>
        internal string helpLink;

        /// <summary>
        /// This constructor allows all event data to be initialized
        /// </summary>
        /// <param name="subcategory">event sub-category</param>
        /// <param name="code">event code</param>
        /// <param name="file">file associated with the event</param>
        /// <param name="lineNumber">line number (0 if not applicable)</param>
        /// <param name="columnNumber">column number (0 if not applicable)</param>
        /// <param name="endLineNumber">end line number (0 if not applicable)</param>
        /// <param name="endColumnNumber">end column number (0 if not applicable)</param>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="senderName">name of event sender</param>
        public BuildErrorEventArgs
            (
            string subcategory,
            string code,
            string file,
            int lineNumber,
            int columnNumber,
            int endLineNumber,
            int endColumnNumber,
            string message,
            string helpKeyword,
            string senderName
            )
            : this(subcategory, code, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, helpKeyword, senderName, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// This constructor which allows a timestamp to be set
        /// </summary>
        /// <param name="subcategory">event sub-category</param>
        /// <param name="code">event code</param>
        /// <param name="file">file associated with the event</param>
        /// <param name="lineNumber">line number (0 if not applicable)</param>
        /// <param name="columnNumber">column number (0 if not applicable)</param>
        /// <param name="endLineNumber">end line number (0 if not applicable)</param>
        /// <param name="endColumnNumber">end column number (0 if not applicable)</param>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="senderName">name of event sender</param>
        /// <param name="eventTimestamp">Timestamp when event was created</param>
        public BuildErrorEventArgs
            (
            string subcategory,
            string code,
            string file,
            int lineNumber,
            int columnNumber,
            int endLineNumber,
            int endColumnNumber,
            string message,
            string helpKeyword,
            string senderName,
            DateTime eventTimestamp
            )
            : this(subcategory, code, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, helpKeyword, senderName, null, eventTimestamp, null)
        {
            // do nothing
        }


        /// <summary>
        /// This constructor which allows a timestamp to be set
        /// </summary>
        /// <param name="subcategory">event sub-category</param>
        /// <param name="code">event code</param>
        /// <param name="file">file associated with the event</param>
        /// <param name="lineNumber">line number (0 if not applicable)</param>
        /// <param name="columnNumber">column number (0 if not applicable)</param>
        /// <param name="endLineNumber">end line number (0 if not applicable)</param>
        /// <param name="endColumnNumber">end column number (0 if not applicable)</param>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="senderName">name of event sender</param>
        /// <param name="eventTimestamp">Timestamp when event was created</param>
        /// <param name="messageArgs">message arguments</param>
        public BuildErrorEventArgs
            (
            string subcategory,
            string code,
            string file,
            int lineNumber,
            int columnNumber,
            int endLineNumber,
            int endColumnNumber,
            string message,
            string helpKeyword,
            string senderName,
            DateTime eventTimestamp,
            params object[] messageArgs
            )
            : this(subcategory, code, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, helpKeyword, senderName, null, eventTimestamp, messageArgs)
        {
            // do nothing
        }

        /// <summary>
        /// This constructor which allows a timestamp to be set
        /// </summary>
        /// <param name="subcategory">event sub-category</param>
        /// <param name="code">event code</param>
        /// <param name="file">file associated with the event</param>
        /// <param name="lineNumber">line number (0 if not applicable)</param>
        /// <param name="columnNumber">column number (0 if not applicable)</param>
        /// <param name="endLineNumber">end line number (0 if not applicable)</param>
        /// <param name="endColumnNumber">end column number (0 if not applicable)</param>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="helpLink">A link pointing to more information about the error </param>
        /// <param name="senderName">name of event sender</param>
        /// <param name="eventTimestamp">Timestamp when event was created</param>
        /// <param name="messageArgs">message arguments</param>
        public BuildErrorEventArgs
            (
            string subcategory,
            string code,
            string file,
            int lineNumber,
            int columnNumber,
            int endLineNumber,
            int endColumnNumber,
            string message,
            string helpKeyword,
            string senderName,
            string helpLink,
            DateTime eventTimestamp,
            params object[] messageArgs
            )
            : base(message, helpKeyword, senderName, eventTimestamp, messageArgs)
        {
            this.subcategory = subcategory;
            this.code = code;
            this.file = file;
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
            this.endLineNumber = endLineNumber;
            this.endColumnNumber = endColumnNumber;
            this.helpLink = helpLink;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected BuildErrorEventArgs()
            : base()
        {
            // do nothing
        }

        /// <summary>
        /// The custom sub-type of the event.
        /// </summary>
        public string Subcategory => subcategory;

        /// <summary>
        /// Code associated with event.
        /// </summary>
        public string Code => code;

        /// <summary>
        /// File associated with event.
        /// </summary>
        public string File => file;

        /// <summary>
        /// The project file which issued this event.
        /// </summary>
        public string ProjectFile
        {
            get => projectFile;
            set => projectFile = value;
        }

        /// <summary>
        /// Line number of interest in associated file.
        /// </summary>
        public int LineNumber => lineNumber;

        /// <summary>
        /// Column number of interest in associated file.
        /// </summary>
        public int ColumnNumber => columnNumber;

        /// <summary>
        /// Ending line number of interest in associated file.
        /// </summary>
        public int EndLineNumber => endLineNumber;

        /// <summary>
        /// Ending column number of interest in associated file.
        /// </summary>
        public int EndColumnNumber => endColumnNumber;

        /// <summary>
        /// A link pointing to more information about the error.
        /// </summary>
        public string HelpLink => helpLink;
    }
}
