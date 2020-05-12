
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Eventing;
using Microsoft.Build.UnitTests.Shared;
using Shouldly;
using System.Diagnostics.Tracing;
using Xunit;
using Xunit.Abstractions;

namespace MSBuild.UnitTests
{
    public sealed class EventSource_Tests
    {
        private class CallEventSource : EventListener
        {
            public ITestOutputHelper logger;
            public CallEventSource(ITestOutputHelper l)
            {
                logger = l;
            }

            protected override void OnEventWritten(EventWrittenEventArgs eventData)
            {
                logger.WriteLine(eventData.Message);
            }
        }

        [Fact]
        public void EventSource()
        {
            ITestOutputHelper helper = new Xunit.Sdk.TestOutputHelper();
            CallEventSource listener = new CallEventSource(helper);
            listener.EnableEvents(MSBuildEventSource.Log, EventLevel.Verbose);
            RunnerUtilities.ExecMSBuild(@"C:\Users\namytelk\Desktop\Temp\ThreeProject\SimpleProject\SimpleProject.sln", out bool success, helper);
            success.ShouldBeTrue();
        }
    }
}
