using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameCode;

public static class Logger
{
    public static List<LogEntry> History { get; set; } = new List<LogEntry>();

    public static void Log(string txt, Color? color = null)
    {
        History.Add(new LogEntry
        {
            Text = txt,
            Color = color ?? Color.White
        });
    }
}

public class LogEntry
{
    public string Text { get; set; }
    public Color Color { get; set; }
}