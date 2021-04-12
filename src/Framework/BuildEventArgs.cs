// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Build.Shared;

namespace Microsoft.Build.Framework
{
    /// <summary>
    /// This class encapsulates the default data associated with build events. 
    /// It is intended to be extended/sub-classed.
    /// </summary>
    public abstract class BuildEventArgs : EventArgs
    {
        /// <summary>
        /// Message
        /// </summary>
        internal string message;

        /// <summary>
        /// Help keyword
        /// </summary>
        internal string helpKeyword;

        /// <summary>
        /// Sender name
        /// </summary>
        internal string senderName;

        /// <summary>
        /// Timestamp
        /// </summary>
        internal DateTime timestamp;

        [NonSerialized]
        internal DateTime? _localTimestamp;

        /// <summary>
        /// Thread id
        /// </summary>
        internal int threadId;

        /// <summary>
        /// Build event context
        /// </summary>
        [OptionalField(VersionAdded = 2)]
        internal BuildEventContext buildEventContext;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected BuildEventArgs()
            : this(null, null, null, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// This constructor allows all event data to be initialized
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="senderName">name of event sender</param>
        protected BuildEventArgs(string message, string helpKeyword, string senderName)
            : this(message, helpKeyword, senderName, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// This constructor allows all event data to be initialized while providing a custom timestamp.
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="senderName">name of event sender</param>
        /// <param name="eventTimestamp">TimeStamp of when the event was created</param>
        protected BuildEventArgs(string message, string helpKeyword, string senderName, DateTime eventTimestamp)
        {
            this.message = message;
            this.helpKeyword = helpKeyword;
            this.senderName = senderName;
            timestamp = eventTimestamp;
            threadId = System.Threading.Thread.CurrentThread.GetHashCode();
        }

        /// <summary>
        /// The time when event was raised.
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                // Rather than storing dates in Local time all the time, we store in UTC type, and only
                // convert to Local when the user requests access to this field.  This lets us avoid the
                // expensive conversion to Local time unless it's absolutely necessary.
                if (!_localTimestamp.HasValue)
                {
                    _localTimestamp = timestamp.Kind == DateTimeKind.Utc || timestamp.Kind == DateTimeKind.Unspecified
                        ? timestamp.ToLocalTime()
                        : timestamp;
                }

                return _localTimestamp.Value;
            }
        }

        /// <summary>
        /// Exposes the private <see cref="timestamp"/> field to derived types.
        /// Used for serialization. Avoids the side effects of calling the
        /// <see cref="Timestamp"/> getter.
        /// </summary>
        protected internal DateTime RawTimestamp
        {
            get => timestamp;
            set => timestamp = value;
        }

        /// <summary>
        /// The thread that raised event.  
        /// </summary>
        public int ThreadId => threadId;

        /// <summary>
        /// Text of event. 
        /// </summary>
        public virtual string Message
        {
            get => message;
            protected set => message = value;
        }

        /// <summary>
        /// Custom help keyword associated with event.
        /// </summary>
        public string HelpKeyword => helpKeyword;

        /// <summary>
        /// Name of the object sending this event.
        /// </summary>
        public string SenderName => senderName;

        /// <summary>
        /// Event contextual information for the build event argument
        /// </summary>
        public BuildEventContext BuildEventContext
        {
            get => buildEventContext;
            set => buildEventContext = value;
        }



#region SetSerializationDefaults
        /// <summary>
        /// Run before the object has been deserialized
        /// UNDONE (Logging.)  Can this and the next function go away, and instead return a BuildEventContext.Invalid from
        /// the property if the buildEventContext field is null?
        /// </summary>
        [OnDeserializing]
        private void SetBuildEventContextDefaultBeforeSerialization(StreamingContext sc)
        {
            // Don't want to create a new one here as default all the time as that would be a lot of 
            // possibly useless allocations
            buildEventContext = null;
        }

        /// <summary>
        /// Run after the object has been deserialized
        /// </summary>
        [OnDeserialized]
        private void SetBuildEventContextDefaultAfterSerialization(StreamingContext sc)
        {
            if (buildEventContext == null)
            {
                buildEventContext = BuildEventContext.Invalid;
            }
        }
#endregion

    }
}
