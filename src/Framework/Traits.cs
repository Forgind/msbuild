// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;

#nullable disable

namespace Microsoft.Build.Framework
{
    /// <summary>
    ///     Represents toggleable features of the MSBuild engine
    /// </summary>
    internal class Traits
    {
        private static readonly Traits _instance = new Traits();
        public static Traits Instance
        {
            get
            {
                if (BuildEnvironmentState.s_runningTests)
                {
                    return new Traits();
                }
                return _instance;
            }
        }

        public Traits()
        {
            EscapeHatches = new EscapeHatches();

            DebugScheduler = DebugEngine || !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDEBUGSCHEDULER"));
            DebugNodeCommunication = DebugEngine || !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDEBUGCOMM"));
        }

        public EscapeHatches EscapeHatches { get; }

        internal readonly string MSBuildDisableFeaturesFromVersion = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDISABLEFEATURESFROMVERSION");

        /// <summary>
        /// Do not expand wildcards that match a certain pattern
        /// </summary>
        public readonly bool UseLazyWildCardEvaluation = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MsBuildSkipEagerWildCardEvaluationRegexes"));
        public readonly bool LogExpandedWildcards = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBUILDLOGEXPANDEDWILDCARDS"));

        /// <summary>
        /// Cache file existence for the entire process
        /// </summary>
        public readonly bool CacheFileExistence = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MsBuildCacheFileExistence"));

        public readonly bool UseSimpleProjectRootElementCacheConcurrency = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MsBuildUseSimpleProjectRootElementCacheConcurrency"));

        /// <summary>
        /// Cache wildcard expansions for the entire process
        /// </summary>
        public readonly bool MSBuildCacheFileEnumerations = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MsBuildCacheFileEnumerations"));

        public readonly bool EnableAllPropertyFunctions = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDENABLEALLPROPERTYFUNCTIONS") == "1";

        /// <summary>
        /// Enable restore first functionality in MSBuild.exe
        /// </summary>
        public readonly bool EnableRestoreFirst = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDENABLERESTOREFIRST") == "1";

        /// <summary>
        /// Allow the user to specify that two processes should not be communicating via an environment variable.
        /// </summary>
        public static readonly string MSBuildNodeHandshakeSalt = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDNODEHANDSHAKESALT");

        /// <summary>
        /// Setting the associated environment variable to 1 restores the pre-15.8 single
        /// threaded (slower) copy behavior. Zero implies Int32.MaxValue, less than zero
        /// (default) uses the empirical default in Copy.cs, greater than zero can allow
        /// perf tuning beyond the defaults chosen.
        /// </summary>
        public readonly int CopyTaskParallelism = ParseIntFromEnvironmentVariableOrDefault("MSBUILDCOPYTASKPARALLELISM", -1);

        /// <summary>
        /// Instruct MSBuild to write out the generated "metaproj" file to disk when building a solution file.
        /// </summary>
        public readonly bool EmitSolutionMetaproj = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBuildEmitSolution"));

        /// <summary>
        /// Log statistics about property functions which require reflection
        /// </summary>
        public readonly bool LogPropertyFunctionsRequiringReflection = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBuildLogPropertyFunctionsRequiringReflection"));

        /// <summary>
        /// Log property tracking information.
        /// </summary>
        public readonly int LogPropertyTracking = ParseIntFromEnvironmentVariableOrDefault("MsBuildLogPropertyTracking", 0); // Default to logging nothing via the property tracker.

        /// <summary>
        /// When evaluating items, this is the minimum number of items on the running list to use a dictionary-based remove optimization.
        /// </summary>
        public readonly int DictionaryBasedItemRemoveThreshold = ParseIntFromEnvironmentVariableOrDefault("MSBUILDDICTIONARYBASEDITEMREMOVETHRESHOLD", 100);

        public readonly bool DebugEngine = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBuildDebugEngine"));
        public readonly bool DebugScheduler;
        public readonly bool DebugNodeCommunication;

        private static int ParseIntFromEnvironmentVariableOrDefault(string environmentVariable, int defaultValue)
        {
            return int.TryParse(EnvironmentUtilities.GetEnvironmentVariable(environmentVariable), out int result)
                ? result
                : defaultValue;
        }
    }

    internal class EscapeHatches
    {
        /// <summary>
        /// Do not log command line information to build loggers. Useful to unbreak people who parse the msbuild log and who are unwilling to change their code.
        /// </summary>
        public readonly bool DoNotSendDeferredMessagesToBuildManager = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MsBuildDoNotSendDeferredMessagesToBuildManager"));

        /// <summary>
        /// https://github.com/dotnet/msbuild/pull/4975 started expanding qualified metadata in Update operations. Before they'd expand to empty strings.
        /// This escape hatch turns back the old empty string behavior.
        /// </summary>
        public readonly bool DoNotExpandQualifiedMetadataInUpdateOperation = !string.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBuildDoNotExpandQualifiedMetadataInUpdateOperation"));

        /// <summary>
        /// Force whether Project based evaluations should evaluate elements with false conditions.
        /// </summary>
        public readonly bool? EvaluateElementsWithFalseConditionInProjectEvaluation = ParseNullableBoolFromEnvironmentVariable("MSBUILDEVALUATEELEMENTSWITHFALSECONDITIONINPROJECTEVALUATION");

        /// <summary>
        /// Always use the accurate-but-slow CreateFile approach to timestamp extraction.
        /// </summary>
        public readonly bool AlwaysUseContentTimestamp = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDALWAYSCHECKCONTENTTIMESTAMP") == "1";

        /// <summary>
        /// Truncate task inputs when logging them. This can reduce memory pressure
        /// at the expense of log usefulness.
        /// </summary>
        public readonly bool TruncateTaskInputs = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDTRUNCATETASKINPUTS") == "1";

        /// <summary>
        /// Disables truncation of Condition messages in Tasks/Targets via ExpanderOptions.Truncate.
        /// </summary>
        public readonly bool DoNotTruncateConditions = EnvironmentUtilities.GetEnvironmentVariable("MSBuildDoNotTruncateConditions") == "1";

        /// <summary>
        /// Disables skipping full drive/filesystem globs that are behind a false condition.
        /// </summary>
        public readonly bool AlwaysEvaluateDangerousGlobs = EnvironmentUtilities.GetEnvironmentVariable("MSBuildAlwaysEvaluateDangerousGlobs") == "1";

        /// <summary>
        /// Disables skipping full up to date check for immutable files. See FileClassifier class.
        /// </summary>
        public readonly bool AlwaysDoImmutableFilesUpToDateCheck = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDONOTCACHEMODIFICATIONTIME") == "1";

        /// <summary>
        /// Emit events for project imports.
        /// </summary>
        private bool? _logProjectImports;

        /// <summary>
        /// Emit events for project imports.
        /// </summary>
        public bool LogProjectImports
        {
            get
            {
                // Cache the first time
                if (_logProjectImports == null)
                {
                    _logProjectImports = !String.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBUILDLOGIMPORTS"));
                }
                return _logProjectImports.Value;
            }
            set
            {
                _logProjectImports = value;
            }
        }

        private bool? _logTaskInputs;
        public bool LogTaskInputs
        {
            get
            {
                if (_logTaskInputs == null)
                {
                    _logTaskInputs = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDLOGTASKINPUTS") == "1";
                }
                return _logTaskInputs.Value;
            }
            set
            {
                _logTaskInputs = value;
            }
        }

        private bool? _logPropertiesAndItemsAfterEvaluation;
        private bool _logPropertiesAndItemsAfterEvaluationInitialized = false;
        public bool? LogPropertiesAndItemsAfterEvaluation
        {
            get
            {
                if (!_logPropertiesAndItemsAfterEvaluationInitialized)
                {
                    _logPropertiesAndItemsAfterEvaluationInitialized = true;
                    var variable = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDLOGPROPERTIESANDITEMSAFTEREVALUATION");
                    if (!string.IsNullOrEmpty(variable))
                    {
                        _logPropertiesAndItemsAfterEvaluation = variable == "1" || string.Equals(variable, "true", StringComparison.OrdinalIgnoreCase);
                    }
                }

                return _logPropertiesAndItemsAfterEvaluation;
            }

            set
            {
                _logPropertiesAndItemsAfterEvaluationInitialized = true;
                _logPropertiesAndItemsAfterEvaluation = value;
            }
        }

        /// <summary>
        /// Read information only once per file per ResolveAssemblyReference invocation.
        /// </summary>
        public readonly bool CacheAssemblyInformation = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDONOTCACHERARASSEMBLYINFORMATION") != "1";

        public readonly ProjectInstanceTranslationMode? ProjectInstanceTranslation = ComputeProjectInstanceTranslation();

        /// <summary>
        /// Never use the slow (but more accurate) CreateFile approach to timestamp extraction.
        /// </summary>
        public readonly bool UseSymlinkTimeInsteadOfTargetTime = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDUSESYMLINKTIMESTAMP") == "1";

        /// <summary>
        /// Allow node reuse of TaskHost nodes. This results in task assemblies locked past the build lifetime, preventing them from being rebuilt if custom tasks change, but may improve performance.
        /// </summary>
        public readonly bool ReuseTaskHostNodes = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDREUSETASKHOSTNODES") == "1";

        /// <summary>
        /// Whether or not to ignore imports that are considered empty.  See ProjectRootElement.IsEmptyXmlFile() for more info.
        /// </summary>
        public readonly bool IgnoreEmptyImports = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDIGNOREEMPTYIMPORTS") == "1";

        /// <summary>
        /// Whether to respect the TreatAsLocalProperty parameter on the Project tag.
        /// </summary>
        public readonly bool IgnoreTreatAsLocalProperty = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDIGNORETREATASLOCALPROPERTY") != null;

        /// <summary>
        /// Whether to write information about why we evaluate to debug output.
        /// </summary>
        public readonly bool DebugEvaluation = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDEBUGEVALUATION") != null;

        /// <summary>
        /// Whether to warn when we set a property for the first time, after it was previously used.
        /// </summary>
        public readonly bool WarnOnUninitializedProperty = !String.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBUILDWARNONUNINITIALIZEDPROPERTY"));

        /// <summary>
        /// MSBUILDUSECASESENSITIVEITEMNAMES is an escape hatch for the fix
        /// for https://github.com/dotnet/msbuild/issues/1751. It should
        /// be removed (permanently set to false) after establishing that
        /// it's unneeded (at least by the 16.0 timeframe).
        /// </summary>
        public readonly bool UseCaseSensitiveItemNames = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDUSECASESENSITIVEITEMNAMES") == "1";

        /// <summary>
        /// Disable the use of paths longer than Windows MAX_PATH limits (260 characters) when running on a long path enabled OS.
        /// </summary>
        public readonly bool DisableLongPaths = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDISABLELONGPATHS") == "1";

        /// <summary>
        /// Disable the use of any caching when resolving SDKs.
        /// </summary>
        public readonly bool DisableSdkResolutionCache = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDISABLESDKCACHE") == "1";

        /// <summary>
        /// Disable the NuGet-based SDK resolver.
        /// </summary>
        public readonly bool DisableNuGetSdkResolver = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDISABLENUGETSDKRESOLVER") == "1";

        /// <summary>
        /// Don't delete TargetPath metadata from associated files found by RAR.
        /// </summary>
        public readonly bool TargetPathForRelatedFiles = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDTARGETPATHFORRELATEDFILES") == "1";

        /// <summary>
        /// Disable AssemblyLoadContext isolation for plugins.
        /// </summary>
        public readonly bool UseSingleLoadContext = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDSINGLELOADCONTEXT") == "1";

        /// <summary>
        /// Enables the user of autorun functionality in CMD.exe on Windows which is disabled by default in MSBuild.
        /// </summary>
        public readonly bool UseAutoRunWhenLaunchingProcessUnderCmd = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDUSERAUTORUNINCMD") == "1";

        /// <summary>
        /// Disables switching codepage to UTF-8 after detection of characters that can't be represented in the current codepage.
        /// </summary>
        public readonly bool AvoidUnicodeWhenWritingToolTaskBatch = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDAVOIDUNICODE") == "1";

        /// <summary>
        /// Workaround for https://github.com/Microsoft/vstest/issues/1503.
        /// </summary>
        public readonly bool EnsureStdOutForChildNodesIsPrimaryStdout = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDENSURESTDOUTFORTASKPROCESSES") == "1";

        /// <summary>
        /// Use the original, string-only resx parsing in .NET Core scenarios.
        /// </summary>
        /// <remarks>
        /// Escape hatch for problems arising from https://github.com/dotnet/msbuild/pull/4420.
        /// </remarks>
        public readonly bool UseMinimalResxParsingInCoreScenarios = EnvironmentUtilities.GetEnvironmentVariable("MSBUILDUSEMINIMALRESX") == "1";

        private bool _sdkReferencePropertyExpansionInitialized;
        private SdkReferencePropertyExpansionMode? _sdkReferencePropertyExpansionValue;

        /// <summary>
        /// Overrides the default behavior of property expansion on evaluation of a <see cref="Framework.SdkReference"/>.
        /// </summary>
        /// <remarks>
        /// Escape hatch for problems arising from https://github.com/dotnet/msbuild/pull/5552.
        /// </remarks>
        public SdkReferencePropertyExpansionMode? SdkReferencePropertyExpansion
        {
            get
            {
                if (!_sdkReferencePropertyExpansionInitialized)
                {
                    _sdkReferencePropertyExpansionValue = ComputeSdkReferencePropertyExpansion();
                    _sdkReferencePropertyExpansionInitialized = true;
                }

                return _sdkReferencePropertyExpansionValue;
            }
        }

        private static bool? ParseNullableBoolFromEnvironmentVariable(string environmentVariable)
        {
            var value = EnvironmentUtilities.GetEnvironmentVariable(environmentVariable);

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (bool.TryParse(value, out bool result))
            {
                return result;
            }

            ThrowInternalError($"Environment variable \"{environmentVariable}\" should have values \"true\", \"false\" or undefined");

            return null;
        }

        private static ProjectInstanceTranslationMode? ComputeProjectInstanceTranslation()
        {
            var mode = EnvironmentUtilities.GetEnvironmentVariable("MSBUILD_PROJECTINSTANCE_TRANSLATION_MODE");

            if (mode == null)
            {
                return null;
            }

            if (mode.Equals("full", StringComparison.OrdinalIgnoreCase))
            {
                return ProjectInstanceTranslationMode.Full;
            }

            if (mode.Equals("partial", StringComparison.OrdinalIgnoreCase))
            {
                return ProjectInstanceTranslationMode.Partial;
            }

            ThrowInternalError($"Invalid escape hatch for project instance translation: {mode}");

            return null;
        }

        private static SdkReferencePropertyExpansionMode? ComputeSdkReferencePropertyExpansion()
        {
            var mode = EnvironmentUtilities.GetEnvironmentVariable("MSBUILD_SDKREFERENCE_PROPERTY_EXPANSION_MODE");

            if (mode == null)
            {
                return null;
            }

            // The following uses StartsWith instead of Equals to enable possible tricks like
            // the dpiAware "True/PM" trick (see https://devblogs.microsoft.com/oldnewthing/20160617-00/?p=93695)
            // in the future.

            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;

            if (mode.StartsWith("no", comparison))
            {
                return SdkReferencePropertyExpansionMode.NoExpansion;
            }

            if (mode.StartsWith("default", comparison))
            {
                return SdkReferencePropertyExpansionMode.DefaultExpand;
            }

            if (mode.StartsWith(nameof(SdkReferencePropertyExpansionMode.ExpandUnescape), comparison))
            {
                return SdkReferencePropertyExpansionMode.ExpandUnescape;
            }

            if (mode.StartsWith(nameof(SdkReferencePropertyExpansionMode.ExpandLeaveEscaped), comparison))
            {
                return SdkReferencePropertyExpansionMode.ExpandLeaveEscaped;
            }

            ThrowInternalError($"Invalid escape hatch for SdkReference property expansion: {mode}");

            return null;
        }

        public enum ProjectInstanceTranslationMode
        {
            Full,
            Partial
        }

        public enum SdkReferencePropertyExpansionMode
        {
            NoExpansion,
            DefaultExpand,
            ExpandUnescape,
            ExpandLeaveEscaped
        }

        /// <summary>
        /// Emergency escape hatch. If a customer hits a bug in the shipped product causing an internal exception,
        /// and fortuitously it happens that ignoring the VerifyThrow allows execution to continue in a reasonable way,
        /// then we can give them this undocumented environment variable as an immediate workaround.
        /// </summary>
        /// <remarks>
        /// Clone from ErrorUtilities which isn't (yet?) available in Framework.
        /// </remarks>

        private static readonly bool s_throwExceptions = String.IsNullOrEmpty(EnvironmentUtilities.GetEnvironmentVariable("MSBUILDDONOTTHROWINTERNAL"));

        /// <summary>
        /// Throws InternalErrorException.
        /// </summary>
        /// <remarks>
        /// Clone of ErrorUtilities.ThrowInternalError which isn't (yet?) available in Framework.
        /// </remarks>
        internal static void ThrowInternalError(string message)
        {
            if (s_throwExceptions)
            {
                throw new InternalErrorException(message);
            }
        }
    }
}
