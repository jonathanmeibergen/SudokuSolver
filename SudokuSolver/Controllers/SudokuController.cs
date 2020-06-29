using Microsoft.Ajax.Utilities;
using SudokuSolver.Logics;
using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SudokuSolver.Controllers
{
    public class SudokuController : Controller
    {
        private Solver solver = new Solver();
        private SudokuModel sudokuModel = new SudokuModel();
        private IEnumerable<Sudoku> SudokuList = Sudokus.MockData();

        // GET: Sudoku
        public ActionResult Sudoku()
        {
            if (TempData["sudoku"] != null)
            {
                sudokuModel = TempData["sudoku"] as SudokuModel;
            }

            sudokuModel.Sudokus = SudokuList;

            if (sudokuModel.Cells == null)
            {
                sudokuModel.Cells = SudokuList.ElementAt(0).Cells;
            }

            return View(sudokuModel);
        }

        /// <summary>
        /// Solve without guessing.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ActionResult Solve(SudokuModel Model)
        {
            Model.Cells = solver.Solve(Model.Cells);
            TempData["sudoku"] = Model;
            return RedirectToAction("Sudoku");
        }

        /// <summary>
        /// Solve with guessing
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ActionResult SolveGuessing(SudokuModel Model)
        {
            Model.Cells = solver.SolveGuessing(Model.Cells);
            TempData["sudoku"] = Model;
            return RedirectToAction("Sudoku");
        }

        /// <summary>
        /// Change the Sudoku to the given sudoku Id from the Sukokus model containing several Sudokus.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChangeSudoku(int? id)
        {
            int sudokuNumber = id ?? 0;

            var model = new SudokuModel();
            model.Cells = SudokuList.ElementAt(sudokuNumber).Cells;
            model.SudokuId = sudokuNumber;
            TempData["sudoku"] = model;

            return RedirectToAction("Sudoku");
        }

        /// <summary>
        /// Creates a randomized sudoku using the empty sudoku.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSudoku(Random rnd)
        {
            sudokuModel.Cells = solver.Create(SudokuList.ElementAt(2).Cells, rnd);
            TempData["sudoku"] = sudokuModel;
            return RedirectToAction("Sudoku");
        }
    }
}