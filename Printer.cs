class Printer
{
  public static void Logln(string text) => Log(text + '\n');
  public static void Log(string text) => Console.Write(text);
  public static void Infoln(string text) => Info(text + '\n');
  public static void Info(string text)
  {
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write(text);
    Console.ResetColor();
  }
  public static void Warnln(string text) => Warn(text + '\n');
  public static void Warn(string text)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write($"WARN: {text}");
    Console.ResetColor();
  }
  public static void Errorln(string text) => Error(text + '\n');
  public static void Error(string text)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"ERROR: {text}");
    Console.ResetColor();
  }
}