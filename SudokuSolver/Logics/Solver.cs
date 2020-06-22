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

        public int[][] Solve(int[][] sudoku)
        {

            int[] temp = new int[6];
            Console.WriteLine(temp.Count());
            Console.WriteLine(temp.Length);
            Console.WriteLine(temp.Rank);
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