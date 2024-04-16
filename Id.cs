class HasId
{
  public uint Id { get; set; } = Next();

  public static uint GetId { get; set; } = 0;
  public static uint Next() => GetId++;
}
