// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Build.Shared
{
    internal static partial class EnvironmentUtilities
    {
        public static bool Is64BitProcess => Marshal.SizeOf<IntPtr>() == 8;

        private static bool DoNotUseEnvironmentVariables = Environment.GetEnvironmentVariable("MSBUILDDONOTUSEENVIRONMENTVARIABLES") == "1";
        internal static Dictionary<string, string> EnvironmentVariablesUsed { get; } = new() { { "MSBUILDDONOTUSEENVIRONMENTVARIABLES", Environment.GetEnvironmentVariable("MSBUILDDONOTUSEENVIRONMENTVARIABLES") } };

        public static bool Is64BitOperatingSystem =>
#if FEATURE_64BIT_ENVIRONMENT_QUERY
            Environment.Is64BitOperatingSystem;
#else
            RuntimeInformation.OSArchitecture == Architecture.Arm64 ||
            RuntimeInformation.OSArchitecture == Architecture.X64;
#endif
        public static string? GetEnvironmentVariable(string name)
        {
            if (DoNotUseEnvironmentVariables)
            {
                return null;
            }
            string? value = Environment.GetEnvironmentVariable(name);
            EnvironmentVariablesUsed[name] = value;
            return value;
        }

        public static string? GetEnvironmentVariable(ReadOnlySpan<char> name)
        {
            if (DoNotUseEnvironmentVariables)
            {
                return null;
            }

            string stringForm = name.ToString();
            string? value = Environment.GetEnvironmentVariable(stringForm);
            EnvironmentVariablesUsed[stringForm] = value;
            return value;
        }
    }
}
