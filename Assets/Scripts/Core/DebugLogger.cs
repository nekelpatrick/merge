using System.Diagnostics;
using UnityEngine;

namespace ShieldWall.Core
{
    /// <summary>
    /// MOB-065: Conditional debug logging that is stripped in release builds
    /// Use this instead of Debug.Log to avoid log overhead in production
    /// </summary>
    public static class DebugLogger
    {
        /// <summary>
        /// Log message - only active in Editor or Development builds
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// Log message with context object - only active in Editor or Development builds
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message, Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        /// <summary>
        /// Log warning - only active in Editor or Development builds
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        /// <summary>
        /// Log warning with context - only active in Editor or Development builds
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(string message, Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        /// <summary>
        /// Log error - ALWAYS active (errors should always be logged)
        /// </summary>
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// Log error with context - ALWAYS active
        /// </summary>
        public static void LogError(string message, Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        /// <summary>
        /// Log exception - ALWAYS active
        /// </summary>
        public static void LogException(System.Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        /// <summary>
        /// Log exception with context - ALWAYS active
        /// </summary>
        public static void LogException(System.Exception exception, Object context)
        {
            UnityEngine.Debug.LogException(exception, context);
        }
    }
}

