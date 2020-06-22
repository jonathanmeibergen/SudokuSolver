using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SudokuSolver.Models
{
    public class SudokuModel
    {
        public int[][] Cells { get; set; }
        public string Alert { get; set; } = string.Empty;

        public int SudokuId { get; set; }
        public IEnumerable<Sudoku> Sudokus { get; set; }
    }
}