// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace Microsoft.Build.Framework
{
    /// <summary>
    /// This class represents the event arguments for build finished events.
    /// </summary>
    // WARNING: marking a type [Serializable] without implementing
    // ISerializable imposes a serialization contract -- it is a
    // promise to never change the type's fields i.e. the type is
    // immutable; adding new fields in the next version of the type
    // without following certain special FX guidelines, can break both
    // forward and backward compatibility
    [Serializable]
    public class BuildFinishedEventArgs : BuildStatusEventArgs
    {
        /// <summary>
        /// Whether the build succeeded
        /// </summary>
        internal bool succeeded;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected BuildFinishedEventArgs()
            : base()
        {
            // do nothing
        }

        /// <summary>
        /// Constructor to initialize all parameters.
        /// Sender field cannot be set here and is assumed to be "MSBuild"
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="succeeded">True indicates a successful build</param>
        public BuildFinishedEventArgs
        (
            string message,
            string helpKeyword,
            bool succeeded
        )
            : this(message, helpKeyword, succeeded, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Constructor which allows the timestamp to be set
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="succeeded">True indicates a successful build</param>
        /// <param name="eventTimestamp">Timestamp when the event was created</param>
        public BuildFinishedEventArgs
        (
            string message,
            string helpKeyword,
            bool succeeded,
            DateTime eventTimestamp
        )
            : this(message, helpKeyword, succeeded, eventTimestamp, null)
        {
            // do nothing
        }

        /// <summary>
        /// Constructor which allows the timestamp to be set
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="succeeded">True indicates a successful build</param>
        /// <param name="eventTimestamp">Timestamp when the event was created</param>
        /// <param name="messageArgs">message arguments</param>
        public BuildFinishedEventArgs
        (
            string message,
            string helpKeyword,
            bool succeeded,
            DateTime eventTimestamp,
            params object[] messageArgs
        )
            : base(message, helpKeyword, "MSBuild", eventTimestamp, messageArgs)
        {
            this.succeeded = succeeded;
        }

        /// <summary>
        /// Succeeded is true if the build succeeded; false otherwise.
        /// </summary>
        public bool Succeeded
        {
            get
            {
                return succeeded;
            }
        }
    }
}
