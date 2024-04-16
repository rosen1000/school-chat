class ArgsHelper(int min = -1, int max = -1, int exact = -1)
{
  private readonly int min = min;
  private readonly int max = max;
  private readonly int exact = exact;

  public bool Validate(string text)
  {
    string[] args = text.Split(' ');
    if (min != -1 && args.Length < min) return false;
    if (max != -1 && args.Length > max) return false;
    if (exact != -1 && args.Length != exact) return false;
    return true;
  }
}