using Microsoft.Ajax.Utilities;
using SudokuSolver.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace SudokuSolver.Logics
{
    public class Solver
    {
        int soleCandidateNumber;
        int uniqueCandidateNumber;
        bool isFull = false;

        public int[] GetBlockNumbers(double rowNumber, double columnNumber, int[][] sudoku)
        {
            double blockStartRow = rowNumber - (rowNumber % 3);
            double blockStartColumn = columnNumber - (columnNumber % 3);

            int[] blockNumbers = new int[9];
            int i = 0;
            for (int r = (int)blockStartRow; r < blockStartRow + 3; r++)
            {
                for (int c = (int)blockStartColumn; c < blockStartColumn + 3; c++)
                {
                    if (c < 9 && r < 9)
                    {
                        blockNumbers[i] = sudoku[r][c] != 0 ? sudoku[r][c] : 0;
                        i++;
                    }

                }
            }

            return blockNumbers;
        }

        public int[] GetRowNumbers(int row, int[][] sudoku)
        {
            int[] rowNumbers = new int[10];
            for (int i = 0; i < 8; i++)
            {
                rowNumbers[i] = sudoku[row][i];
            }

            return rowNumbers;
        }

        public int[] GetColumnNumbers(int column, int[][] sudoku)
        {
            int[] columnNumbers = new int[10];
            for (int i = 0; i < 8; i++)
            {
                columnNumbers[i] = sudoku[i][column];
            }

            return columnNumbers;
        }

        public int[] GetAllCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] blockNumbers = GetBlockNumbers(rowNumber, columnNumber, sudoku);
            int[] rowNumbers = GetRowNumbers(rowNumber, sudoku);
            int[] columnNumbers = GetColumnNumbers(columnNumber, sudoku);

            int[] numberArray = new int[10];
            for (int i = 0; i < 9; i++)
            {
                if (blockNumbers[i] > 0)
                {
                    numberArray[blockNumbers[i]] = blockNumbers[i];
                }
                if (rowNumbers[i] > 0)
                {
                    numberArray[rowNumbers[i]] = rowNumbers[i];
                }
                if (columnNumbers[i] > 0)
                {
                    numberArray[columnNumbers[i]] = columnNumbers[i];
                }
            }

            return numberArray;
        }

        public int[] GetBlockCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] blockNumbers = GetBlockNumbers(rowNumber, columnNumber, sudoku);

            int[] numberArray = new int[10];
            for (int i = 0; i < 9; i++)
            {
                if (blockNumbers[i] > 0)
                {
                    numberArray[blockNumbers[i]] = blockNumbers[i];
                }
            }

            return numberArray;
        }

        public int[] GetRCCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] rowNumbers = GetRowNumbers(rowNumber, sudoku);
            int[] columnNumbers = GetColumnNumbers(columnNumber, sudoku);

            int[] numberArray = new int[10];
            for (int i = 0; i < 9; i++)
            {
                if (rowNumbers[i] > 0)
                {
                    numberArray[rowNumbers[i]] = rowNumbers[i];
                }
                if (columnNumbers[i] > 0)
                {
                    numberArray[columnNumbers[i]] = columnNumbers[i];
                }
            }

            return numberArray;
        }

        public int[] GetRowCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] rowNumbers = GetRowNumbers(rowNumber, sudoku);

            int[] numberArray = new int[10];
            for (int i = 0; i < 9; i++)
            {
                if (rowNumbers[i] > 0)
                {
                    numberArray[rowNumbers[i]] = rowNumbers[i];
                }
            }

            return numberArray;
        }

        public int[] GetColumnCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] columnNumbers = GetColumnNumbers(columnNumber, sudoku);

            int[] numberArray = new int[10];
            for (int i = 0; i < 9; i++)
            {
                if (columnNumbers[i] > 0)
                {
                    numberArray[columnNumbers[i]] = columnNumbers[i];
                }
            }

            return numberArray;
        }

        public int UniqueCandidate(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int rowNumberBottom = 0;
            int rowNumberMid = 0;
            int rowNumberTop = 0;
            int columnNumberLeft = 0;
            int columnNumberMid = 0;
            int columnNumberRight = 0;

            if (rowNumber >= 0 || rowNumber <= 2)
            {
                rowNumberBottom = 2;
                rowNumberMid = 1;
                rowNumberTop = 0;
            }
            else if (rowNumber >= 3 || rowNumber <= 5)
            {
                rowNumberBottom = 5;
                rowNumberMid = 4;
                rowNumberTop = 3;
            }
            else if (rowNumber >= 6 || rowNumber <= 8)
            {
                rowNumberBottom = 6;
                rowNumberMid = 7;
                rowNumberTop = 8;
            }

            if (columnNumber >= 0 || columnNumber <= 2)
            {
                columnNumberRight = 2;
                columnNumberMid = 1;
                columnNumberLeft = 0;
            }
            if (columnNumber >= 3 || columnNumber <= 5)
            {
                columnNumberRight = 5;
                columnNumberMid = 4;
                columnNumberLeft = 3;
            }
            if (columnNumber >= 6 || columnNumber <= 8)
            {
                columnNumberRight = 8;
                columnNumberMid = 7;
                columnNumberLeft = 6;
            }


            bool hasLooped = false;
            for (int j = 1; j < 10 && hasLooped == false; j++)
            {
                int[] blockNumber = GetBlockNumbers(rowNumber, columnNumber, sudoku);
                int[] blockCandidates = GetBlockCandidates(rowNumber, columnNumber, sudoku);

                int[] rowTopCandidates = GetRowCandidates(rowNumberTop, columnNumber, sudoku);
                int[] rowMidCandidates = GetRowCandidates(rowNumberMid, columnNumber, sudoku);
                int[] rowBottomCandidates = GetRowCandidates(rowNumberBottom, columnNumber, sudoku);
                int[] columnLeftCandidates = GetColumnCandidates(rowNumber, columnNumberLeft, sudoku);
                int[] columnMidCandidates = GetColumnCandidates(rowNumber, columnNumberMid, sudoku);
                int[] columnRightCandidates = GetColumnCandidates(rowNumber, columnNumberRight, sudoku);

                if (blockCandidates[j] == 0 && sudoku[rowNumber][columnNumber] == 0)
                {
                    if (rowTopCandidates[j] == j)
                    {
                        blockNumber[0] = -1;
                        blockNumber[1] = -1;
                        blockNumber[2] = -1;
                    }
                    if (rowMidCandidates[j] == j)
                    {
                        blockNumber[3] = -1;
                        blockNumber[4] = -1;
                        blockNumber[5] = -1;
                    }
                    if (rowBottomCandidates[j] == j)
                    {
                        blockNumber[6] = -1;
                        blockNumber[7] = -1;
                        blockNumber[8] = -1;
                    }


                    if (columnLeftCandidates[j] == j)
                    {
                        blockNumber[0] = -1;
                        blockNumber[3] = -1;
                        blockNumber[6] = -1;
                    }
                    if (columnMidCandidates[j] == j)
                    {
                        blockNumber[1] = -1;
                        blockNumber[4] = -1;
                        blockNumber[7] = -1;
                    }
                    if (columnRightCandidates[j] == j)
                    {
                        blockNumber[2] = -1;
                        blockNumber[5] = -1;
                        blockNumber[8] = -1;
                    }


                    int zeroCount = 0;
                    int solvedNumber = 0;
                    for (int i = 0; i < 9 && zeroCount < 3; i++)
                    {
                        if (blockNumber[i] == 0)
                        {
                            zeroCount++;
                            int row1 = Convert.ToInt32(Math.Floor(Convert.ToDouble((rowNumber + 1) / 3)));
                            int column1 = (columnNumber + 1) % 3;
                            int row2 = Convert.ToInt32(Math.Floor(Convert.ToDouble((i + 1) / 3) - 1));
                            int column2 = (i + 1) % 3;
                            if (row1 == row2 && column1 == column2)
                            {
                                solvedNumber = j;
                            }
                        }
                    }

                    if (zeroCount == 1 && solvedNumber > 0)
                    {
                        return solvedNumber;
                        break;
                    }
                }
            }
            return 0;
        }

        public int soleCandidate(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] candidates = GetAllCandidates(rowNumber, columnNumber, sudoku);


            int zeroCount = 0;
            int solvedNumber = 0;
            for (int i = 1; i < candidates.Length; i++)
            {
                if (candidates[i] == 0)
                {
                    solvedNumber = i;
                    zeroCount++;
                }
            }

            if (zeroCount == 1)
            {
                return solvedNumber;
                sudoku[rowNumber][columnNumber] = solvedNumber;
            }

            return 0;
        }


        public int[][] Solve(int[][] sudoku)
        {
            int emptyCells = 0;
            int emptyCellsPrev = -1;
            int runs = 0;
            while (emptyCells != emptyCellsPrev && runs < 1000)
            {
                emptyCellsPrev = emptyCells;

                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        if (sudoku[r][c] == 0)
                        {
                            //soleCandidate(r, c, sudoku);
                            UniqueCandidate(r, c, sudoku);
                            emptyCells++;
                        }
                    }
                }
                Debug.WriteLine(runs);
                runs++;
            }


            return sudoku;
        }

        public int[][] SolveGuessing(int[][] sudoku)
        {
            int emptyCells = 0;
            int emptyCellsPrev = -1;
            int loopCount = 0;
            while (emptyCells != emptyCellsPrev && loopCount < 1000)
            {
                emptyCellsPrev = emptyCells;

                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        if (sudoku[r][c] == 0)
                        {
                            soleCandidate(r, c, sudoku);
                            emptyCells++;
                        }
                    }
                }

                loopCount++;
            }
            return sudoku;
        }

        public int[][] Create(int[][] sudoku)
        {
            return sudoku;
        }
    }
}