using System.IO;
using System.Text.Json;

namespace ConsoleApp4;

public static class MapManager
{
    public static MapData LoadMap(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<MapData>(jsonString);
    }
}