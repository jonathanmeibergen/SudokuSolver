using Microsoft.Ajax.Utilities;
using SudokuSolver.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SudokuSolver.Logics
{
    public class Solver
    {
        public int[] getBlockNumbers(double rowNumber, double columnNumber, int[][] sudoku)
        {
            double blockStartRow = rowNumber - (rowNumber%3);
            double blockStartColumn = columnNumber - (columnNumber % 3);

            int[] blockNumbers = new int[9];
            int i = 0;
            for (int r = (int)blockStartRow; r < blockStartRow+3;r++)
            {
                for (int c = (int)blockStartColumn; c < blockStartColumn+3; c++)
                {
                    if (c < 9 && r < 9)
                    {
                        blockNumbers[i] = sudoku[r][c] != 0 ? sudoku[r][c] : 0;
                        Console.WriteLine(sudoku[r][c]);
                        int test2 = i;
                        i++;
                    }

                }
            }

            return blockNumbers;
        }

        public int[] getRowNumbers(int row, int[][] sudoku)
        {
            int[] rowNumbers = new int[9];
            for (int i = 0; i < 9; i++)
            {
                rowNumbers[i] = sudoku[row][i];
            }

            return rowNumbers;
        }

        public int[] getColumnNumbers(int column, int[][] sudoku)
        {
            int[] columnNumbers = new int[9];
            for (int i = 0; i < 9; i++)
            {
                columnNumbers[i] = sudoku[i][column];
            }

            return columnNumbers;
        }

        public bool soleCandidate(int rowNumber,int columnNumber, int[][] sudoku)
        {
            int[] blockNumbers = getBlockNumbers(rowNumber, columnNumber, sudoku);
            int[] rowNumbers = getRowNumbers(rowNumber, sudoku);
            int[] columnNumbers = getColumnNumbers(columnNumber, sudoku);
            return true;
        }

        public bool uniqueCandidate()
        {
            return true;
        }

        public int[][] Solve(int[][] sudoku)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (sudoku[r][c] == 0)
                    {
                        
                        soleCandidate(r, c,sudoku);
                    }
                }
            }

            return sudoku;
        }

        public int[][] SolveGuessing(int[][] sudoku)
        {
            return sudoku;
        }

        public int[][] Create(int[][] sudoku)
        {
            return sudoku;
        }
    }
}