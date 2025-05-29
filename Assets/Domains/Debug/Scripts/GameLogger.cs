using System;
using System.IO;
using UnityEngine;

namespace Domains.Debug
{
    public class GameLogger : MonoBehaviour
    {
        private string logFilePath;
        public static GameLogger Instance { get; private set; }

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

            // Create log file in persistentDataPath
            logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

            // Log startup and clear previous log
            File.WriteAllText(logFilePath, $"=== Log Started {DateTime.Now} ===\n");
            UnityEngine.Debug.Log($"Logging to: {logFilePath}");

            // Subscribe to log events
            Application.logMessageReceived += LogCallback;
        }

        private void OnDestroy()
        {
            // Clean up event subscription
            Application.logMessageReceived -= LogCallback;
        }

        private void LogCallback(string condition, string stacktrace, LogType type)
        {
            try
            {
                // Format the log message
                var formattedMessage = $"[{DateTime.Now}] [{type}] {condition}";
                if (type == LogType.Error || type == LogType.Exception)
                    formattedMessage += $"\n{stacktrace}";

                // Append to file
                File.AppendAllText(logFilePath, formattedMessage + "\n");
            }
            catch (Exception)
            {
                // Avoid recursion in case of file write errors
            }
        }

        // Method for direct logging
        public void Log(string message)
        {
            try
            {
                var formattedMessage = $"[{DateTime.Now}] [CUSTOM] {message}";
                File.AppendAllText(logFilePath, formattedMessage + "\n");
            }
            catch (Exception)
            {
                // Avoid recursion in case of file write errors
            }
        }
    }
}