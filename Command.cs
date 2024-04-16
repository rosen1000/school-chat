class Command(string name, Action<Lexer> exec, string[]? alias = null, ArgsHelper? args = null, string help = "", string usage = "")
{
  public string name = name;
  public Action<Lexer> exec = (lexer) =>
  {
    if (args != null && !args.Validate(lexer.text)) throw new ArgumentOutOfRangeException();
    exec(lexer);
  };
  public ArgsHelper? args = args;
  public string help = help;
  public string[]? alias = alias;
  public string usage = usage;

  public static List<Command> commands = [];
  public static void Register(Command command) => commands.Add(command);
  public static Command? Find(string name)
  {
    // Search by command name
    foreach (var cmd in commands)
    {
      if (cmd.name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) return cmd;
    }
    // Then search by alias
    foreach (var cmd in commands)
    {
      if (cmd.alias != null)
      {
        foreach (var alias in cmd.alias)
        {
          if (alias.Equals(name, StringComparison.CurrentCultureIgnoreCase)) return cmd;
        }
      }
    }
    return null;
  }
  public static void EnableHelp()
  {
    commands.Add(new("Help", (lexer) =>
    {
      string name = lexer.ReadWord();
      Command? cmd = Find(name);
      if (cmd != null)
      {
        Printer.Logln($"{(cmd.usage == "" ? cmd.name : cmd.usage)}: {cmd.help}");
        if (cmd.alias != null) Printer.Logln($"Aliases: {string.Join(", ", cmd.alias)}");
      }
      else
      {
        foreach (var cmd_ in commands)
        {
          Printer.Logln($"{(cmd_.usage == "" ? cmd_.name : cmd_.usage)}{(cmd_.help == "" ? "" : $": {cmd_.help}")}");
        }
      }
    }, help: "Prints help of all commands", usage: "Help <command>"));
  }
}
