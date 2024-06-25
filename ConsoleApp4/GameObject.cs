using System;

namespace ConsoleApp4;

class GameObject
{
    public Position Position { get; set; }
    public string Icon { get; set; }

    public GameObject(int x, int y, string icon)
    {
        Position = new Position(x, y);
        Icon = icon;
    }

    public virtual void Render()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(Icon);
    }
}