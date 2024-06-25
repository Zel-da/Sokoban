using System;

namespace ConsoleApp4;

internal class Program
{
    static void Main(string[] args)
    {
        Console.ResetColor();
        Console.CursorVisible = false;
        Console.Title = "Junkoban";
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Clear();

        int playerStartX = 0;
        int playerStartY = 0;
        const int minX = 0;
        const int maxX = 30;
        const int minY = 0;
        const int maxY = 30;

        Player player = new Player(playerStartX, playerStartY);
        Box[] boxes = {
            new Box(5, 5), new Box(5, 4), new Box(5, 3), new Box(5, 2), new Box(5, 1)
        };
        Wall[] walls = {
            new Wall(7, 7), new Wall(7, 8), new Wall(7, 9), new Wall(7, 10), new Wall(7, 11)
        };
        Goal[] goals = {
            new Goal(10, 10), new Goal(10, 9), new Goal(10, 8), new Goal(10, 7), new Goal(10, 6)
        };
        RandomBox randomBox = new RandomBox(13, 13);

        int pushedBoxIndex = 0;
        int randomBoxPoint = 0;
        Random random = new Random();

        while (true)
        {
            Console.Clear();

            player.Render();
            foreach (var goal in goals) goal.Render();
            foreach (var wall in walls) wall.Render();
            foreach (var box in boxes)
            {
                box.UpdateIcon();
                box.Render();
            }
            randomBox.Render();

            Console.SetCursorPosition(0, 31);
            Console.WriteLine("Point : " + randomBoxPoint);

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            player.Move(keyInfo.Key, minX, maxX, minY, maxY);

            foreach (var wall in walls)
            {
                if (!IsCollided(player.Position, wall.Position)) continue;
                PushOut(player.MoveDirection, player.Position, wall.Position);
            }

            for (int i = 0; i < boxes.Length; ++i)
            {
                if (!IsCollided(player.Position, boxes[i].Position)) continue;

                MoveBox(player.MoveDirection, boxes[i].Position, player.Position);
                pushedBoxIndex = i;
                break;
            }

            for (int i = 0; i < boxes.Length; ++i)
            {
                if (pushedBoxIndex == i) continue;
                if (!IsCollided(boxes[pushedBoxIndex].Position, boxes[i].Position)) continue;

                PushOut(player.MoveDirection, boxes[pushedBoxIndex].Position, boxes[i].Position);
                PushOut(player.MoveDirection, player.Position, boxes[pushedBoxIndex].Position);
            }

            foreach (var wall in walls)
            {
                if (!IsCollided(boxes[pushedBoxIndex].Position, wall.Position)) continue;

                PushOut(player.MoveDirection, boxes[pushedBoxIndex].Position, wall.Position);
                PushOut(player.MoveDirection, player.Position, boxes[pushedBoxIndex].Position);
                break;
            }

            int boxOnGoalCount = CountBoxOnGoal(boxes, goals);
            if (boxOnGoalCount == goals.Length) break;

            if (IsCollided(player.Position, randomBox.Position))
            {
                randomBoxPoint += GetRandomBoxPoint(random);
                randomBox.Position.X = random.Next(minX + 1, maxX);
                randomBox.Position.Y = random.Next(minY + 1, maxY);
            }
        }

        int GetRandomBoxPoint(Random random)
        {
            double chance = random.NextDouble();
            if (chance < 0.90) return 1;
            if (chance < 0.99) return 10;
            if (chance < 0.999) return 100;
            return 100000;
        }

        bool IsCollided(Position pos1, Position pos2) => pos1.X == pos2.X && pos1.Y == pos2.Y;

        void PushOut(Direction moveDirection, Position objPos, Position collidedPos)
        {
            switch (moveDirection)
            {
                case Direction.Left:
                    objPos.X = Math.Min(collidedPos.X + 1, maxX);
                    break;
                case Direction.Right:
                    objPos.X = Math.Max(collidedPos.X - 1, minX);
                    break;
                case Direction.Up:
                    objPos.Y = Math.Min(collidedPos.Y + 1, maxY);
                    break;
                case Direction.Down:
                    objPos.Y = Math.Max(collidedPos.Y - 1, minY);
                    break;
            }
        }

        void MoveBox(Direction moveDirection, Position boxPos, Position playerPos)
        {
            switch (moveDirection)
            {
                case Direction.Left:
                    boxPos.X = Math.Max(playerPos.X - 1, minX);
                    break;
                case Direction.Right:
                    boxPos.X = Math.Min(playerPos.X + 1, maxX);
                    break;
                case Direction.Up:
                    boxPos.Y = Math.Max(playerPos.Y - 1, minY);
                    break;
                case Direction.Down:
                    boxPos.Y = Math.Min(playerPos.Y + 1, maxY);
                    break;
            }
        }

        int CountBoxOnGoal(Box[] boxes, Goal[] goals)
        {
            int result = 0;
            foreach (var box in boxes)
            {
                box.IsOnGoal = false;
                foreach (var goal in goals)
                {
                    if (IsCollided(box.Position, goal.Position))
                    {
                        result++;
                        box.IsOnGoal = true;
                        break;
                    }
                }
            }
            return result;
        }
    }
}