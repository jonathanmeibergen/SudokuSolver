using Microsoft.Ajax.Utilities;
using SudokuSolver.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
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
        private int seed = 1;
        private Random rnd = new Random();

        public List<int> getBlockNumbers(double rowNumber, double columnNumber, ref List<int> candidates, int[][] sudoku)
        {
            double blockStartRow = rowNumber - (rowNumber % 3);
            double blockStartColumn = columnNumber - (columnNumber % 3);

            for (int r = (int)blockStartRow; r < blockStartRow + 3; r++)
            {
                for (int c = (int)blockStartColumn; c < blockStartColumn + 3; c++)
                {
                    candidates.Remove(sudoku[r][c]);
                }
            }

            return candidates;
        }

        public List<int> getRowNumbers(int row, ref List<int> candidates, int[][] sudoku)
        {
            for (int i = 0; i < 9; i++)
            {
                candidates.Remove(sudoku[row][i]);
            }

            return candidates;
        }

        public List<int> getColumnNumbers(int column, ref List<int> candidates, int[][] sudoku)
        {
            for (int i = 0; i < 9; i++)
            {
                candidates.Remove(sudoku[i][column]);
            }

            return candidates;
        }

        public List<int> getAllCandidates(int rowNumber, int columnNumber, int[][] sudoku)
        {
            List<int> candidates = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            getBlockNumbers(rowNumber, columnNumber, ref candidates, sudoku);
            getRowNumbers(rowNumber, ref candidates, sudoku);
            getColumnNumbers(columnNumber, ref candidates, sudoku);

            return candidates;
        }

        public int soleCandidate(int rowNumber, int columnNumber, int[][] sudoku)
        {
            List<int> candidates = getAllCandidates(rowNumber, columnNumber, sudoku);

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            return 0;
        }


        public int[][] solveLogical(int[][] sudoku)
        {
            int ivar = 81;
            int skip = 0;
            while (ivar > 1)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        if (sudoku[r][c] == 0)
                        {
                            int value = soleCandidate(r, c, sudoku);
                            if (value > 0)
                            {
                                sudoku[r][c] = value;
                            }
                        }
                        else 
                            --ivar;

                    }
                }
            }

            return sudoku;
        }

        static int[][] CopyArrayBuiltIn(int[][] source)
        {
            var len = source.Length;
            var dest = new int[len][];
            for (var x = 0; x < len; x++)
            {
                var inner = source[x];
                var ilen = inner.Length;
                var newer = new int[ilen];
                Array.Copy(inner, newer, ilen);
                dest[x] = newer;
            }
            return dest;
        }

        public bool solveRec(int row, int col, int[][] sudoku, int rndInt)
        {
            //DateTime.Now;
            //Debug.WriteLine(new String('=', (row * 9) + col + 1) + " " + DateTime.Now);
            int random = rnd.Next(1, rndInt);

            //walking trough the sudoku from top left to bottom right
            if (col >= 9)
            {
                //next line please
                ++row; col -= 9;
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

                //if there are no candidates return to previous function call, nothing to guess here
                if (candidates.Count == 0)
                    return false;

                //if there are candidates, walk through them in a linear fashion
                foreach (var guess in candidates)
                {
                    //if (1 == random)
                    //    continue;
                    //lets try out a guess from candidates
                    sudoku[row][col] = guess;

                    //now lets move on to the next cell and get some work done.
                    //When eventually succesfull please pass on the good news to our ancestor function call

                    if (solveRec(row, col + random, sudoku, rndInt))
                        // its solved let it cascade
                        return true;
                }
                //end of our guesses, lets restore the current cell to 0,
                //well be back with different guesses 
                sudoku[row][col] = 0;
            }
            //if the current cell is not empty, please leave it alone, its the part of the 
            //sudoku we are trying to solve, move on to the next cell please!
            else if (solveRec(row, col + random, sudoku, rndInt))
                // its solved let it cascade
                return true;

            //well it seemes we didnt solve anything here, better luck next time..., 
            //go up the call chain and see what we can do up there
            return false;
        }

        public int[][] Solve(int[][] sudoku)
        {

            return solveLogical(sudoku);
        }

        public int[][] SolveGuessing(int[][] sudoku)
        {
            solveRec(0, 0, sudoku, 1);
            return sudoku;
        }

        public int[][] Create(int[][] sudoku, Random rnd)
        {
            solveRec(0, 0, sudoku, 1);

            int rndInt = 0, colSwap;

            for (int i = 2; i >= 0; i--)
            {
                for (int j = 2; j >= 0; j--)
                {
                    rndInt = rnd.Next(0, j);
                    int colIndex = (i * 3) + j;
                    int colRnd = (i * 3) + rndInt;
                    for (int k = 0; k < 9; k++)
                    {
                        colSwap = sudoku[k][colIndex];
                        sudoku[k][colIndex] = sudoku[k][colRnd];
                        sudoku[k][colRnd] = colSwap;
                    }
                }
            }

            for (int i = 2; i >= 0; i--)
            {
                for (int j = 2; j >= 0; j--)
                {
                    rndInt = rnd.Next(0, j);
                    int[] rowSwap;
                    int rowIndex = (i * 3) + j;
                    int rowRnd = (i * 3) + rndInt;
                    rowSwap = sudoku[rowIndex];
                    sudoku[rowIndex] = sudoku[rowRnd];
                    sudoku[rowRnd] = rowSwap;

                }

            }

            for (int i = 2; i >= 0; i--)
            {
                rndInt = rnd.Next(0, i);

                for (int j = 2; j <= 0; j++)
                {
                    int[] rowSwap;
                    int rowIndex = (i * 3) + j;
                    int rowRnd = (rndInt * 3) + j;
                    rowSwap = sudoku[rowIndex];
                    sudoku[rowIndex] = sudoku[rowRnd];
                    sudoku[rowRnd] = rowSwap;
                }
            }

            for (int i = 2; i >= 0; i--)
            {
                rndInt = rnd.Next(0, i);

                for (int j = 2; j >= 0; j--)
                {
                    int colIndex = (i * 3) + j;
                    int colRnd = (rndInt * 3) + j;
                    for (int k = 0; k < 9; k++)
                    {
                        colSwap = sudoku[k][colIndex];
                        sudoku[k][colIndex] = sudoku[k][colRnd];
                        sudoku[k][colRnd] = colSwap;
                    }
                }
            }

            List<int> indices = new List<int>();
            for (int i = 0; i < 81; i++)
            {
                indices.Add(i);
            }
            for (int i = indices.Count - 1; i > 0; i--)
            {
                rndInt = rnd.Next(0, i);

                int swap = indices[i];
                indices[i] = indices[rndInt];
                indices[rndInt] = swap;
            }

            List<int[]> picks = new List<int[]>();
            foreach (var item in indices)
            {
                int row = Convert.ToInt32(Math.Floor(Convert.ToDecimal(item / 9)));
                int col = item % 9;
                int[] coord = new int[2] { row, col };

                int cellValue = sudoku[row][col];
                sudoku[row][col] = 0;

                int[][] temp = solveLogical(sudoku);

                if (temp[row][col] == 0)
                {
                    sudoku[row][col] = cellValue;
                }

                foreach (var pick in picks)
                {
                    sudoku[pick[0]][pick[1]] = 0;
                }
                picks.Add(coord);


            }
            return sudoku;
        }
    }
}