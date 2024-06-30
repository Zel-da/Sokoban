using System;

namespace ConsoleApp4;

public class RandomBox
{
    public Position Position { get; private set; }

    public RandomBox(int x, int y)
    {
        Position = new Position(x, y);
    }

    public void Render()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write("R");
    }
}