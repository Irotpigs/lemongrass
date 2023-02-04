//---------------------------------------------------------------------------//
// Name        - Subsystem_Logging.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - This subsystem will redirect debug logs onto the screen.
//  Unreal Style
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief This subsystem will log all things logged with Debug onto the screen.
   */
  public class Subsystem_Logging : GameSubsystem
  {
    // Make a new log levels to map to Unity's because they refuse to behave rationally
    public enum LogLevels
    {
      None,

      Exception,
      Error,
      Warning,
      Assert,
      Log,

      All
    }

    /**
     * @brief Contains information of each log that is logged
     */
    private class LogMessage
    {
      public LogMessage(string message, float timer)
      {
        MessageText = message;
        MessageTimer = timer;
      }

      public string MessageText;
      public float MessageTimer;
    }

    private List<LogMessage> messageQueue = new List<LogMessage>();

    // Values loaded from ini file if possible
    private float messageLifetime = 5.0f;
    private LogLevels minLogLevel = LogLevels.All;

    // GameSubsystem implementation //////////////////////////////////////////////////
    protected internal override void Initialize()
    {
      // Loads all of the applicable config values
      INIParser parser = new INIParser();
      parser.OpenConfig("SubsystemLogging.ini");

      messageLifetime = (float)parser.ReadValue("SubsystemDefaults", "MessageLifetime", messageLifetime);
      minLogLevel = MapStringToLogLevel(parser.ReadValue("SubsystemDefaults", "MinLogLevel", "all"));

      parser.Close();

      Application.logMessageReceived += HandleApplicationLog;
    }

    protected internal override void Update()
    {
      // count all the log timers down
      foreach (LogMessage message in messageQueue) message.MessageTimer -= Time.deltaTime;

      messageQueue.RemoveAll((item) => item.MessageTimer <= 0);
    }

    protected internal override void Shutdown()
    {
      Application.logMessageReceived -= HandleApplicationLog;
    }

    protected internal override void OnGui()
    {
      List<string> rawMessages = new List<string>();
      foreach (LogMessage message in messageQueue) rawMessages.Add(message.MessageText);

      GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
      GUILayout.Label("\n" + string.Join("\n", rawMessages.ToArray()));
      GUILayout.EndArea();
    }

    //////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    // Unity does not structure their logs in order. Indicative of their company's management
    public static LogLevels MapLogTypeToLogLevel(LogType type)
    {
      switch (type)
      {
        case LogType.Assert: return LogLevels.Assert;
        case LogType.Exception: return LogLevels.Exception;
        case LogType.Error: return LogLevels.Error;
        case LogType.Warning: return LogLevels.Warning;
        case LogType.Log: return LogLevels.Log;
      }
      return LogLevels.None;
    }

    public static LogLevels MapStringToLogLevel(string level)
    {
      if (string.Compare(level, "assert", true) == 0) return LogLevels.Assert;
      if (string.Compare(level, "exception", true) == 0) return LogLevels.Exception;
      if (string.Compare(level, "error", true) == 0) return LogLevels.Error;
      if (string.Compare(level, "warning", true) == 0) return LogLevels.Warning;
      if (string.Compare(level, "log", true) == 0) return LogLevels.Log;
      if (string.Compare(level, "all", true) == 0) return LogLevels.All;
      return LogLevels.None;
    }

    //-protected-----------------------------------------------------------------//
    protected void HandleApplicationLog(string logString, string stackTrace, LogType type)
    {
      if (minLogLevel < MapLogTypeToLogLevel(type)) return;

      string formattedString = $"[{type}]: {logString}";
      messageQueue.Add(new LogMessage(formattedString, 5.0f));
      if (type == LogType.Exception)
      {
        messageQueue.Add(new LogMessage(stackTrace, 5.0f));
      }
    }
  }
}
