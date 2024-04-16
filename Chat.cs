class Chat
{
  public List<uint> Participants { get; set; } = [];
  public List<ChatLog> Logs { get; set; } = [];
  public void Join(Person chatable) => Participants.Add(chatable.Id);
  public void Leave(Person chatable) => Participants.Remove(chatable.Id);
  public void LogMessage(IPersonLite from, IPersonLite to, string message) => LogMessage(new(from, to, message));
  public void LogMessage(IPersonLite from, uint to, string message)
  {
    IPersonLite? target = App.state.Students.Find(p => p.Id == to);
    if (target == null) target = App.state.Teachers.Find(p => p.Id == to);
    if (target == null) throw new("Person not found!");
    LogMessage(new(from, target, message));
  }
  public void LogMessage(ChatLog log)
  {
    bool found1 = false;
    bool found2 = false;
    foreach (var id in Participants)
    {
      if (!found1 && id == log.From.Id) found1 = true;
      if (!found2 && id == log.To.Id) found2 = true;
    }
    if (!found1) throw new Exception("Sender is not in the chat");
    if (!found2) throw new Exception("Receiver is not in the chat");
    Logs.Add(log);
  }
}

class ChatLog(IPersonLite from, IPersonLite to, string message)
{
  public IPersonLite From { get; set; } = from;
  public IPersonLite To { get; set; } = to;
  public string Message { get; set; } = message;
  public DateTime Created { get; set; } = DateTime.Now;
  public bool IsRead { get; set; } = false;

  public override string ToString()
  {
    string timeFrame;
    if (Created.Date == DateTime.Today)
      timeFrame = $"{Created.Hour}:{Created.Minute}";
    else
      timeFrame = $"{Created.Day}/{Created.Month}/{Created.Year} {Created.Hour}:{Created.Minute}";

    return $"[{timeFrame}] {From.Name} to {To.Name} {(IsRead ? "👁️ " : "🙈")}:\n{Message}";
  }
}
