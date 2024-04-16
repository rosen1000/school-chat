class Person(string name, int age) : HasId, IPersonLite
{
  public string Name { get; set; } = name;
  public int Age { get; set; } = age;

  public IPersonLite GetLite() {
    return this;
  }

  public void SendMessage(Chat chat, IPersonLite recepient, string message)
  {
    chat.LogMessage(this, recepient, message);
  }
}

interface IPersonLite
{
  uint Id { get; set; }
  string Name { get; set; }
}
