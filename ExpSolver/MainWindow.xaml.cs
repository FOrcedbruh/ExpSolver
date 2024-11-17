using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NCalc;


namespace ExpSolver
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            string input = EquationInput.Text;
            try
            {
                List<double> roots = SolveEquation(input);
                ResultText.Text = $"Корни: {string.Join(", ", roots)}";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private List<double> SolveEquation(string equation)
        {
            
            if (equation.EndsWith("= 0"))
            {
                equation = equation.Substring(0, equation.Length - 4);
            }

           
            int index = 0;
            while ((index = equation.IndexOf("x^", index)) != -1)
            {
                
                int start = index + 2; 
                int end = start;

               
                while (end < equation.Length && char.IsDigit(equation[end]))
                {
                    end++;
                }

                
                string power = equation.Substring(start, end - start);

                
                equation = equation.Remove(index, end - index); 
                equation = equation.Insert(index, $"Pow(x, {power})");

               
                index += $"Pow(x, {power})".Length;
            }

            
            List<double> roots = new List<double>();
            double[] initialGuesses = { 3, -3 };
            double tolerance = 1e-7;
            int maxIterations = 100;

            foreach (var x0 in initialGuesses)
            {
                double x = x0;
                int iterations = 0;

                while (iterations < maxIterations)
                {
                    double fx = EvaluateEquation(equation, x);
                    double dfx = EvaluateDerivative(equation, x);

                    if (Math.Abs(fx) < tolerance)
                    {
                        
                        if (!roots.Contains(x))
                        {
                            roots.Add(x);
                        }
                        break; 
                    }

                    if (Math.Abs(dfx) < tolerance)
                    {
                       
                        x += 0.1; 
                        continue; 
                    }

                    x = x - fx / dfx; 
                    iterations++;
                }
            }

            if (roots.Count == 0)
            {
                throw new Exception("Корень не найден за максимальное количество итераций.");
            }

            return roots;
        }

        private double EvaluateEquation(string equation, double x)
        {
            var e = new NCalc.Expression(equation);
            e.Parameters["x"] = x;
            return (double)e.Evaluate();
        }

        private double EvaluateDerivative(string equation, double x)
        {
           
            double h = 1e-5;
            return (EvaluateEquation(equation, x + h) - EvaluateEquation(equation, x)) / h;
        }
    }
}
