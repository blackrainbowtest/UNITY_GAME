using UnityEngine;

/// <summary>
/// Global debug logger with a simple enable switch.
/// </summary>
public static class UDADebug
{
    public static bool Enabled = false;

    public static void Log(object message)
    {
        if (Enabled)
            UnityEngine.Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        if (Enabled)
            UnityEngine.Debug.Log(message, context);
    }
}

