using Microsoft.Build.BackEnd;
using Microsoft.Build.Exceptions;
using Microsoft.Build.Framework;
using System;
using System.Globalization;

namespace Microsoft.Build.Errors
{
    internal static class TranslatorHelpers
    {
        internal static void Translate(ITranslator translator, ref InvalidProjectFileException e)
        {
            string message = e?.BaseMessage;
            translator.Translate(ref message);
            e ??= new(message);
            translator.Translate(ref e.file);
            translator.Translate(ref e.lineNumber);
            translator.Translate(ref e.columnNumber);
            translator.Translate(ref e.endLineNumber);
            translator.Translate(ref e.endColumnNumber);
            translator.Translate(ref e.errorSubcategory);
            translator.Translate(ref e.errorCode);
            translator.Translate(ref e.helpKeyword);
            translator.Translate(ref e.hasBeenLogged);
        }

        internal static void Translate(ITranslator translator, ref InternalLoggerException e)
        {
            string message = e?.Message;
            translator.Translate(ref message);
            e ??= new(message);
            translator.Translate(ref e.e);
            translator.Translate(ref e.errorCode);
            translator.Translate(ref e.helpKeyword);
            translator.Translate(ref e.initializationException);
        }

        internal static void Translate(this ITranslator translator, ref BuildEventArgs bea)
        {
            if (translator.TranslateNullable(bea))
            {
                translator.Translate(ref bea.message);
                translator.Translate(ref bea.helpKeyword);
                translator.Translate(ref bea.senderName);
                translator.Translate(ref bea.timestamp);
                translator.Translate(ref bea.threadId);
                translator.Translate(ref bea.buildEventContext);

                if (bea is LazyFormattedBuildEventArgs lfbea)
                {
                    if (translator.TranslateNullable(lfbea.arguments))
                    {
                        int count = lfbea.arguments.Length;
                        translator.Translate(ref count);
                        if (translator.Mode == TranslationDirection.ReadFromStream)
                        {
                            lfbea.arguments = new object[count];
                            for (int i = 0; i < count; i++)
                            {
                                string arg = string.Empty;
                                translator.Translate(ref arg);
                                lfbea.arguments[i] = arg;
                            }
                        }
                        else
                        {
                            foreach (object arg in lfbea.arguments)
                            {
                                string argString = Convert.ToString(arg, CultureInfo.CurrentCulture);
                                translator.Translate(ref argString);
                            }
                        }
                    }

                    translator.Translate(ref lfbea.originalCultureName);
                }

                if (bea is BuildErrorEventArgs beea)
                {
                    translator.TranslateNullableString(ref beea.subcategory);
                    translator.TranslateNullableString(ref beea.code);
                    translator.TranslateNullableString(ref beea.file);
                    translator.TranslateNullableString(ref beea.projectFile);
                    translator.TranslateNullableString(ref beea.helpLink);

                    translator.Translate(ref beea.lineNumber);
                    translator.Translate(ref beea.columnNumber);
                    translator.Translate(ref beea.endLineNumber);
                    translator.Translate(ref beea.endColumnNumber);
                }

                if (bea is BuildFinishedEventArgs bfea)
                {
                    translator.Translate(ref bfea.succeeded);
                }

                if (bea is BuildMessageEventArgs bmea)
                {

                    translator.TranslateNullableString(ref bmea.subcategory);
                    translator.TranslateNullableString(ref bmea.code);
                    translator.TranslateNullableString(ref bmea.file);
                    translator.TranslateNullableString(ref bmea.projectFile);

                    int importance = (Int32)bmea.importance;
                    translator.Translate(ref importance);
                    bmea.importance = (MessageImportance)importance;
                    translator.Translate(ref bmea.lineNumber);
                    translator.Translate(ref bmea.columnNumber);
                    translator.Translate(ref bmea.endLineNumber);
                    translator.Translate(ref bmea.endColumnNumber);
                }

                if (bea is BuildWarningEventArgs bwea)
                {
                    translator.TranslateNullableString(ref bwea.subcategory);
                    translator.TranslateNullableString(ref bwea.code);
                    translator.TranslateNullableString(ref bwea.file);
                    translator.TranslateNullableString(ref bwea.projectFile);
                    translator.TranslateNullableString(ref bwea.helpLink);

                    translator.Translate(ref bwea.lineNumber);
                    translator.Translate(ref bwea.columnNumber);
                    translator.Translate(ref bwea.endLineNumber);
                    translator.Translate(ref bwea.endColumnNumber);
                }

                if (bea is ProjectFinishedEventArgs pfea)
                {
                    translator.Translate(ref pfea.succeeded);
                    translator.TranslateNullableString(ref pfea.projectFile);
                }

                if (bea is TargetFinishedEventArgs tfea)
                {
                    translator.TranslateNullableString(ref tfea.projectFile);
                    translator.TranslateNullableString(ref tfea.targetFile);
                    translator.TranslateNullableString(ref tfea.targetName);

                    translator.Translate(ref tfea.succeeded);
                }

                if (bea is TargetStartedEventArgs tsea)
                {
                    translator.TranslateNullableString(ref tsea.targetName);
                    translator.TranslateNullableString(ref tsea.projectFile);
                    translator.TranslateNullableString(ref tsea.targetFile);
                    translator.TranslateNullableString(ref tsea.parentTarget);

                    int reason = (int)tsea.buildReason;
                    translator.Translate(ref reason);
                    tsea.buildReason = (TargetBuiltReason)reason;
                }

                if (bea is ProjectStartedEventArgs psea)
                {
                    translator.Translate(ref psea.projectId);

                    if (translator.TranslateNullable(psea.parentProjectBuildEventContext))
                    {

                    }
                    writer.Write((byte)1);
                    writer.Write((Int32)parentProjectBuildEventContext.NodeId);
                    writer.Write((Int32)parentProjectBuildEventContext.ProjectContextId);
                    writer.Write((Int32)parentProjectBuildEventContext.TargetId);
                    writer.Write((Int32)parentProjectBuildEventContext.TaskId);
                    writer.Write((Int32)parentProjectBuildEventContext.SubmissionId);
                    writer.Write((Int32)parentProjectBuildEventContext.ProjectInstanceId);
                }

                translator.TranslateNullableString(ref psea.projectFile);

                // TargetNames cannot be null as per the constructor
                writer.Write(targetNames);

                // If no properties were added to the property list 
                // then we have nothing to create when it is deserialized
                // This can happen if properties is null or if none of the 
                // five properties were found in the property object.
                if (properties == null)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    var validProperties = properties.Cast<DictionaryEntry>().Where(entry => entry.Key != null && entry.Value != null);
                    // ReSharper disable once PossibleMultipleEnumeration - We need to get the count of non-null first
                    var propertyCount = validProperties.Count();

                    writer.Write((byte)1);
                    writer.Write(propertyCount);

                    // Write the actual property name value pairs into the stream
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var propertyPair in validProperties)
                    {
                        writer.Write((string)propertyPair.Key);
                        writer.Write((string)propertyPair.Value);
                    }
                }
            }
        }

        private static void TranslateNullableString(this ITranslator translator, ref string str)
        {
            if (translator.TranslateNullable(str))
            {
                translator.Translate(ref str);
            }
        }

        private static void TranslateNullableInt(this ITranslator translator, ref int i)
        {
            if (translator.TranslateNullable(i))
            {
                translator.Translate(ref i);
            }
        }
    }
}
