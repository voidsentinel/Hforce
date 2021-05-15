/// <summary>
/// A Simple logger with depth added.
///Based on the simpleLogger from Heiswayi Nrird
/// </summary>
public class Logger
{

    private const string FILE_EXT = ".log";
    private const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    private static object fileLock = new object();
    private static string logFilename;
    private static int _level = 0;

    /// <summary>
    /// Initialise the Logger. Should be put in the Main method in the first few lines, so that the fiel is created
    /// </summary>
    public static void initialisation()
    {
        logFilename = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + FILE_EXT;
        // Log file header line
        string logHeader = logFilename + " is created.";

        if (!System.IO.File.Exists(logFilename))
        {
            System.IO.File.Delete(logFilename);
        }
        WriteLine(System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader);
    }

    /// <summary>
    /// Add one level to the log indentation
    /// </summary>
    public static void Push()
    {
        _level = _level + 1;
    }

    /// <summary>
    /// Remove one level to the log indentation
    /// </summary>
    public static void Pop()
    {
        _level = _level - 1;
        if (_level < 0)
        {
            _level = 0;
        }
    }

    /// <summary>
    /// Reset the log indentation to level 0 (no indentation)
    /// </summary>
    public static void Reset()
    {
        _level = 0;
    }


    /// <summary>
    /// Log a DEBUG message
    /// </summary>
    /// <param name="text">Message log log with level DEBUG</param>
    public static void Debug(string text)
    {
        WriteFormattedLog(LogLevel.DEBUG, text);
    }

    /// <summary>
    /// Log a DEBUG message
    /// </summary>
    /// <param name="text">Message log with level DEBUG</param>
    /// <param name="action">Perform the given action after logging the Message</param>
    public static void Debug(string text, LogAction action)
    {
        WriteFormattedLog(LogLevel.DEBUG, text);
        PerformAction(action);
    }

    /// <summary>
    /// Log an ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    public static void Error(string text)
    {
        WriteFormattedLog(LogLevel.ERROR, text);
    }

    /// <summary>
    /// Log an ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    /// <param name="action">Perform the given action after logging the Message</param>
    public static void Error(string text, LogAction action)
    {
        WriteFormattedLog(LogLevel.ERROR, text);
        PerformAction(action);
    }


    /// <summary>
    /// Log a FATAL ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    public static void Fatal(string text)
    {
        WriteFormattedLog(LogLevel.FATAL, text);
    }

    /// <summary>
    /// Log a FATAL ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    /// <param name="action">Perform the given action after logging the Message</param>
    public static void Fatal(string text, LogAction action)
    {
        WriteFormattedLog(LogLevel.FATAL, text);
        PerformAction(action);
    }

    /// <summary>
    /// Log an INFO message
    /// </summary>
    /// <param name="text">Message</param>
    public static void Info(string text)
    {
        WriteFormattedLog(LogLevel.INFO, text);
    }

    /// <summary>
    /// Log an INFO message
    /// </summary>
    /// <param name="text">Message</param>
    /// <param name="action">Perform the given action after logging the Message</param>
    public static void Info(string text, LogAction action)
    {
        WriteFormattedLog(LogLevel.INFO, text);
        PerformAction(action);
    }

    /// <summary>
    /// Log a TRACE message
    /// </summary>
    /// <param name="text">Message</param>
    public static void Trace(string text)
    {
        WriteFormattedLog(LogLevel.TRACE, text);
    }

    /// <summary>
    /// Log a TRACE message
    /// </summary>
    /// <param name="text">Message</param>
    /// <param name="action">Perform the given action after logging the Message</param>
    public static void Trace(string text, LogAction action)
    {
        WriteFormattedLog(LogLevel.TRACE, text);
        PerformAction(action);
    }

    /// <summary>
    /// Log a WARNING message
    /// </summary>
    /// <param name="text">Message</param>
    public static void Warning(string text)
    {
        WriteFormattedLog(LogLevel.WARNING, text);
    }

    /// <summary>
    /// Log a WARNING message
    /// </summary>
    /// <param name="text">Message</param>
    /// <param name="action">Perform the given action after logging the Message</param>
    public static void Warning(string text, LogAction action)
    {
        WriteFormattedLog(LogLevel.WARNING, text);
        PerformAction(action);
    }

    private static void WriteLine(string text, bool append = false)
    {
        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            lock (fileLock)
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilename, append, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(text);
                }
            }
        }
        catch
        {
            throw;
        }
    }

    private static void WriteFormattedLog(LogLevel level, string text)
    {
        // create the pre-text
        string pretext;
        switch (level)
        {
            case LogLevel.TRACE:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [TRACE]   ";
                break;
            case LogLevel.INFO:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [INFO]    ";
                break;
            case LogLevel.DEBUG:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
                break;
            case LogLevel.WARNING:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [WARNING] ";
                break;
            case LogLevel.ERROR:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
                break;
            case LogLevel.FATAL:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [FATAL]   ";
                break;
            default:
                pretext = "";
                break;
        }
        // add the indenttaion level after the pretext
        if (_level > 0)
        {
            pretext = pretext + new string(' ', 3 * _level);
        }
        // write the pretext & the text
        WriteLine(pretext + text, true);
    }


    private static void PerformAction(LogAction action)
    {
        if (action == LogAction.PUSH)
        {
            Push();

        }
        if (action == LogAction.POP)
        {
            Pop();
        }
    }

    [System.Flags]
    private enum LogLevel
    {
        TRACE,
        INFO,
        DEBUG,
        WARNING,
        ERROR,
        FATAL
    }

    public enum LogAction
    {
        PUSH, POP
    }
}