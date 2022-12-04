using System;
using System.Collections.Generic;
using System.Linq;

namespace _37SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Task 37");
            //SolveSudoku(new char[][] {
            //    new char [9] { '5', '3', '.', '.', '7', '.', '.', '.', '.' },
            //    new char [9] { '6', '.', '.', '1', '9', '5', '.', '.', '.' },
            //    new char [9] { '.', '9', '8', '.', '.', '.', '.', '6', '.' },
            //    new char [9] { '8', '.', '.', '.', '6', '.', '.', '.', '3' },
            //    new char [9] { '4', '.', '.', '8', '.', '3', '.', '.', '1' },
            //    new char [9] { '7', '.', '.', '.', '2', '.', '.', '.', '6' },
            //    new char [9] { '.', '6', '.', '.', '.', '.', '2', '8', '.' },
            //    new char [9] { '.', '.', '.', '4', '1', '9', '.', '.', '5' },
            //    new char [9] { '.', '.', '.', '.', '8', '.', '.', '7', '9' }
            //    });

            SolveSudoku(new char[][] {
                new char [9] { '.','.','9','7','4','8','.','.','.' },
                new char [9] { '7','.','.','.','.','.','.','.','.' },
                new char [9] { '.','2','.','1','.','9','.','.','.' },
                new char [9] { '.','.','7','.','.','.','2','4','.' },
                new char [9] { '.','6','4','.','1','.','5','9','.' },
                new char [9] { '.','9','8','.','.','.','3','.','.' },
                new char [9] { '.','.','.','8','.','3','.','2','.' },
                new char [9] { '.','.','.','.','.','.','.','.','6' },
                new char [9] { '.','.','.','2','7','5','9','.','.' }
                });

            //SolveSudoku(new char[][] {
            //    new char [9] { '.','.','.','2','.','.','.','6','3' },
            //    new char [9] { '3','.','.','.','.','5','4','.','1' },
            //    new char [9] { '.','.','1','.','.','3','9','8','.' },
            //    new char [9] { '.','.','.','.','.','.','.','9','.' },
            //    new char [9] { '.','.','.','5','3','8','.','.','.' },
            //    new char [9] { '.','3','.','.','.','.','.','.','.' },
            //    new char [9] { '.','2','6','3','.','.','5','.','.' },
            //    new char [9] { '5','.','3','7','.','.','.','.','8' },
            //    new char [9] { '4','7','.','.','.','1','.','.','.' }
            //    });

            Console.ReadLine();
        }

        // https://leetcode.com/problems/sudoku-solver/
        // 37. Sudoku Solver
        // Write a program to solve a Sudoku puzzle by filling the empty cells.
        // A sudoku solution must satisfy all of the following rules:
        // Each of the digits 1-9 must occur exactly once in each row.
        // Each of the digits 1-9 must occur exactly once in each column.
        // Each of the digits 1-9 must occur exactly once in each of the 9 3x3 sub-boxes of the grid.
        // The '.' character indicates empty cells.

        public struct place
        {
            public int row; // строка
            public int col; // столбец
            public char[] maybe; // возможные числа (1...9)
            public int used;
        }
        static public void SolveSudoku(char[][] board)
        {
            Console.WriteLine();
            Console.WriteLine("================= Поехали ==================");
            printFiled(board); // выводим начальное поле
            Console.WriteLine();

            // набор для заполнения строки, столбца и блока
            char[] nums = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            // поворотный массив
            char[][] boardY = RotateField(board);
            // массив блоков 3х3
            char[][] boardBox = BoxField(board);

            Stack<char[][]> mems = new Stack<char[][]>(); // храним поля для возврата
            Stack <place> places = new Stack<place>();

            long iter = 0;
            bool work; // работаем
            bool win; // победа
            
            bool err = false; // ошибка ранее выбранного варианта
            int Level = 0;
            place pl = new place();

            bool unclear = false; // есть более одного варианта
            int minSelect = 99;
            
            do
            {
                iter++;
                Console.WriteLine($"           начало прохода {iter} <<<");
                work = false;
                win = true;

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (err == false && board[row][col] == '.') // если нет ошибок и требуется заполнение
                        {
                            win = false; // значит еще не победа
                            List<char> dic = new List<char>(); // набор всех использованных чисел
                            int x = row / 3;
                            int y = col / 3;
                            dic.AddRange(board[row]);
                            dic.AddRange(boardY[col]);
                            dic.AddRange(boardBox[x * 3 + y]);
                            dic.RemoveAll(x => x == '.');
                            List<char> dicUsed = dic.Distinct().ToList(); // набор уникальных использованных чисел
                            var dicMaybe = nums.Except(dicUsed).OrderBy(x => x).ToList(); // набор допустимых (оставшихся) вариантов
                            var MBCount = dicMaybe.Count(); // количество оставшихся вариантов

                            // если допустимый вариант [ один ]  или  [ больше одного, но мы можем сделать выбор ]
                            if (MBCount == 1 || (MBCount > 0 && unclear && row == pl.row && col == pl.col))
                            {
                                int dicIndex = 0; // индекс допустимого варианта
                                if (MBCount > 1) // если более одного, то 
                                {
                                    dicIndex = pl.used; // выбираем по очереди
                                    Console.WriteLine($" Выбор сделан. index = {dicIndex}");
                                }

                                // заполняем
                                board[row][col] = dicMaybe[dicIndex];
                                boardY[col][row] = dicMaybe[dicIndex];
                                var bx = row - x * 3;
                                var by = col - y * 3;
                                boardBox[x * 3 + y][bx * 3 + by] = dicMaybe[dicIndex];
                                Console.WriteLine($" адрес[{row}][{col}] = ({dicMaybe.Count()}) = {dicMaybe[dicIndex]}");
                                
                                // служебные переменные
                                err = false;
                                work = true;
                                unclear = false;
                            }
                            else if (MBCount == 0) // если вариантов нет - выбор был сделан неверно (или судоку не решаемое)
                            {
                                err = true;
                                Console.WriteLine($" адрес[{row}][{col}] = Ошибка!");
                            }
                            else
                            {
                                // наилучшие критерии для работы с вариантами (>1)
                                if (!unclear && MBCount > 1 && MBCount <= minSelect)
                                {
                                    minSelect = MBCount;
                                    pl = new place();
                                    pl.row = row;
                                    pl.col = col;
                                    pl.maybe = dicMaybe.ToArray();
                                    pl.used = 0;
                                }
                            }

                        }
                    }
                }
                
                // -----------------------------------------------------------------------------

                Console.WriteLine("           конец прохода >>>");
                if (minSelect <= 9)
                    Console.WriteLine($" Варианты = {minSelect}, адрес [{pl.row}][{pl.col}] = ({String.Join(',', pl.maybe)}) index = {pl.used}");
                Console.WriteLine();
                printFiled(board); // выводим поле в конце прохода
                Console.WriteLine();

                if (err) // если была ошибка, то начинаем с момента сохранения
                {
                    Console.WriteLine($" Во время прохода выявлена ошибка");
                    if (mems.Count > 0)
                    {
                        Console.WriteLine($" Загружаем сохраненку. Уровень {Level}");

                        board = mems.Pop();
                        boardY = RotateField(board);
                        boardBox = BoxField(board);

                        pl = places.Pop();
                        pl.used++;
                        Console.WriteLine($" адрес[{pl.row}][{pl.col}] = ({String.Join(',', pl.maybe)}) index = {pl.used}");

                        if (pl.used < pl.maybe.Length)
                        {
                            mems.Push(board);
                            places.Push(pl);

                            unclear = true;
                            err = false;
                            Console.WriteLine();
                            printFiled(board); // выводим загруженное поле
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine(" Эта сохраненка исчерпала себя, идем назад");
                            Level--;
                            err = true;
                        }
                        work = true;
                    }
                }
                else if (work == false && win == false) // если работу продолжить нельзя и это еще не победа, то...
                {
                    // у нас проблема...
                    unclear = true;
                    Level++;

                    places.Push(pl); // запоминание точки
                    mems.Push(CopyArray(board)); // запоминаем состояние доски

                    Console.WriteLine($" Есть варианты");
                    Console.WriteLine($" Уровень = {Level}, адрес[{pl.row}][{pl.col}] = ({String.Join(',',pl.maybe)}) index = {pl.used}");

                    // и продолжаем работу
                    work = true;

                } else
                {
                    minSelect = 99;
                }
            } while (work == true);

            // ----------- Вывод результата -----------
            Console.WriteLine();
            Console.WriteLine($"<<< Победа? = {win} >>> Ошибка? = {err}");
            Console.WriteLine();

        }

        static private void printFiled(char[][] board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Console.Write($" {board[row][col]}");
                    if ((col + 1) % 3 == 0 && col != 8)
                        Console.Write(" |");
                }
                Console.WriteLine();
                if ((row + 1) % 3 == 0 && row != 8)
                    Console.WriteLine("----------------------");
            }
        }

        static private void printForBox(char[][] boardBox)
        {
            for (int row = 0; row < 9; row++)
            {
                Console.Write(row + " > ");
                for (int col = 0; col < 9; col++)
                {
                    Console.Write(boardBox[row][col]);
                }
                Console.WriteLine();
            }
        }


        static private char[][] RotateField(char[][] board)
        {
            // поворотный массив
            char[][] boardY = new char[][] {
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
            };

            // заполняем поворотный массив
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    boardY[row][col] = board[col][row];
                }
            }
            return boardY;
        }

        static private char[][] BoxField(char[][] board)
        {
            // массив блоков 3х3
            char[][] boardBox = new char[][] {
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
            };

            // заполняем массив блоков 3х3
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var startRow = (row) * 3;
                    var startCol = (col) * 3;
                    boardBox[row * 3 + col] = new char[] {
                        board[startRow][startCol], board[startRow][startCol+1], board[startRow][startCol+2],
                        board[startRow+1][startCol], board[startRow+1][startCol+1], board[startRow+1][startCol+2],
                        board[startRow+2][startCol], board[startRow+2][startCol+1], board[startRow+2][startCol+2]
                    };

                }
            }

            return boardBox;
        }

        static private char[][] CopyArray(char[][] board)
        {
            char[][] copyBoard = new char[][] {
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9],
                new char[9]
            };
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    copyBoard[row][col] = board[row][col];
                }
            }
            return copyBoard;
        }

    }
}
