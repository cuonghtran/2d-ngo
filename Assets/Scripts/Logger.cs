using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicNetcode
{
    public class Logger
    {
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void LogYellow(object message)
        {
            Debug.Log($"<color=yellow>{message}</color>");
        }

        public static void LogGreen(object message)
        {
            Debug.Log($"<color=green>{message}</color>");
        }

        public static void LogRed(object message)
        {
            Debug.Log($"<color=red>{message}</color>");
        }
    }
}
