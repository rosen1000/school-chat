class App
{
  public static State state = new(0);
  static Person? currentPerson = null;
  const StringComparison noCase = StringComparison.CurrentCultureIgnoreCase;

  public static void Main()
  {
    InitCommands();

    var temp = Persistance.Load<State>();
    if (temp != null) state = temp;
    HasId.GetId = state.Id;

    while (true)
    {
      Printer.Info("> ");
      string? input = Console.ReadLine();
      if (input == null) Environment.Exit(0);
      Lexer inputLexer = new(input, true);
      var cmd = Command.Find(inputLexer.ReadWord(true));
      if (cmd == null)
      {
        Printer.Errorln("Command not found");
        continue;
      }
      try
      {
        cmd.exec(inputLexer.HardCut());
        state.Id = HasId.GetId;
        Persistance.Save(state);
      }
      catch (Exception e)
      {
        Printer.Errorln(e.Message);
      }
    }
  }

  private static void InitCommands()
  {
    Command.Register(new("NewTeacher", args: new(exact: 2),
      exec: (lexer) =>
      {
        state.Teachers.Add(new(lexer.ReadWord(true), lexer.ReadInt()));
        state.Chat.Join(state.Teachers.Last());
        Printer.Infoln("Done!");
      },
      usage: "NewTeacher <name> <age>",
      help: "Create new teacher"
    ));
    Command.Register(new("NewStudent", args: new(exact: 2),
      exec: (lexer) =>
      {
        state.Students.Add(new(lexer.ReadWord(true), lexer.ReadInt()));
        state.Chat.Join(state.Students.Last());
        Printer.Infoln("Done!");
      },
      usage: "NewStudent <name> <age>",
      help: "Create new student"
    ));
    Command.Register(new("Tell", alias: ["Send"], args: new(min: 2), exec: (lexer) =>
    {
      if (currentPerson == null) throw new("You are not logged in");
      var name = lexer.ReadWord(true);
      var receiver = FindPerson(name) ?? throw new("Can't find person with this name");
      currentPerson.SendMessage(state.Chat, receiver, lexer.ReadAll());
      Printer.Infoln("Message sent!");
    }, usage: "Tell <name> <message...>", help: "Send a message to a user.\nThat user can use the \"Read\" command to read the messages"));
    Command.Register(new("Login", (lexer) =>
    {
      var name = lexer.ReadWord();
      currentPerson = FindPerson(name) ?? throw new("There is no person with that name");
      Printer.Infoln($"You are logged as a {currentPerson.GetType().Name}");
    }, args: new(exact: 1), usage: "Login <name>", help: "Change the current user to the user with given name"));
    Command.Register(new("Read", (lexer) =>
    {
      if (currentPerson == null) throw new("You need to be logged in");
      string rawFirst = lexer.ReadWord().ToLower();
      bool onlyUnread = false;
      if ("yes".StartsWith(rawFirst) || "true".StartsWith(rawFirst) || rawFirst == "1") onlyUnread = true;
      var id = currentPerson.Id;
      List<ChatLog> logs = state.Chat.Logs.FindAll(log => (log.From.Id == id || log.To.Id == id) && (!onlyUnread || !log.IsRead));
      foreach (var log in logs)
      {
        Printer.Infoln(log.ToString());
        if (id != log.From.Id) log.IsRead = true;
      }
    }, usage: "Read [only unread=true]", help: "Read all messages send/received to the logged user"));
    Command.Register(new("List", (lexer) =>
    {
      static void print(List<Person> people)
      {
        foreach (var p in people) Printer.Infoln(p.ToString()!);
      }

      string search = lexer.ReadWord();
      if (search != "")
      {
        if ("teachers".StartsWith(search, noCase))
          print(state.Teachers.Cast<Person>().ToList());
        else if ("students".StartsWith(search, noCase))
          print(state.Students.Cast<Person>().ToList());
        else
        {
          List<Person> found = [];
          foreach (var t in state.Teachers)
            if (t.Name.Contains(search, noCase)) found.Add(t);
          foreach (var s in state.Students)
            if (s.Name.Contains(search, noCase)) found.Add(s);
          print(found);
        }
        return;
      }
      print(state.Teachers.Cast<Person>().ToList());
      print(state.Students.Cast<Person>().ToList());
    }, usage: "List [name]", help: "Print all users\nShow only given user if name is provided"));
    Command.Register(new("Grade", (lexer) =>
    {
      if (currentPerson == null || currentPerson.GetType().Name != "Teacher") throw new("You need to be logged in as a teacher to do this!");
      string name = lexer.ReadWord(true);
      string action = lexer.ReadWord(true).ToLower();
      List<double> grades = lexer.ReadAll().Split(" ").Select(x => double.Parse(x)).ToList();
      Person? p = FindPerson(name);//((Teacher)currentPerson).Students.Find(s => s.Name.Equals(name, noCase)) ?? throw new("Can't find this student");
      if (p == null || !((Teacher)currentPerson).Students.Exists(s => s.Id == p.Id)) throw new("Can't find this student");
      Student target = (Student)p;

      switch (action)
      {
        case "add":
          target.AddGrade([.. grades]);
          break;
        case "remove":
        case "delete":
          target.DeleteGrade([.. grades]);
          break;
        default:
          throw new("Unknown subcommand");
      }
      Printer.Infoln("Done!");
    }, args: new(min: 3), usage: "Grade <name> <add|remove> <grades...>", help: "Modify grades of a student"));
    Command.Register(new("Student", (lexer) =>
    {
      if (currentPerson == null || currentPerson.GetType().Name != "Teacher") throw new("You have to be logged in as a teacher");
      string action = lexer.ReadWord(true).ToLower();
      string name = lexer.ReadWord(true);
      Person p = FindPerson(name) ?? throw new("Student can't be found");
      if (p.GetType().Name != "Student") throw new("Student can't be found");
      Student target = (Student)p;
      switch (action)
      {
        case "add":
        case "attach":
          if (((Teacher)currentPerson).Students.Exists(s => s.Id == target.Id))
          {
            Printer.Warnln("You already have this student attached");
            return;
          }
          else
            ((Teacher)currentPerson).Students.Add(target);
          break;
        case "remove":
        case "detach":
          ((Teacher)currentPerson).Students.Remove(target);
          break;
        default:
          throw new("Unknown subcommand");
      }
      Printer.Infoln("Done!");
    }, usage: "Student <(attach,add)|(detach,remove)> <name>"));
    Command.Register(new("Broadcast", (lexer) =>
    {
      if (currentPerson == null || currentPerson.GetType().Name != "Teacher") throw new("You have to be logged in as a teacher");
      ((Teacher)currentPerson).Broadcast(state.Chat, lexer.ReadAll());
      Printer.Infoln("Done!");
    }, usage: "Broadcast <message...>", help: "Send a message to all attached students"));
    Command.Register(new("Rank", (_) =>
    {
      List<Student> toRank = state.Students;
      int i = 0;
      foreach (var student in toRank)
      {
        if (student.Grades.Count == 0) continue;
        Printer.Infoln($"{++i,-3} {student.Name,-10} {student.AvgGrade}");
      }
    }));
    Command.Register(new("WhoAmI", (_) =>
    {
      if (currentPerson == null) Printer.Warnln("You are not logged in!");
      else Printer.Infoln($"You are logged as {currentPerson}");
    }, help: "Show current logged user"));
    Command.Register(new("Logout", (_) => { }));
    Command.Register(new("Exit", (_) => Environment.Exit(0), help: "Exit from program"));
    Command.EnableHelp();
    Command.commands.Sort((c1, c2) => c1.name.CompareTo(c2.name));
  }

  public static Person? FindPerson(string name)
  {
    Predicate<Person> p;
    if (int.TryParse(name, out int id))
      p = x => x.Id == id;
    else
      p = x => x.Name.Equals(name, noCase);
    Person? person = state.Teachers.Find(p);
    person ??= state.Students.Find(p);
    return person;
  }
}

class State(uint id)
{
  public uint Id { get; set; } = id;
  public List<Teacher> Teachers { get; set; } = [];
  public List<Student> Students { get; set; } = [];
  public Chat Chat { get; set; } = new();
}
