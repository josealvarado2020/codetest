using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assignment
{
    /*
     * ASSUMPTIONS
     * only works for columns A to Z
     * formulaes have to be separated by spaces ie: 2a + 2
     * unexpected value / operations return -1
     * operations gets calculated left to right (FIFO)
     * operations will be completed in format (left operator right) ie 'a2 + 3' -> 'a2 +' will fail
     */
    class Program
    {
        static void Main(string[] args)
        {
            var cols = promptValueFor("columns");
            var rows = promptValueFor("rows");
            var sheet = new string[rows, cols];
            populateSheet(sheet, rows, cols);
            processSheet(sheet, rows, cols);
            Console.WriteLine($"done");
        }


        private static void processSheet(string[,] sheet, int rows, int cols)
        {
            var processsedSheet = new double[rows, cols];
            PrintColumns(cols);
            for (int r = 0; r <= rows - 1; r++)
            {
                var printRow = new StringBuilder();
                printRow.Append($"{r+1}   ");
                for (int c = 0; c <= cols - 1; c++)
                {
                    processsedSheet[r, c] = CalculateValue(sheet[r, c], processsedSheet);

                    printRow.Append($"{processsedSheet[r, c]}   ");

                }
                Console.WriteLine(printRow);
            }
        }

        private static void PrintColumns(int cols)
        {
            var printCols = new StringBuilder();
            printCols.Append($"    ");
            for (int c = 0; c <= cols - 1; c++)
            {
                printCols.Append($"{getColumnLetter(c)}   ");
            }
            Console.WriteLine(printCols);
        }


        private static double CalculateValue(string cell, double[,] sheet)
        {
            var array = cell.Split(" ");
            if (array.Length == 0)
                return -1;
            if (array.Length == 1)
            {
                var containsNumber = Regex.Matches(array[0], @"[a-zA-Z]").ToList();
                if (containsNumber.Any())
                    return GetValueFromCell(array[0], sheet);
                return Convert.ToDouble(array[0]);
            }


            var formulae = new Queue<string>();
            foreach (var x in array)
            {
                formulae.Enqueue(BuildFormString(x, sheet));
            }

            var result = PerformOperation(formulae);
            return result;
        }

        private static double PerformOperation(Queue<string> formulae)
        {
            double totalSoFar = 0;
            bool firstIteration = true;
            while (formulae.Count > 0)
            {
                var l = !firstIteration ? totalSoFar : Convert.ToDouble(formulae.Dequeue());
                var op = formulae.Dequeue();
                var r = Convert.ToDouble(formulae.Dequeue());
                
                if (op.Equals("+"))
                    totalSoFar = l + r;
                else if (op.Equals("-"))
                    totalSoFar = l - r;
                else if (op.Equals("*"))
                    totalSoFar = l * r;
                else if (op.Equals("/"))
                    totalSoFar = Divide(l,r);
                else
                    totalSoFar = 0;
                firstIteration = false;
            }

            return totalSoFar;
        }

        private static double Divide(double num, double den)
        {
            try
            {
                return num / den;
            }
            catch
            {
                return -1;
            }
        }

        private static string BuildFormString(string x, double[,] sheet)
        {
            string[] operators = { "+", "-", "*", "/" };
            if (operators.Contains(x))
                return x;

            if (double.TryParse(x, out double number))
                return number.ToString();

            var cellValue = GetValueFromCell(x, sheet);
            return cellValue.ToString();
        }

        private static double GetValueFromCell(string cell, double[,] sheet)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return -1;
            }
            if (double.TryParse(cell, out double number))
                return number;

            //else retrieve value from cell
            try
            {
                var letter = cell.Substring(0, 1);
                var row = Convert.ToInt32(cell.Substring(1)) - 1;
                var letterNumber = getColumnNumber(letter) - 1;
                var cellvalue = sheet[row, letterNumber];
                return Convert.ToDouble(cellvalue);
            }
            catch
            {
                return -1;
            }
        }

        private static int getColumnNumber(string letter)
        {
            if (string.IsNullOrEmpty(letter))
            {
                throw new Exception("Column cant be empty");
            }

            var character = letter.ToLowerInvariant();
            var x = character[0] - 'a';
            return x + 1;
        }

        private static void populateSheet(string[,] sheet, int rows, int cols)
        {
            for (int r = 0; r <= rows - 1; r++)
            {
                for (int c = 0; c <= cols - 1; c++)
                {
                    Console.WriteLine($"{getColumnLetter(c)}{r + 1}:");
                    var cellTxt = Console.ReadLine();
                    sheet[r, c] = cellTxt;

                }
            }
        }

        public static char getColumnLetter(int input)
        {
            var letter = 'A' + input % ('Z' - 'A');
            return Convert.ToChar(letter);
        }

        public static int promptValueFor(string label)
        {
            Console.WriteLine($"Enter number of {label}:");
            var txt = Console.ReadLine();
            return Convert.ToInt32(txt);
        }
    }
}
