﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Build.Shared;

namespace Microsoft.Build.Framework
{
    /// <summary>
    /// Arguments for project finished events
    /// </summary>
    // WARNING: marking a type [Serializable] without implementing
    // ISerializable imposes a serialization contract -- it is a
    // promise to never change the type's fields i.e. the type is
    // immutable; adding new fields in the next version of the type
    // without following certain special FX guidelines, can break both
    // forward and backward compatibility
    [Serializable]
    public class ProjectFinishedEventArgs : BuildStatusEventArgs
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        protected ProjectFinishedEventArgs()
            : base()
        {
            // do nothing
        }

        /// <summary>
        /// This constructor allows event data to be initialized.
        /// Sender is assumed to be "MSBuild".
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="projectFile">name of the project</param>
        /// <param name="succeeded">true indicates project built successfully</param>
        public ProjectFinishedEventArgs
        (
            string message,
            string helpKeyword,
            string projectFile,
            bool succeeded
        )
            : this(message, helpKeyword, projectFile, succeeded, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// This constructor allows event data to be initialized.
        /// Sender is assumed to be "MSBuild". This constructor allows the timestamp to be set as well
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="helpKeyword">help keyword </param>
        /// <param name="projectFile">name of the project</param>
        /// <param name="succeeded">true indicates project built successfully</param>
        /// <param name="eventTimestamp">Timestamp when the event was created</param>
        public ProjectFinishedEventArgs
        (
            string message,
            string helpKeyword,
            string projectFile,
            bool succeeded,
            DateTime eventTimestamp
        )
            : base(message, helpKeyword, "MSBuild", eventTimestamp)
        {
            this.projectFile = projectFile;
            this.succeeded = succeeded;
        }

        internal string projectFile;
        internal bool succeeded;

        /// <summary>
        /// Project name
        /// </summary>
        public string ProjectFile => projectFile;

        /// <summary>
        /// True if project built successfully, false otherwise
        /// </summary>
        public bool Succeeded => succeeded;
    }
}
