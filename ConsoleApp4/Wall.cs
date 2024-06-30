using System;

namespace ConsoleApp4;

public class Wall
{
    public Position Position { get; private set; }

    public Wall(int x, int y)
    {
        Position = new Position(x, y);
    }

    public void Render()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write("W");
    }
}