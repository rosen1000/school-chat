using System.Text;
using System.Text.Json;

class Persistance
{
  public static string path = "./chat.json";
  private static readonly JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
  private static readonly Encoding encoding = Encoding.UTF8;

  public static void Save<T>(T obj)
  {
    string json = JsonSerializer.Serialize(obj, options);
    File.WriteAllText(path, json, encoding);
  }

  public static T? Load<T>()
  {
    try
    {
      using Stream stream = File.OpenRead(path);
      return JsonSerializer.Deserialize<T>(stream, options);
    }
    catch (Exception e)
    {
      Printer.Warnln(e.Message);
      return default;
    }
  }

}
