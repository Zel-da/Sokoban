using System.Collections.Generic;

namespace ConsoleApp4;

public class MapData
{
    public Position PlayerStart { get; set; }
    public List<Position> Boxes { get; set; }
    public List<Position> Walls { get; set; }
    public List<Position> Goals { get; set; }
    public Position RandomBox { get; set; }
}