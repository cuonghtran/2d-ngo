using UnityEngine;

namespace BasicNetcode
{
    public class Logger
    {
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogYellow(object message)
        {
            Debug.Log($"<color=yellow>{message}</color>");
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogGreen(object message)
        {
            Debug.Log($"<color=green>{message}</color>");
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogRed(object message)
        {
            Debug.Log($"<color=red>{message}</color>");
        }
    }
}
