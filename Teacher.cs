class Teacher(string name, int age) : Person(name, age)
{
  public List<IPersonLite> Students { get; set; } = [];

  public void Broadcast(Chat chat, string message)
  {
    foreach (var stud in Students)
    {
      SendMessage(chat, stud, message);
    }
  }

  public override string ToString()
  {
    return $"Teacher {{ id={Id} name={Name} age={Age} students=({string.Join(", ", Students.Select(s => s.Name))}) }}";
  }
}
