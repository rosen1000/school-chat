class Lexer(string text, bool toLower = false)
{
  public readonly string text = text;
  private readonly bool toLower = toLower;
  private int position = 0;

  public string Read(int n)
  {
    if (position + n > text.Length) n = text.Length - position;
    string output = text[position..(position + n)];
    position += n;
    if (toLower) return output.ToLower();
    return output;
  }

  // Returns true if there were spaces removed
  public bool ConsumeWhiteSpace()
  {
    bool output = false;
    try
    {
      while (char.IsWhiteSpace(text[position]))
      {
        output = true;
        position++;
      }

    }
    catch (Exception) { return output; }
    return output;
  }

  public string ReadWord(bool consumeWSAfter = false)
  {
    string output = "";
    int n = position;
    while (n < text.Length && !char.IsWhiteSpace(text[n]))
    {
      output += text[n++];
      position++;
    }
    if (consumeWSAfter) ConsumeWhiteSpace();
    if (toLower) return output.ToLower();
    return output;
  }

  public int ReadInt(bool consumeWSAfter = false)
  {
    string output = "";
    while (position < text.Length && !char.IsWhiteSpace(text[position]))
    {
      output += text[position++];
    }
    if (consumeWSAfter) ConsumeWhiteSpace();
    return int.Parse(output);
  }

  public string ReadAll()
  {
    string output = text[position..];
    position = text.Length;
    return output;
  }

  public Lexer HardCut() => new(text[position..text.Length]);

  public void Reset() => position = 0;
}