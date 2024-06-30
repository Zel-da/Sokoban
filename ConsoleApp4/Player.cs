using System;

namespace ConsoleApp4;

public class Player
{
    public Position Position { get; private set; }
    public Direction MoveDirection { get; private set; }

    public Player(int x, int y)
    {
        Position = new Position(x, y);
        MoveDirection = Direction.None;
    }

    public void Move(ConsoleKey key, int minX, int maxX, int minY, int maxY)
    {
        if (key == ConsoleKey.LeftArrow)
        {
            Position.X = Math.Max(minX, Position.X - 1);
            MoveDirection = Direction.Left;
        }
        if (key == ConsoleKey.RightArrow)
        {
            Position.X = Math.Min(maxX, Position.X + 1);
            MoveDirection = Direction.Right;
        }
        if (key == ConsoleKey.UpArrow)
        {
            Position.Y = Math.Max(minY, Position.Y - 1);
            MoveDirection = Direction.Up;
        }
        if (key == ConsoleKey.DownArrow)
        {
            Position.Y = Math.Min(maxY, Position.Y + 1);
            MoveDirection = Direction.Down;
        }
    }

    public void Render()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write("P");
    }
}