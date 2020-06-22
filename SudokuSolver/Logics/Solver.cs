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
            sudoku[0][0] = 2;
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