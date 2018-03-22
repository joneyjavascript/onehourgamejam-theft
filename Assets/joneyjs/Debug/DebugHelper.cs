using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace joneyjs.EasyDebug
{
    public enum LogType { info };

    public class EasyDebugHelper : MonoBehaviour
    {
        public static bool enabled = true;
        
        public static void log(string data, LogType logType = LogType.info) {
            if (enabled)
            {
                Debug.Log(logType + " - " + data);
            }
        }

        public static void enableEasyDebug() {
            enabled = true;
        }

        public static void disableEasyDebug()
        {
            Debug.LogWarning("Easy Debug is Disabled");
            enabled = false;
        }

    }

    
}