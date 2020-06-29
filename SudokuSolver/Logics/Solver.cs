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
        private int seed = 1;
        private Random rnd = new Random();

        public List<int> getBlockNumbers(double rowNumber, double columnNumber, ref List<int> candidates, int[][] sudoku)
        {
            double blockStartRow = rowNumber - (rowNumber%3);
            double blockStartColumn = columnNumber - (columnNumber % 3);

            for (int r = (int)blockStartRow; r < blockStartRow+3;r++)
            {
                for (int c = (int)blockStartColumn; c < blockStartColumn+3; c++)
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
            List<int> candidates = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            getBlockNumbers(rowNumber, columnNumber, ref candidates, sudoku);
            getRowNumbers(rowNumber, ref candidates, sudoku);
            getColumnNumbers(columnNumber, ref candidates, sudoku);

            return candidates;
        }

        public bool soleCandidate(int rowNumber,int columnNumber, int[][] sudoku)
        {
            List<int> candidates = getAllCandidates(rowNumber, columnNumber, sudoku);
           
            if (candidates.Count == 1)
            {
                sudoku[rowNumber][columnNumber] = candidates[0];
            }

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
                //end of our guesses, lets restore the current cell to 0, we'll be back with different guesses 
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
            
            return solveWithForLoops(sudoku);
        }   

        public int[][] SolveGuessing(int[][] sudoku)
        {
            solveRec(0, 0, sudoku, 1);
            return sudoku;
        }

        public int[][] Create(int[][] sudoku, Random rnd)
        {
            solveRec(0, 0, sudoku, rnd.Next(1,1));
            
            int[] rowSwap;
            int rndInt = 0, colSwap;

            //insert 1 trough 9 diagonally
            for (int i = 8; i >= 0; i--)
            {
                rndInt = rnd.Next(0, i);
                sudoku[i][i] = i + 1;
            }

            for (int j = 0; j < 9; j++)
            {

            }

            //swap columns and rows
            for (int i = 8; i >= 0; i--)
            {
                rndInt = rnd.Next(0, i);

                for (int j = 0; j < 9; j++)
                {
                    colSwap = sudoku[j][i];
                    sudoku[j][i] = sudoku[j][rndInt];
                    sudoku[j][rndInt] = colSwap;
                }
            }

            //solveRec(0, rnd.Next(0,10), sudoku, 10);

            return sudoku;
        }
    }
}