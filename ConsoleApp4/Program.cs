using System;
using System.IO;

namespace ConsoleApp4;

internal class Program
{
    static void Main(string[] args)
    {
        string[] mapFiles = { "map1.json", "map2.json" };
        int currentMapIndex = 0;

        while (currentMapIndex < mapFiles.Length)
        {
            MapData mapData = MapManager.LoadMap(mapFiles[currentMapIndex]);
            PlayGame(mapData);

            currentMapIndex++;
        }

        Console.WriteLine("모든 맵을 클리어했습니다!");
    }

    static void PlayGame(MapData mapData)
    {
        int playerStartX = mapData.PlayerStart.X;
        int playerStartY = mapData.PlayerStart.Y;
        const int minX = 0;
        const int maxX = 30;
        const int minY = 0;
        const int maxY = 30;

        Player player = new Player(playerStartX, playerStartY);
        Box[] boxes = CreateBoxes(mapData.Boxes);
        Wall[] walls = CreateWalls(mapData.Walls);
        Goal[] goals = CreateGoals(mapData.Goals);
        RandomBox randomBox = new RandomBox(mapData.RandomBox.X, mapData.RandomBox.Y);

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
    }

    static Box[] CreateBoxes(List<Position> positions)
    {
        Box[] boxes = new Box[positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            boxes[i] = new Box(positions[i].X, positions[i].Y);
        }
        return boxes;
    }

    static Wall[] CreateWalls(List<Position> positions)
    {
        Wall[] walls = new Wall[positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            walls[i] = new Wall(positions[i].X, positions[i].Y);
        }
        return walls;
    }

    static Goal[] CreateGoals(List<Position> positions)
    {
        Goal[] goals = new Goal[positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            goals[i] = new Goal(positions[i].X, positions[i].Y);
        }
        return goals;
    }

    static int GetRandomBoxPoint(Random random)
    {
        double chance = random.NextDouble();
        if (chance < 0.90) return 1;
        if (chance < 0.99) return 10;
        if (chance < 0.999) return 100;
        return 100000;
    }

    static bool IsCollided(Position pos1, Position pos2) => pos1.X == pos2.X && pos1.Y == pos2.Y;

    static void PushOut(Direction moveDirection, Position objPos, Position collidedPos)
    {
        switch (moveDirection)
        {
            case Direction.Left:
                objPos.X = Math.Min(collidedPos.X + 1, 30);
                break;
            case Direction.Right:
                objPos.X = Math.Max(collidedPos.X - 1, 0);
                break;
            case Direction.Up:
                objPos.Y = Math.Min(collidedPos.Y + 1, 30);
                break;
            case Direction.Down:
                objPos.Y = Math.Max(collidedPos.Y - 1, 0);
                break;
        }
    }

    static void MoveBox(Direction moveDirection, Position boxPos, Position playerPos)
    {
        switch (moveDirection)
        {
            case Direction.Left:
                boxPos.X = Math.Max(playerPos.X - 1, 0);
                break;
            case Direction.Right:
                boxPos.X = Math.Min(playerPos.X + 1, 30);
                break;
            case Direction.Up:
                boxPos.Y = Math.Max(playerPos.Y - 1, 0);
                break;
            case Direction.Down:
                boxPos.Y = Math.Min(playerPos.Y + 1, 30);
                break;
        }
    }

    static int CountBoxOnGoal(Box[] boxes, Goal[] goals)
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