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
    public class Cell
    {
        public Cell(int row, int column)
        {
            _row = row;
        }
        public int _row { get; private set; }

        int[] candidates;
    }


    public class Solver
    {
        int soleCandidateNumber;
        int uniqueCandidateNumber;
        bool isFull = false;

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

        public int[] getBlockNumbersRec(double rowNumber, double columnNumber, int[][] sudoku)
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
                        Console.WriteLine(sudoku[r][c]);
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

        public int[] getAllCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] blockNumbers = getBlockNumbers(rowNumber, columnNumber, sudoku);
            int[] rowNumbers = getRowNumbers(rowNumber, sudoku);
            int[] columnNumbers = getColumnNumbers(columnNumber, sudoku);

            int[] numberArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < 9; i++)
            {
                if (numberArray[blockNumbers[i]] == blockNumbers[i])
                    numberArray[blockNumbers[i]] = 0;
                if (numberArray[rowNumbers[i]] == rowNumbers[i])
                    numberArray[rowNumbers[i]] = 0;
                if (numberArray[columnNumbers[i]] == columnNumbers[i])
                    numberArray[columnNumbers[i]] = 0;
            }

            return numberArray;
        }

        public int[] getRCCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] rowNumbers = getRowNumbers(rowNumber, sudoku);
            int[] columnNumbers = getColumnNumbers(columnNumber, sudoku);

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

        public int[] getRowCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] rowNumbers = getRowNumbers(rowNumber, sudoku);

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

        public int[] getColumnCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            int[] columnNumbers = getRowNumbers(columnNumber, sudoku);

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


        public bool soleCandidate(int rowNumber,int columnNumber, int[][] sudoku)
        {
            int[] candidates = getAllCandidates(rowNumber, columnNumber, sudoku);

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
                sudoku[rowNumber][columnNumber] = solvedNumber;
            }

            return true;
        }

        public bool uniqueCandidate()
        {
            return true;
        }

        public int[][] solveWithForLoops(int [][] sudoku)
        {
            int emptyCells = 0;
            int emptyCellsPrev = -1;
            while (emptyCells != emptyCellsPrev && emptyCells < 100000)
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
            }

            return sudoku;
        }

        public bool solveRec(int row, int col, int[][] sudoku)
        {
            //walking trough the sudoku from top left to bottom right
            if (col == 9)
            {
                //next line please
                ++row; col = 0;
                //if we hit the last column on the last row it means we are succesfull and 
                //so we should tell 'true' to our ancestor function and 
                //let it cascade all the way up trough the call chain
                if (row == 9)
                    return true;
            }

            //if the current cell is 0 lets try to fill it
            if (sudoku[row][col] == 0)
            {
                //get all canditates for the current cell
                List<int> candidates = new List<int>(getAllCandidates(row, col, sudoku));
                //sanitize the candidates list
                candidates.RemoveAll(item => item == 0);

                //if there are no candidates return to previous function call, nothing to guess here
                if (candidates.Count == 0)
                    return false;

                //if there are candidates, walk through them in a linear fashion
                foreach (var guess in candidates)
                {
                    //lets try out a guess from candidates
                    sudoku[row][col] = guess;

                    //now lets move on to the next cell and get some work done.
                    //When eventually succesfull please pass on the good news to our ancestor function call
                    if (solveRec(row, col + 1, sudoku))
                        // great so far so good, go up the call chain
                        return true;
                }
                //end of our guesses, lets restore the current cell to 0, we'll be back with different guesses 
                sudoku[row][col] = 0;
            }
            //if the current cell is not empty, please leave it alone, its the part of the 
            //sudoku we are trying to solve, move on to the next cell please!
            else if (solveRec(row, col + 1, sudoku))
                    // great so far so good, go up the call chain
                    return true;

            //well it seemes we didnt solve anything here, better luck next time..., 
            //go up the call chain and see what we can do up there
            return false;
        }

        public int[][] Solve(int[][] sudoku)
        {
            
            return solveWithForLoops(sudoku);
        }   

        public int[][] SolveGuessing(int[][] sudoku)
        {
            //sudoku[0][0] = 6;
            solveRec(0, 0, sudoku);
            return sudoku;
        }

        public int[][] Create(int[][] sudoku)
        {
            return sudoku;
        }
    }
}