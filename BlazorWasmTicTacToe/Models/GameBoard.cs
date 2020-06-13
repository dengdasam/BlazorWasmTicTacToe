using BlazorWasmTicTacToe.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmTicTacToe.Models
{
    public class GameBoard
    {
        public GamePiece[,] Board { get; set; }

        public PieceStyle CurrentTurn = PieceStyle.X;

        public bool GameComplete => GetWinner() != null || IsADraw();


        public GameBoard()
        {
            Reset();
        }

        public void Reset()
        {
            Board = new GamePiece[3, 3];

            //初始化棋盘，将棋子转换为空白
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    Board[i, j] = new GamePiece();
                }
            }
        }

        //Given the coordinates of the space that was clicked...
        public void PieceClicked(int x, int y)
        {
            //如果棋局结束，则不能落子
            if (GameComplete) { return; }

            //判断落子区域是否空白，空白才能落子
            GamePiece clickedSpace = Board[x, y];
            if (clickedSpace.Style == PieceStyle.Blank)
            {
                //将落子区域绘制为 X 或者 O
                clickedSpace.Style = CurrentTurn;
                SwitchTurns();
            }
        }

        private void SwitchTurns()
        {
            //判断轮到哪方落子
            CurrentTurn = CurrentTurn == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        }

        public bool IsADraw()
        {
            int pieceBlankCount = 0;

            //计算空白区域，如果空白区域为0，则表明棋局结束
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    pieceBlankCount = this.Board[i, j].Style == PieceStyle.Blank
                                        ? pieceBlankCount + 1
                                        : pieceBlankCount;
                }
            }

            return pieceBlankCount == 0;
        }

        public WinningPlay GetWinner()
        {
            WinningPlay winningPlay = null;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    foreach (EvaluationDirection evalDirection in (EvaluationDirection[])Enum.GetValues(typeof(EvaluationDirection)))
                    {
                        winningPlay = EvaluatePieceForWinner(i, j, evalDirection);
                        if (winningPlay != null) { return winningPlay; }
                    }
                }
            }

            return winningPlay;

        }

        private WinningPlay EvaluatePieceForWinner(int i, int j, EvaluationDirection dir)
        {
            GamePiece currentPiece = Board[i, j];
            if (currentPiece.Style == PieceStyle.Blank)
            {
                return null;
            }

            int inARow = 1;
            int iNext = i;
            int jNext = j;

            var winningMoves = new List<string>();

            while (inARow < 3)
            {
                switch (dir)
                {
                    case EvaluationDirection.Up:
                        jNext -= 1;
                        break;
                    case EvaluationDirection.UpRight:
                        iNext += 1;
                        jNext -= 1;
                        break;
                    case EvaluationDirection.Right:
                        iNext += 1;
                        break;
                    case EvaluationDirection.DownRight:
                        iNext += 1;
                        jNext += 1;
                        break;
                }
                if (iNext < 0 || iNext >= 3 || jNext < 0 || jNext >= 3) { break; }
                if (Board[iNext, jNext].Style == currentPiece.Style)
                {
                    winningMoves.Add($"{iNext},{jNext}");
                    inARow++;
                }
                else
                {
                    return null;
                }
            }

            if (inARow >= 3)
            {
                winningMoves.Add($"{i},{j}");

                return new WinningPlay()
                {
                    WinningMoves = winningMoves,
                    WinningStyle = currentPiece.Style,
                    WinningDirection = dir,
                };
            }

            return null;
        }

        public string GetGameCompleteMessage()
        {
            var winningPlay = GetWinner();
            return winningPlay != null ? $"{winningPlay.WinningStyle} Wins!" : "Draw!";
        }

        public bool IsGamePieceAWinningPiece(int i, int j)
        {
            var winningPlay = GetWinner();
            return winningPlay?.WinningMoves?.Contains($"{i},{j}") ?? false;
        }
    }
}
