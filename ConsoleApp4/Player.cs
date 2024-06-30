using System;

namespace ConsoleApp4;

class Player : GameObject
{
    public Direction MoveDirection { get; set; }

    public Player(int x, int y) : base(x, y, "P")
    {
        MoveDirection = Direction.None;
    }

    public void Move(ConsoleKey key, int minX, int maxX, int minY, int maxY)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                Position.X = Math.Max(minX, Position.X - 1);
                MoveDirection = Direction.Left;
                break;
            case ConsoleKey.RightArrow:
                Position.X = Math.Min(maxX, Position.X + 1);
                MoveDirection = Direction.Right;
                break;
            case ConsoleKey.UpArrow:
                Position.Y = Math.Max(minY, Position.Y - 1);
                MoveDirection = Direction.Up;
                break;
            case ConsoleKey.DownArrow:
                Position.Y = Math.Min(maxY, Position.Y + 1);
                MoveDirection = Direction.Down;
                break;
        }
    }
}