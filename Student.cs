using System.Text.Json.Serialization;

class Student(string name, int age) : Person(name, age)
{
  public List<double> Grades { get; set; } = [];
  public void AddGrade(params double[] grades)
  {
    foreach (var grade in grades)
      if (grade >= 2 && grade <= 6) Grades.Add(grade);
      else throw new($"Invalid grade {grade}");
  }
  public void DeleteGrade(params double[] grades)
  {
    foreach (var g in grades)
      Grades.Remove(g);
  }
  [JsonIgnore]
  public double AvgGrade { get { return Grades.Sum() / Grades.Count; } }

  public override string ToString()
  {
    return $"Students {{ id={Id} name={Name} age={Age} grades=({string.Join(", ", Grades.Select(g => $"{g:F2}"))}) }}";
  }
}
