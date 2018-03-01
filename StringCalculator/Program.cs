using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace StringCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class CalculatorTest
    {
        [Fact]
        public void ShouldReturnZeroForEmptyString()
        {
            var calculator = new Calculator();

            var result = calculator.Evaluate("");

            Assert.Equal(0, result);
        }

        [Fact]
        public void GivenAConstantThatValueIsReturned()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("34");
            Assert.Equal(34, result);
        }

        [Fact]
        public void GivenAConstantDoubleValueIsReturned()
        {
            var calculator = new Calculator();

            var result = calculator.Evaluate("24.5");
            Assert.Equal(24.5m, result);
        }

        [Fact]
        public void AddTogetherIfStrongContainsAPlus()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("10+25");
            Assert.Equal(35m, result);
        }

        [Fact]
        public void ThenReturnsCorrectValueForSubtractingValues()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("10-10");
            Assert.Equal(0m, result);
        }


        [Fact]
        public void ThenReturnsCorrectValueForMultiplyingValues()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("1.5*2");
            Assert.Equal(3m, result);
        }



        [Fact]
        public void ThenReturnsCorrectValueForMultiplyingThreeValues()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("1.5*2*2");
            Assert.Equal(6m, result);
        }

        [Fact]
        public void ThenReturnsCorrectValueForDividingValues()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("1.2/2");
            Assert.Equal(0.6m, result);
        }

        [Fact]
        public void ThenReturnsCorrectValueForPowers()
        {
            var calculator = new Calculator();
            var result = calculator.Evaluate("3^3");
            Assert.Equal(27m, result);
        }

        [Fact]
        public void ThenReturnsCorrectValueForMultipleDivisions()
        {

            var calculator = new Calculator();
            var result = calculator.Evaluate("12/2/3");
            Assert.Equal(2m, result);
        }


        [Fact]
        public void ThenReturnsCorrectValueForUnaryMinus()
        {

            var calculator = new Calculator();
            var result = calculator.Evaluate("-2+3");
            Assert.Equal(1m, result);
        }



    }


    //public class Expr
    //{
    //    public int Op { ++get; set; }
    //    public Expr L, R;
    //    public int Val = 0;
    //        public static Expr Parse(string s)
    //        {
    //            string tail;
    //            if (s.Length > 0)
    //            {
    //                Expr t;
    //                if (s[0] >= '0' && s[0] <= '9')
    //                {
                        
    //                    return Terminal(s, out tail);
    //                }
    //                else
    //                {
    //                    var l = Parse(s, out tail);
    //                    int o = s[0];
    //                    switch (o)
    //                    {
    //                        case '+': case '-': break;
    //                        case '*': case '/': break;
    //                    }
    //                }
                    
    //            }
                
    //        }

    //}

    public class Calculator
    {
        public Dictionary<char, Func<decimal, decimal, decimal>> OperatorMap = new Dictionary<char, Func<decimal, decimal, decimal>>()
        {
            {'+', (a,b) => a+b },
            {'-', (a,b) => a-b },
            {'*', (a,b) => a*b },
            {'/', (a,b) => a/b },
            {'^', (a,b) => (decimal) Math.Pow((double)a, (double)b)},
        };

        public decimal Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return 0;
            }

            if (expression.ElementAtOrDefault(0) == '-')
            {
                expression = "0" + expression;
            }

            return Worker(expression);
        }

        private decimal Worker(string expression)
        {
            var value = expression.Split(OperatorMap.Keys.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (value.Length == 1)
            {
                return decimal.Parse(expression);
            }
            
            
            var op = expression[expression.Substring(1).IndexOfAny(OperatorMap.Keys.ToArray()) + 1];
            var opFunc = OperatorMap[op];
            if (value.Length == 2)
            {
                return opFunc(Convert.ToDecimal(value[0]), Convert.ToDecimal(value[1]));
            }

            var op2 = expression[expression.LastIndexOfAny(OperatorMap.Keys.ToArray())];
            
            return Worker(opFunc(Convert.ToDecimal(value[0]), Convert.ToDecimal(value[1])) + op2.ToString() + value[2]);




            if (expression.Contains("/"))
            {
                var sploit2 = expression.Split('/');
                if (sploit2.Length > 1)
                {
                    if (sploit2.Length > 2)
                    {
                        return Evaluate( Evaluate(string.Join("/", sploit2.Take(sploit2.Length-1))) +"/" + sploit2.Last());
                    }
                }
                return decimal.Parse(sploit2[0]) / decimal.Parse(sploit2[1]  );
            }

            var split = expression.Split('^');
            if (split.Length > 1)
            {
                int multiplier = int.Parse(split[1]);

                expression = String.Join("*", Enumerable.Repeat(split[0], multiplier));
            }


            split = expression.Split('*');

            if (split.Length > 1)
            {
                if (split.Length > 2)
                {
                    return Evaluate(split[0] + "*" 
                       + Evaluate(string.Join("*", split.Skip(1))));
                }

                int multiplier = int.Parse(split[1]);

                expression = String.Join("+", Enumerable.Repeat(split[0], multiplier));

            }

            expression = expression.Replace("-", "+-");

            return expression.Split(new []{'+'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(decimal.Parse)
                .Sum(x => x);
        }
    }
}
