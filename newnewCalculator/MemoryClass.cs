using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace newnewCalculator
{
    public interface IMemory
    {
        void PutP(string expression);
        void PutM(string expression);
        string GetExpressions(string expression);
        void Delete();
    }

    class RAMMemory : IMemory
    {
        private string _memory = "";
        public void PutP(string expression)
        {
            string numbers = "0123456789,";

            if (string.IsNullOrWhiteSpace(expression) == false && (expression.IndexOf("=") != -1 || expression.All(c => numbers.Contains(c)) == true)){
                int index = expression.IndexOf("=");
                if (index == -1)
                {
                    if (_memory != "")
                        _memory = (double.Parse(("+" + expression).Replace(" ", "")) + double.Parse(_memory)).ToString();
                    else
                        _memory = (double.Parse(("+" + expression).Replace(" ", ""))).ToString();
                }
                else
                {
                    string mem = expression.Substring(index + 1);
                    if (_memory != "")
                        _memory = (double.Parse(("+" + mem).Replace(" ", "")) + double.Parse(_memory)).ToString();
                    else
                        _memory = (double.Parse(("+" + mem).Replace(" ", ""))).ToString();
                }
            }
        }

        public void PutM(string expression)
        {
            string numbers = "0123456789,";

            if (string.IsNullOrWhiteSpace(expression) == false && (expression.IndexOf("=") != -1 || expression.All(c => numbers.Contains(c)) == true)){
                int index = expression.IndexOf("=");
                if (index == -1)
                {
                    if (_memory != "")
                        _memory = (double.Parse(("-" + expression).Replace(" ", "")) + double.Parse(_memory)).ToString();
                    else
                        _memory = (double.Parse(("-" + expression).Replace(" ", ""))).ToString();
                }
                else
                {
                    string mem = expression.Substring(index + 1);
                    if (_memory != "")
                        _memory = (double.Parse(("-" + mem).Replace(" ", "")) + double.Parse(_memory)).ToString();
                    else
                        _memory = (double.Parse(("-" + mem).Replace(" ", ""))).ToString();

                }
            }
        }

        public string GetExpressions(string expression)
        {
            int index = expression.IndexOf("=");
            if (string.IsNullOrWhiteSpace(expression) == true)
            {
                return _memory;
            }
            else if (index == -1)
            {
                if (Regex.IsMatch(expression, @".*[+-/*(]$"))
                    return (expression + _memory).Replace(" ", "");
                else
                    return expression;
            }
            else
            {
                return expression;
            }
        }

        public void Delete()
        {
            _memory = "";
        }

    }




    class FileMemory : IMemory
    {
        private string _filePath = "C:\\Users\\Vladislav Yavorskiy\\Desktop\\ТРПО\\newnewTestCalculator\\newnewCalculator\\memoryFileBD.txt";
        bool IsVisible = false;

        public void PutP(string expression)
        {
            File.AppendAllLines(_filePath, new[] { expression });
        }

        public void PutM(string expression)
        {
            // Implement if necessary
        }

        public string GetExpressions(string expression)
        {
            if (IsVisible == false)
            {
                IsVisible = true;
                if (File.Exists(_filePath))
                {
                    string[] expressions = File.ReadAllLines(_filePath);
                    return string.Join(Environment.NewLine, expressions);
                }
                return "";
            }
            else
            {
                IsVisible = false;
                return "";
            }

        }

        public void Delete()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }








    class BDMemory : IMemory
    {
        private const string connectionString = "Data Source=Memory.db;Version=3;";
        bool IsVisible = false;


        public void PutP(string expression)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {

                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Expressions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Expression TEXT)";
                    cmd.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "INSERT INTO Expressions (Expression) VALUES (@expression)";
                    cmd.Parameters.AddWithValue("@expression", expression);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void PutM(string expression)
        {
            
        }
        public string GetExpressions(string expression)
        {
            List<string> expressions = new List<string>();


            if (IsVisible == false)
            {
                IsVisible = true;
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT Expression FROM Expressions", connection))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                expressions.Add(reader["Expression"].ToString());
                            }
                        }
                    }
                }

                // Возвращает строки, разделенные новой строкой
                return string.Join(Environment.NewLine, expressions);
            }
            else
            {
                IsVisible = false;
                return "";
            }
            
        }

        public void Delete()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Expressions", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }


    

}
