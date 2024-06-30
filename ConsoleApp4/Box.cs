using System;

namespace ConsoleApp4;

public class Box
{
    public Position Position { get; private set; }
    public bool IsOnGoal { get; set; }
    private string Icon => IsOnGoal ? "O" : "B";

    public Box(int x, int y)
    {
        Position = new Position(x, y);
        IsOnGoal = false;
    }

    public void UpdateIcon()
    {
        IsOnGoal = false;
    }

    public void Render()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(Icon);
    }
}