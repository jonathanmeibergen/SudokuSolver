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
    public class Solver
    {
        private Random rnd = new Random();

        /// <summary>
        /// return all candidates in the block containing the current cell
        /// </summary>
        /// <param name="row">the row number of the current cell</param>
        /// <param name="col">the column number of the current cell</param>
        /// <param name="candidates">list of possibly previously calculated cells</param>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>the partially filled list of candidates</returns>
        public List<int> getBlockNumbers(double row, double col, ref List<int> candidates, ref int[][] sudoku)
        {
            // calculate start coordinates for the block of the cell
            // when row = 5:
            //          ┌─┐ = row % 3 = 2, so 5 - 2 = 3 and this is the start col index of the block   
            //  0 1 2 3 4 5   
            // ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢
            double blockStartCol = row - (row % 3);
            // same as the above but for row number
            double blockStartRow = col - (col % 3);

            // iterate through all block cells from top left postion of the current block
            for (int r = (int)blockStartCol; r < blockStartCol + 3; r++)
            {
                for (int c = (int)blockStartRow; c < blockStartRow + 3; c++)
                {
                    candidates.Remove(sudoku[r][c]);
                }
            }

            return candidates;
        }

        /// <summary>
        /// return all candidates in the row containing the current cell
        /// </summary>
        /// <param name="row">the row number of the current cell</param>
        /// <param name="candidates">list of possibly previously calculated cells</param>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>the partially filled list of candidates</returns>
        public List<int> getRowNumbers(int row, ref List<int> candidates, ref int[][] sudoku)
        {
            for (int i = 0; i < 9; i++)
            {
                candidates.Remove(sudoku[row][i]);
            }

            return candidates;
        }

        /// <summary>
        /// return all candidates in the column containing the current cell
        /// </summary>
        /// <param name="col">the column number of the current cell</param>
        /// <param name="candidates">list of possibly previously calculated cells</param>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>the partially filled list of candidates</returns>
        public List<int> getColumnNumbers(int col, ref List<int> candidates, ref int[][] sudoku)
        {
            for (int i = 0; i < 9; i++)
            {
                candidates.Remove(sudoku[i][col]);
            }

            return candidates;
        }

        /// <summary>
        /// get all the candidates for the row, column and block containing the current cell
        /// </summary>
        /// <param name="row">the row number of the current cell</param>
        /// <param name="col">the column number of the current cell</param>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>the complete list of candidates</returns>
        public List<int> getAllCandidates(int row, int col, ref int[][] sudoku)
        {
            List<int> candidates = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            getBlockNumbers(row, col, ref candidates, ref sudoku);
            getRowNumbers(row, ref candidates, ref sudoku);
            getColumnNumbers(col, ref candidates, ref sudoku);

            return candidates;
        }

        /// <summary>
        /// Returns the sole candidate if there is only one
        /// </summary>
        /// <param name="row">the row number of the current cell</param>
        /// <param name="col">the column number of the current cell</param>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>the sole candidate value</returns>
        public int soleCandidate(int row, int col, ref int[][] sudoku)
        {
            //get all candidates for the current cell
            List<int> candidates = getAllCandidates(row, col, ref sudoku);

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            return 0;
        }

        /// <summary>
        /// The method that solves the sudoku logically
        /// </summary>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>the solved sudoku</returns>
        public int[][] solveLogical(ref int[][] sudoku)
        {
            //check if there are cells solved and keep track of this using two variables
            int changedA = 0, changedB = 1;

            //while not equal
            while (changedA != changedB)
            {
                //set them equal, later when they are still equal everything has been solved
                changedA = changedB;

                //iterate trough sudoke from top left to bottom right
                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        //when the cell is empty try solve
                        if (sudoku[r][c] == 0)
                        {
                            //get a single candidate
                            int candidate = soleCandidate(r, c, ref sudoku);

                            //if the candidate is valid
                            if (candidate > 0)
                            {
                                sudoku[r][c] = candidate;
                                //we just solved one so keep track of that for later iteration
                                changedB++;
                            }
                        }
                    }
                }
            }

            return sudoku;
        }

        //not used...
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

        /// <summary>
        /// Recursive function based on guessing numbers picked from possible candidates
        /// </summary>
        /// <param name="row">the row number of the current cell</param>
        /// <param name="col">the column number of the current cell</param>
        /// <param name="sudoku">reference of the sudoku</param>
        /// <returns>false if the the call couldnt guess the right number. True if it can or the cell is already filled</returns>
        public bool SolveGuessingRecursion(int row, int col, ref int[][] sudoku)
        {
            //DateTime.Now;
            //Debug.WriteLine(new String('=', (row * 9) + col + 1) + " " + DateTime.Now);

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
                List<int> candidates = new List<int>(getAllCandidates(row, col, ref sudoku));

                //if there are no candidates return to previous function call, nothing to guess here
                if (candidates.Count == 0)
                    return false;

                //if there are candidates, walk through them in a linear fashion
                foreach (var guess in candidates)
                {
                    //lets try out a guess from candidates
                    sudoku[row][col] = guess;

                    //now lets move on to the next cell and get some work done.
                    //not the random gap addition this allows for generating sudokus
                    //When eventually succesfull please pass on the good news to our ancestor function call
                    if (SolveGuessingRecursion(row, col + 1, ref sudoku))
                        // its solved let it cascade up the call stack
                        return true;
                }
                //end of our guesses, lets restore the current cell to 0,
                //well be back with different guesses 
                sudoku[row][col] = 0;
            }
            //if the current cell is not empty, please leave it alone, its the part of the 
            //sudoku we are trying to solve, move on to the next cell please!
            //not the random gap addition this allows for generating sudokus
            else if (SolveGuessingRecursion(row, col + 1, ref sudoku))
                // its solved let it cascade up the call stack
                return true;

            //well it seemes we didnt solve anything here, better luck next time..., 
            //go up the call chain and see what we can do up there
            return false;
        }

        /// <summary>
        /// Fills in an empty sudoku and then shuffles the rows and columns per block and also 
        /// the block rows and columns themselves
        /// </summary>
        /// <param name="sudoku">the sudoku as reference</param>
        /// <returns>boolean succes</returns>
        public bool ShuffleSudoku (ref int[][] sudoku)
        {
            //fill all the cells
            SolveGuessingRecursion(0, 0, ref sudoku);

            //keep track of random picks
            int rndInt = 0;

            /// <example>
            /// a column    a block column
            /// ┌─┐         ┌─────┐
            /// ╔═╤═╤═╦═╤═╤═╦═╤═╤═╗ ┐
            /// ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢ │ a block row
            /// ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢ │
            /// ╠═╪═╪═╬═╪═╪═╬═╪═╪═╣ ┘
            /// ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢ ┐ a row
            /// ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢ ┘
            /// ╠═╪═╪═╬═╪═╪═╬═╪═╪═╣
            /// ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢
            /// ╟─┼─┼─╫─┼─┼─╫─┼─┼─╢
            /// ╚═╧═╧═╩═╧═╧═╩═╧═╧═╝
            /// </example>
            //shuffle columns per block columns
            for (int i = 2; i >= 0; i--)
            {
                for (int j = 2; j >= 0; j--)
                {
                    rndInt = rnd.Next(0, j);
                    int colIndex = (i * 3) + j;//last column in current column block
                    int colRnd = (i * 3) + rndInt;//random column pick in current column block
                    for (int k = 0; k < 9; k++)// shuffle the cells in all 9 rows that are in the same column
                    {
                        int colSwap = sudoku[k][colIndex];
                        sudoku[k][colIndex] = sudoku[k][colRnd];
                        sudoku[k][colRnd] = colSwap;
                    }
                }
            }

            //shuffle block columns
            for (int i = 2; i >= 0; i--)
            {
                rndInt = rnd.Next(0, i);

                for (int j = 2; j >= 0; j--)
                {
                    int colIndex = (i * 3) + j;
                    int colRnd = (rndInt * 3) + j;
                    for (int k = 0; k < 9; k++)
                    {
                        int colSwap = sudoku[k][colIndex];
                        sudoku[k][colIndex] = sudoku[k][colRnd];
                        sudoku[k][colRnd] = colSwap;
                    }
                }
            }

            //shuffle rows per blockrows
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

            //shuffle blockrows
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

            return true;
        }

        /// <summary>
        /// A list of shuffled indices later to be used for random deletion of cell values
        /// </summary>
        /// <returns>the shuffled list of random indices</returns>
        public List<int> createShuffledSudokuIndices()
        {
            //keep track of random picks;
            int rndInt = 0;

            //create list of int from 1 to 81
            // 1 2 3 4 5 ... 77 78 79 80 81
            List<int> randomIndices = new List<int>();
            for (int i = 0; i < 81; i++)
            {
                randomIndices.Add(i);
            }

            //shuffle twice
            for (int j = 0; j < 2; j++)
            {
                //shuffle them and later use them for cell coordinates
                //we start wich 81 numbers shuffled and later convert them to indices and
                // 27 56 13 8 ... 42 35 67 2 18
                for (int i = randomIndices.Count - 1; i > 0; i--)
                {
                    rndInt = rnd.Next(0, i);

                    //swap last (i) item with random (rndInt) item
                    int swap = randomIndices[i];
                    randomIndices[i] = randomIndices[rndInt];
                    randomIndices[rndInt] = swap;
                }
            }

            return randomIndices;
        }

        public int[][] Solve(int[][] sudoku)
        {

            return solveLogical(ref sudoku);
        }

        public int[][] SolveGuessing(int[][] sudoku)
        {
            SolveGuessingRecursion(0, 0, ref sudoku);
            return sudoku;
        }

        // note the parameter with random object reference, this allows for different random numbers
        // through the entire program. The Random object is initialized in the view Sudoku.cshtml
        public int[][] CreateGuessing(int[][] sudoku, Random rnd)
        {
            ShuffleSudoku(ref sudoku);

            // the amount cells we want to keep
            int remainingCells = 13;

            // keep track of the cell coordinates we are going to zero in random order
            // [0]{ 1 , 6 }
            // [1]{ 4 , 0 }
            // [2]{ 3 , 2 }
            // ...
            List<int> randomIndices = createShuffledSudokuIndices();

            //walk trough all 81 (9*9) (shuffled) numbers to see wich cell can or cant be zeroed
            for (int i = 0; i < randomIndices.Count - remainingCells; i++)
            {
                //cell number to coordinates
                int row = Convert.ToInt32(Math.Floor(Convert.ToDecimal(randomIndices[i] / 9)));
                int col = randomIndices[i] % 9;

                sudoku[row][col] = 0;
            }
            
            return sudoku;
        }

        // note the parameter with random object reference, this allows for different random numbers
        // through the entire program. The Random object is initialized in the view Sudoku.cshtml
        public int[][] Create(int[][] sudoku, Random rnd)
        {
            ShuffleSudoku(ref sudoku);
            List<int> randomIndices = createShuffledSudokuIndices();

            //keep track of the cell coordinates we are going to zero in random order
            // [0]{ 1 , 6 }
            // [1]{ 4 , 0 }
            // [2]{ 3 , 2 }
            // ...
            List<int[]> randomCells = new List<int[]>();

            //walk trough all 81 (9*9) (shuffled) numbers to see wich cell can or cant be zeroed
            foreach (var item in randomIndices)
            {
                //cell number to coordinates
                int row = Convert.ToInt32(Math.Floor(Convert.ToDecimal(item / 9)));
                int col = item % 9;

                //remember the (current) value we might have to put back if the sudoku cant be solved 
                int cellValue = sudoku[row][col];
                //make current cell zero
                sudoku[row][col] = 0;

                //remember the coordinates (instead of cell number) for the current cell 
                int[] coord = new int[2] { row, col};
                randomCells.Add(coord);

                //lets give the logical solve a it a try 
                sudoku = solveLogical(ref sudoku);

                //check for empty cells, since we know wich cells we erased we can
                //walk through them (in reverse order, since the last cells we tried are usually zero, but not always!!), 
                //if we have an empty cell, restore current cell and remove it from randomCells list,
                // also break from the loop, we know enough
                foreach (var co in randomCells.Reverse<int[]>())
                {
                    if (sudoku[co[0]][co[1]] == 0)
                    {
                        sudoku[row][col] = cellValue;
                        //remove the last cell from randomCells since it shouldnt be zero
                        randomCells.RemoveAt(randomCells.Count - 1);
                        break;
                    }
                }
                //restore the checked cells that can and should be 0
                foreach (var co in randomCells)
                {
                    sudoku[co[0]][co[1]] = 0;
                }
            }

            //well what can we say...
            return sudoku;
        }
    }
}