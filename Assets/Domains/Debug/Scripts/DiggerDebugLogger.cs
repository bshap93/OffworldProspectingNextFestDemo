using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Domains.Debug
{
    public class DiggerDebugLogger : MonoBehaviour
    {
        [Header("Settings")] public bool enableLogging = true;

        public string logFileName = "digger_debug.log";
        private readonly float flushInterval = 5.0f; // Flush to disk every 5 seconds
        private readonly StringBuilder logBuffer = new();
        private readonly int maxBufferSize = 100; // Max entries before forced flush
        private int bufferCount;
        private bool isInitialized;
        private float lastFlushTime;

        private string logFilePath;
        public static DiggerDebugLogger Instance { get; private set; }

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeLogger();
        }

        private void Update()
        {
            if (!enableLogging || !isInitialized) return;

            // Flush buffer periodically or if too many entries
            if (Time.time - lastFlushTime > flushInterval || bufferCount >= maxBufferSize)
            {
                FlushBuffer();
                lastFlushTime = Time.time;
            }
        }

        private void OnDestroy()
        {
            // Ensure we flush any remaining logs when destroyed
            if (isInitialized)
            {
                LogMessage("DiggerDebugLogger shutting down.");
                FlushBuffer();
            }
        }

        private void OnApplicationQuit()
        {
            // Ensure we flush any remaining logs when application quits
            if (isInitialized)
            {
                LogMessage("Application shutting down.");
                FlushBuffer();
            }
        }

        private void InitializeLogger()
        {
            try
            {
                logFilePath = Path.Combine(Application.persistentDataPath, logFileName);

                // Create a new log file or append to existing
                if (!File.Exists(logFilePath))
                    using (var writer = File.CreateText(logFilePath))
                    {
                        writer.WriteLine($"=== Digger Debug Log Started at {DateTime.Now} ===");
                        writer.Flush();
                    }
                else
                    // Add a separator if appending to existing log
                    using (var writer = File.AppendText(logFilePath))
                    {
                        writer.WriteLine($"\n=== New Session Started at {DateTime.Now} ===");
                        writer.Flush();
                    }

                isInitialized = true;
                LogMessage("DiggerDebugLogger initialized.");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to initialize DiggerDebugLogger: {ex.Message}");
                enableLogging = false;
            }
        }

        public void LogMessage(string message)
        {
            if (!enableLogging || !isInitialized) return;

            try
            {
                var formattedMessage = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";

                // Add to buffer
                logBuffer.AppendLine(formattedMessage);
                bufferCount++;

                // Also log to Unity console if in editor
// #if UNITY_EDITOR
//                 UnityEngine.Debug.Log($"[DiggerDebug] {message}");
// #endif
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error logging message: {ex.Message}");
            }
        }

        public void LogError(string message, Exception ex = null)
        {
            if (!enableLogging || !isInitialized) return;

            try
            {
                var formattedMessage = $"[{DateTime.Now:HH:mm:ss.fff}] [ERROR] {message}";
                if (ex != null) formattedMessage += $"\n{ex.Message}\n{ex.StackTrace}";

                // Add to buffer
                logBuffer.AppendLine(formattedMessage);
                bufferCount++;

                // Force flush on errors
                FlushBuffer();

                // Also log to Unity console
                UnityEngine.Debug.LogError($"[DiggerDebug] {message}");
            }
            catch (Exception logEx)
            {
                UnityEngine.Debug.LogError($"Error logging error: {logEx.Message}");
            }
        }

        public void LogVector3(string prefix, Vector3 vector)
        {
            LogMessage($"{prefix}: ({vector.x}, {vector.y}, {vector.z})");
        }

        public void LogDigOperation(Vector3 position, int textureIndex, float opacity, float radius, float height)
        {
            LogMessage($"Dig Op: pos=({position.x}, {position.y}, {position.z}), tex={textureIndex}, " +
                       $"opacity={opacity}, radius={radius}, height={height}");
        }

        private void FlushBuffer()
        {
            if (logBuffer.Length == 0) return;

            try
            {
                using (var writer = File.AppendText(logFilePath))
                {
                    writer.Write(logBuffer.ToString());
                    writer.Flush();
                }

                // Clear buffer
                logBuffer.Clear();
                bufferCount = 0;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to flush log buffer: {ex.Message}");
            }
        }
    }
}