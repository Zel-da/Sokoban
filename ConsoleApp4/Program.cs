namespace ConsoleApp4
{
     enum Direction
    {
       None,
       Left,
       Right,
       Up,
       Down
    }

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

            int playerX = 0;
            int playerY = 0;
            const int minX = 0;
            const int maxX = 30;
            const int minY = 0;
            const int maxY = 30;

            Direction playerMoveDirection = Direction.None;

            int[] boxPositionsX = { 5, 5, 5, 5, 5};
            int[] boxPositionsY = { 5, 4, 3, 2, 1};
            int[] wallPositionsX = { 7, 7, 7, 7, 7};
            int[] wallPositionsY = { 7, 8, 9, 10, 11};
            int[] goalPositionsX = { 10, 10, 10, 10, 10};
            int[] goalPositionsY = { 10, 9, 8, 7, 6};
            int[] randomboxPositionsX = { 13};
            int[] randomboxPositionsY = { 13};

            int pushedBoxIndex = 0;

            bool[] isBoxOnGoal = new bool[boxPositionsX.Length];
            
            //bool isJump = false;
            //bool[] isBoxOnGoal = new bool[boxPositionsX.Length];

            int randomboxPoint = 0;
            Random random = new Random();
            
            while (true)
            {
                Console.Clear();
                RenderObject(playerX, playerY, "P");
                int goalCount = goalPositionsX.Length;
                for (int i = 0; i < goalCount; ++i)
                {
                    RenderObject(goalPositionsX[i], goalPositionsY[i], "G");
                }
                int boxCount = boxPositionsX.Length;
                for (int i = 0; i < boxCount; ++i)
                {
                    string boxIcon = isBoxOnGoal[i] ? "O" : "B";
                    RenderObject(boxPositionsX[i], boxPositionsY[i], boxIcon);
                }
                int wallCount = wallPositionsX.Length;
                for (int i = 0; i < wallCount; ++i)
                {
                    RenderObject(wallPositionsX[i], wallPositionsY[i], "W");
                }
                int randomboxCount = randomboxPositionsX.Length;
                for(int i = 0; i < randomboxCount; i++)
                {
                    RenderObject(randomboxPositionsX[i], randomboxPositionsY[i], "R");
                }
                Console.SetCursorPosition(0, 31);
                Console.WriteLine("Point : " + randomboxPoint);
                
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                ConsoleKey key = keyInfo.Key;

                MovePlayer(key, ref playerX, ref playerY, ref playerMoveDirection);

                for (int i = 0; i < wallCount; ++i)
                {
                    if (false == IsCollided(playerX, playerY, wallPositionsX[i], wallPositionsY[i]))
                    {
                        continue;
                    }
                    
                    OnCollision(() =>
                    {
                        PushOut(playerMoveDirection, ref playerX, ref playerY, wallPositionsX[i], wallPositionsY[i]);
                    });
                }

                for (int i = 0; i < boxCount; ++i)
                {
                    if (false == IsCollided(playerX, playerY, boxPositionsX[i], boxPositionsY[i]))
                        continue;
                    
                    OnCollision(() =>
                    {
                        MoveBox(playerMoveDirection, ref boxPositionsX[i], ref boxPositionsY[i], playerX, playerY);
                    });
                    
                    pushedBoxIndex = i;
                    
                    break;
                }

                for (int i = 0; i < boxCount; ++i)
                {
                    if (pushedBoxIndex == i)
                        continue;

                    if (false == IsCollided(boxPositionsX[pushedBoxIndex], boxPositionsY[pushedBoxIndex], boxPositionsX[i], boxPositionsY[i]))
                        continue;
                    
                    OnCollision(() =>
                    {
                        PushOut(playerMoveDirection, ref boxPositionsX[pushedBoxIndex], ref boxPositionsY[pushedBoxIndex], boxPositionsX[i], boxPositionsY[i]);
                        PushOut(playerMoveDirection, ref playerX, ref playerY, boxPositionsX[pushedBoxIndex], boxPositionsY[pushedBoxIndex]);
                    });
                }

                for (int i = 0; i < wallCount; ++i)
                {
                    if (false == IsCollided(boxPositionsX[pushedBoxIndex], boxPositionsY[pushedBoxIndex], wallPositionsX[i], wallPositionsY[i]))
                        continue;
                    
                    OnCollision(() =>
                    {
                        PushOut(playerMoveDirection, ref boxPositionsX[pushedBoxIndex], ref boxPositionsY[pushedBoxIndex], wallPositionsX[i], wallPositionsY[i]);
                        PushOut(playerMoveDirection, ref playerX, ref playerY, boxPositionsX[pushedBoxIndex], boxPositionsY[pushedBoxIndex]);
                    });
                    
                    break;
                }
                int boxOnGoalCount = CountBoxOnGoal(in boxPositionsX, in boxPositionsY, ref isBoxOnGoal, in goalPositionsX, in goalPositionsY);
                if (boxOnGoalCount == goalCount)
                    break;

                for (int i = 0; i < randomboxCount; i++)
                {
                    if (IsCollided(playerX, playerY, randomboxPositionsX[i], randomboxPositionsY[i]))
                    {
                        randomboxPoint += GetRandomBoxPoint(random);
                        randomboxPositionsX[i] = new Random().Next(minX + 1, maxX);
                        randomboxPositionsY[i] = new Random().Next(minY + 1, maxY);
                    }
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

            void RenderObject(int x, int y, string icon)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(icon);
            }
            
            int CountBoxOnGoal(in int[] boxPositionsX, in int[] boxPositionsY, ref bool[] isBoxOnGoal, in int[] goalPositionsX, in int[] goalPositionsY)
            {
                int boxCount = boxPositionsX.Length;
                int goalCount = goalPositionsX.Length;
                int result = 0;
                for (int boxId = 0; boxId < boxCount; ++boxId)
                {
                    isBoxOnGoal[boxId] = false;
                    for (int goalId = 0; goalId < goalCount; ++goalId)
                    {
                        if (IsCollided(boxPositionsX[boxId], boxPositionsY[boxId], goalPositionsX[goalId], goalPositionsY[goalId]))
                        {
                            ++result;
                            isBoxOnGoal[boxId] = true;
                            break;
                        }
                    }
                }
                return result;
            }
            
            void MoveToLeftOfTarget(out int x, in int target) => x = Math.Max(minX, target - 1);
            void MoveToRightOfTarget(out int x, in int target) => x = Math.Min(target + 1, maxX);
            void MoveToUpOfTarget(out int y, in int target) => y = Math.Max(minY, target - 1);
            void MoveToDownOfTarget(out int y, in int target) => y = Math.Min(target + 1, maxY);

            void MovePlayer(ConsoleKey key, ref int x, ref int y, ref Direction moveDirection)
            {
                if (key == ConsoleKey.LeftArrow)
                {
                    MoveToLeftOfTarget(out x, in x);
                    moveDirection = Direction.Left;
                }
                if (key == ConsoleKey.RightArrow)
                {
                    MoveToRightOfTarget(out x, in x);
                    moveDirection = Direction.Right;
                }
                if (key == ConsoleKey.UpArrow)
                {
                    MoveToUpOfTarget(out y, in y);
                    moveDirection = Direction.Up;
                }
                if (key == ConsoleKey.DownArrow)
                {
                    MoveToDownOfTarget(out y, in y);
                    moveDirection = Direction.Down;
                }
            }
            
            void OnCollision(Action action)
            {
                action();
            }
            void PushOut(Direction playerMoveDirection, ref int objX, ref int objY, in int collidedObjX, in int collidedObjY)
            {
                switch (playerMoveDirection)
                {
                    case Direction.Left:
                        MoveToRightOfTarget(out objX, in collidedObjX);
                        break;
                    
                    case Direction.Right:
                        MoveToLeftOfTarget(out objX, in collidedObjX);
                        break;
                    
                    case Direction.Up:
                        MoveToDownOfTarget(out objY, in collidedObjY);
                        break;
                    
                    case Direction.Down:
                        MoveToUpOfTarget(out objY, in collidedObjY);
                        break;
                }
            }
            void MoveBox(Direction playerMoveDirection, ref int boxX, ref int boxY, in int playerX, in int playerY)
            {
                switch (playerMoveDirection)
                {
                    case Direction.Left:
                        MoveToLeftOfTarget(out boxX, in playerX);
                        break;
                    
                    case Direction.Right:
                        MoveToRightOfTarget(out boxX, in playerX);
                        break;
                    
                    case Direction.Up:
                        MoveToUpOfTarget(out boxY, in playerY);
                        break;
                    
                    case Direction.Down:
                        MoveToDownOfTarget(out boxY, in playerY);
                        break;
                }
            }
            
            bool IsCollided(int x1, int y1, int x2, int y2)
            {
                if (x1 == x2 && y1 == y2)
                    return true;
                else
                    return false;
            }
        }
    }

    class positionXY
    {
        public int X;
        public int Y;
    }

}