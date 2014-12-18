using RdClientCx;
using System.Runtime.CompilerServices;


namespace RdClient.Shared.CxWrappers
{

    public class RdTraceException : System.Exception
    {
        public RdTraceException(string message) : base(message) { }
    }

    public static class RdTrace
    {
        public const string NoFileName = "NoFileName";
        public const uint NoLineNumber = 0;
        public const string NoFunctionName = "NoFunctionName";

        static RdTrace()
        {
            //RdClientCx.Tracer.Initialize();
        }

        private static void TraceInternal(string tag, TraceLevel traceLevel, string fileName, uint lineNumber, string functionName, string message)
        {
            //RdClientCx.Tracer.Trace(tag, traceLevel, fileName, lineNumber, functionName, message);
        }

        public static void TraceErr(
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            TraceInternal("UI", TraceLevel.Error, fileName, lineNumber, functionName, strMessage);
        }

        public static void TraceWrn(
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            TraceInternal("UI", TraceLevel.Warning, fileName, lineNumber, functionName, strMessage);
        }

        public static void TraceNrm(
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            TraceInternal("UI", TraceLevel.Normal, fileName, lineNumber, functionName, strMessage);
        }

        public static void TraceDbg(
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            TraceInternal("UI", TraceLevel.Debug, fileName, lineNumber, functionName, strMessage);
        }

        public static void TraceAbort(
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            TraceInternal("UI", TraceLevel.Critical, fileName, lineNumber, functionName, strMessage);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public static void IfFailXResultThrow(
            int iXResult,
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            if (iXResult != 0)
            {
                TraceInternal("UI", TraceLevel.Error, fileName, lineNumber, functionName, strMessage);
                throw new RdTraceException(string.Format("{0} (XResult: {1})", strMessage, iXResult));
            }
        }

        public static void IfCondThrow(
            bool fCondition,
            string strMessage,
            [CallerFilePath] string fileName = NoFileName,
            [CallerLineNumber] uint lineNumber = NoLineNumber,
            [CallerMemberName] string functionName = NoFunctionName
            )
        {
            if (fCondition)
            {
                TraceInternal("UI", TraceLevel.Error, fileName, lineNumber, functionName, strMessage);
                throw new RdTraceException(strMessage);
            }
        }
    }
}
