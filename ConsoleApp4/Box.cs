namespace ConsoleApp4;

class Box : GameObject
{
    public bool IsOnGoal { get; set; }

    public Box(int x, int y) : base(x, y, "B")
    {
        IsOnGoal = false;
    }

    public void UpdateIcon()
    {
        Icon = IsOnGoal ? "O" : "B";
    }
}