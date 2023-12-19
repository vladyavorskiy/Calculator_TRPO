using FluentAssertions;
using Xunit;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace newnewTestCalculator
{
    //public class CalculatorTests
    //{
    //    private Calculator _calculator;
    //    public CalculatorTests()
    //    {
    //        _calculator = new Calculator();
    //    }


    //    [Fact]
    //    public void Calculate_ShouldReturnZero_IfSourceIsEmpty()
    //    {
    //        var result = _calculator.Calculate(string.Empty);
    //        result.Should().Be("0");
    //    }


    //    [Theory]
    //    [InlineData("5", "5")]
    //    [InlineData("22", "22")]
    //    [InlineData("0", "0")]
    //    [InlineData("123,5", "123,5")]
    //    [InlineData("123,", "123")]
    //    public void Calculate_ShouldReturnSource_IfSourceContainsOnlyOneNumber(string source, string output)
    //    {
    //        var result = _calculator.Calculate(source);
    //        result.Should().Be(output);
    //    }

    //    [Fact]
    //    public void Calculate_ShouldReturnSourceWithCommas_IfSourceContainsDots()
    //    {
    //        var result = _calculator.Calculate("1.1");
    //        result.Should().Be("1,1");
    //    }


    //    [Theory]
    //    [InlineData("1+2", "3")]
    //    [InlineData("2*3", "6")]
    //    [InlineData("6/3", "2")]
    //    [InlineData("2-4", "-2")]
    //    public void Calculate_ShouldReturnCorrectResultInTypeInteger_IfSourceIsMathOperationsWithTwoInteger(string source, string output)
    //    {
    //        var result = _calculator.Calculate(source);
    //        result.Should().Be(output);
    //    }


    //    [Theory]
    //    [InlineData("1/0", "?")]
    //    [InlineData("1.1+2.2", "3,3")]
    //    [InlineData("1.1*2.2", "2,42")]
    //    [InlineData("3.5-7.32", "-3,82")]
    //    [InlineData("5/3", "1,66666667")]
    //    [InlineData("7.5/14.3", "0,52447552")]
    //    public void Calculation_ShouldReturnCorrectResultInTypeDouble_IfSourceIsMathOperationsWithTwoDouble(string source, string output)
    //    {
    //        var result = _calculator.Calculate(source);
    //        result.Should().Be(output);
    //    }


    //    [Theory]
    //    [InlineData("1+2*(4/2)", "5")]
    //    [InlineData("1*2*3*4*5", "120")]
    //    [InlineData("1*4-3*2", "-2")]
    //    [InlineData("((23*(3-5)-7)-10)*-1", "63")]
    //    public void Calculation_ShouldReturnCorrectResult_IfSourceIsAnExpressionWithPriorityOperatorsInteger(string source, string output)
    //    {
    //        var result = _calculator.Calculate(source);
    //        result.Should().Be(output);
    //    }


    //    [Theory]
    //    [InlineData("1.1+2.2*(4.2/2.3)", "5,11739131")]
    //    [InlineData("1.5*2.5*3.5*4.5*5.5", "324,84375")]
    //    [InlineData("15.2*4.4-3.1*2.4", "59,44")]
    //    [InlineData("((23.4*(3.4-5.6)-77.1)-10.1)*-1.", "138,68")]
    //    public void Calculation_ShouldReturnCorrectResult_IfSourceIsAnExpressionWithPriorityOperatorsDouble(string source, string output)
    //    {
    //        var result = _calculator.Calculate(source);
    //        result.Should().Be(output);
    //    }

    //    [Theory]
    //    [InlineData("2+ab-6", "Expression error")]
    //    [InlineData("2+3(34)", "Expression error")]
    //    [InlineData("1+(5*2", "Expression error")]
    //    [InlineData("1+7)*2", "Expression error")]
    //    [InlineData("1+5+5/(4+3+(3*2)", "Expression error")]

    //    public void Calculation_ShouldReturnError_IfSourceHasErrors(string source, string output)
    //    {
    //        var result = _calculator.Calculate(source);
    //        result.Should().Be(output);
    //    }
    //}

    public class Calculator
    {
        private string res = "";
        private int symbol, max_l = 0;
        private int numb_scobka = 0;


        private void GetSymbol()
        {
            if (max_l <= res.Length - 1)
            {
                symbol = res[max_l];
                max_l++;
            }
            else
                symbol = '\0';
        }

        public string Calculate(string Source)
        {
            string alphabet = "0123456789()+-*/,.=";
            string numbers = "0123456789";
            string signs = "()+-*/,.=";

            res = Source.Replace(" ", "");
            if (res.Length == 0)
            {
                return "0";
            }

            if (res.Length >= 1 && res.All(c => numbers.Contains(c)))
            {
                return res;
            }
            res = res + "=";



            //if (!res.All(c => alphabet.Contains(c)))
            //{
            //    throw new Exception("Expression error");
            //}


            max_l = 0;
            GetSymbol();
            try
            {
                return ProcS().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private double ProcC()
        {
            //string x = "";

            double x = 0;
            double dec_min = 1;

            bool flag = true;
            while (symbol >= '0' && symbol <= '9' || symbol == ',' || symbol == '.')
            {
                if (symbol == ',' || symbol == '.')
                {
                    if(flag == false)
                        throw new Exception("Expression error");
                    flag = false;
                    GetSymbol();
                    //x += ',';
                    continue;
                }

                if (flag == true)
                {
                    x *= 10;
                    x += symbol - '0';
                }
                else
                {
                    dec_min /= 10;
                    x += (symbol - '0') * dec_min;
                }
                //x += (char)symbol;

                GetSymbol();

            }
            //return double.Parse(x);
            return x;
        }


        private double ProcM()
        {
            double x = 0;
            if (symbol == '(')
            {
                numb_scobka++;
                GetSymbol();
                while (symbol == ' ')
                    GetSymbol();
                x = ProcE();
                if (symbol != ')') throw new Exception("Expression error");
                numb_scobka--;
                GetSymbol();
            }
            else if (symbol == ')')
                throw new Exception("Expression error");
            else if (symbol == '-')
            {
                GetSymbol();
                x = -ProcM();
            }
            else if (symbol == '+')
            {
                GetSymbol();
                x = +ProcM();
            }
            else if (symbol >= '0' && symbol <= '9')
                x = ProcC();
            else if (symbol >= 'a' && symbol <= 'z')
                throw new Exception("Expression error");
            else
                throw new Exception("Expression error");

            return x;
        }

        private double ProcT()
        {
            double x = ProcM();
            while (symbol == ' ')
                GetSymbol();
            while (symbol == '*' || symbol == '/')
            {
                char p = (char)symbol;
                GetSymbol();
                while (symbol == ' ')
                    GetSymbol();
                if (p == '*')
                {
                    x *= ProcM();
                    x = Math.Round(x, 8);
                }
                else if (p == '/')
                {
                    x /= ProcM();
                    x = Math.Round(x, 8);
                }
            }
            if ((symbol == ')') && (numb_scobka == 0))
            {
                throw new Exception("Expression error");
            }
            return x;
        }

        private double ProcE()
        {
            double x = ProcT();
            while (symbol == ' ')
                GetSymbol();
            while (symbol == '+' || symbol == '-')
            {
                char p = (char)symbol;
                GetSymbol();
                while (symbol == ' ')
                    GetSymbol();
                if (p == '+')
                {
                    x += ProcT();
                    x = Math.Round(x, 8);
                }
                else if (p == '-')
                {
                    x -= ProcT();
                    x = Math.Round(x, 8);
                }
            }
            if ((symbol == ')') && (numb_scobka == 0))
            {
                throw new Exception("Expression error");
            }
            return x;
        }


        private double ProcS()
        {
            double x = ProcE();
            if (symbol != '=')
                throw new Exception("Expression error");
            return x;
        }



    }
}

