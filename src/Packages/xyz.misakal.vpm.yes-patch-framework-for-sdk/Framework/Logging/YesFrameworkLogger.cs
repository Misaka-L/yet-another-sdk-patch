using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;
using YesPatchFrameworkForVRChatSdk.PatchApi.Logging;
using Object = UnityEngine.Object;

namespace YesPatchFrameworkForVRChatSdk.Logging;

internal sealed class YesFrameworkLogger : IYesLogger
{
    [MenuItem("Tools/Log Something")]
    public static void Log()
    {
        try
        {
            throw new Exception("Test");
        }
        catch (Exception e)
        {
            var rootGameObject = SceneManager.GetActiveScene().GetRootGameObjects().First();
            Instance.Log(YesLogLevel.Error, "YesFrameworkLogger", "This is a test log message.", e, rootGameObject);
        }
    }

    public static YesFrameworkLogger Instance { get; } = new();

    private readonly YesUnityDebugLogger _unityLogger = new();

    private readonly List<YesLogEntity> _logEntities = new();

    private readonly object _lock = new();

    public event EventHandler<YesLogEntity>? OnLogEntityAdded;

    public void Log(YesLogLevel level, string source, string message, Exception? exception, Object? context)
    {
        var logEntity = new YesLogEntity(level, source, message, exception, context);

        lock (_lock)
        {
            _logEntities.Add(logEntity);
            OnOnLogEntityAdded(logEntity);
        }

        _unityLogger.Log(level, source, message, exception, context);
    }

    public List<YesLogEntity> GetLogEntities()
    {
        lock (_lock)
        {
            return new List<YesLogEntity>(_logEntities);
        }
    }

    private void OnOnLogEntityAdded(YesLogEntity e)
    {
        OnLogEntityAdded?.Invoke(this, e);
    }
}