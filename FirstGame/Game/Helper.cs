using System;

namespace FirstGame.Game;

public static class Helper
{
    private static bool NoSpam { get; set; } = true;

    public static void Log(string toLog){
        if (NoSpam)
            NoSpamLog(toLog);
        else
            Console.WriteLine(toLog);
    }

    static string lastString = "";
    public static void NoSpamLog(string toLog) {
        if (lastString != toLog){
            Console.WriteLine(toLog);
            lastString = toLog;
        }
    }
}