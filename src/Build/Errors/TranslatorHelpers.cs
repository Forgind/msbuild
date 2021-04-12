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
    }
}
