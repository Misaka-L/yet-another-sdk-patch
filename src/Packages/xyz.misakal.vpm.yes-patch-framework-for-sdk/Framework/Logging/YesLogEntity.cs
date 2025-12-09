using System;
using System.Text;
using YesPatchFrameworkForVRChatSdk.PatchApi.Logging;
using Object = UnityEngine.Object;

namespace YesPatchFrameworkForVRChatSdk.Logging;

internal class YesLogEntity
{
    public YesLogEntity(YesLogLevel level, string source, string message, Exception? exception, Object? context)
    {
        Level = level;
        Source = source;
        Message = message;
        Exception = exception;
        Context = context;

        FullMessage = GetMessage();
    }

    public YesLogLevel Level { get; }
    public string Source { get; }
    public string Message { get; }

    public Exception? Exception { get; }
    public Object? Context { get; }

    public string FullMessage { get; }

    private string GetMessage()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Message);

        if (Exception is not null)
            stringBuilder.AppendLine(Exception.ToString());

        if (Context != null)
            stringBuilder.AppendLine(Context.ToString());

        return stringBuilder.ToString();
    }
}