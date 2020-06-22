using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SudokuSolver.Models
{
    public class Sudoku
    {
        public int SudokuId { get; set; }
        public string Name { get; set; }
        public int[][] Cells { get; set; }
    }
}