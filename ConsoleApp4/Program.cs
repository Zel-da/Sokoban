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
            
            // 플레이어 위치 좌표
            int playerX = 0;
            int playerY = 0;
            const int minX = 0;
            const int maxX = 30;
            const int minY = 0;
            const int maxY = 30;
            
            // 플레이어 방향
            Direction playerMoveDirection = Direction.None;
            
            // 좌표
            int[] boxPositionsX = { 5, 5, 5, 5, 5};
            int[] boxPositionsY = { 5, 4, 3, 2, 1};
            int[] wallPositionsX = { 7, 7, 7, 7, 7};
            int[] wallPositionsY = { 7, 8, 9, 10, 11};
            int[] goalPositionsX = { 10, 10, 10, 10, 10};
            int[] goalPositionsY = { 10, 9, 8, 7, 6};

            // 플레이어가 민 박스의 인덱스
            int pushedBoxIndex = 0;

            // 각 박스마다 골 위에 올라와 있는지에 관한 데이터다
            bool[] isBoxOnGoal = new bool[boxPositionsX.Length];
            
            // 박스 삭제
            //bool isJump = false;
            //bool[] isBoxOnGoal = new bool[boxPositionsX.Length];
            
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
                
                // 유저로부터 입력을 받는다
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                ConsoleKey key = keyInfo.Key; // 실제 키는 ConsoleKeyInfo에 Key에 있다

                MovePlayer(key, ref playerX, ref playerY, ref playerMoveDirection);

                // 플레이어와 벽의 충돌 처리
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
                
                // 박스 업데이트
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
                
                // 박스끼리의 충돌 처리
                for (int i = 0; i < boxCount; ++i)
                {
                    // 같은 박스라면 처리할 필요가 없다
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
                
                // 박스와 벽의 충돌 처리
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
            }
            
            // 오브젝트를 그린다
            void RenderObject(int x, int y, string icon)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(icon);
            }
            
            // 골 위에 박스가 몇 개 있는지 센다
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

            // target 근처로 이동시킨다
            void MoveToLeftOfTarget(out int x, in int target) => x = Math.Max(minX, target - 1);
            void MoveToRightOfTarget(out int x, in int target) => x = Math.Min(target + 1, maxX);
            void MoveToUpOfTarget(out int y, in int target) => y = Math.Max(minY, target - 1);
            void MoveToDownOfTarget(out int y, in int target) => y = Math.Min(target + 1, maxY);

            // 플레이어를 움직인다
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

            // 충돌을 처리한다
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
            // 박스를 움직인다
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
            
            // 충돌했는지 검사한다
            bool IsCollided(int x1, int y1, int x2, int y2)
            {
                if (x1 == x2 && y1 == y2)
                    return true;
                else
                    return false;
            }
        }
    }

}