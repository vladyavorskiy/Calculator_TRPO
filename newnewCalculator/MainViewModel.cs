using CommunityToolkit.Mvvm.Input;
using newnewTestCalculator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace newnewCalculator
{
    public partial class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private Calculator _calculator;
        private readonly IMemory _memory;

        private string _selectedExpression;


        private ObservableCollection<string> _memoryList = new ObservableCollection<string>();
        public ObservableCollection<string> MemoryList
        {
            get => _memoryList;
            set => SetField(ref _memoryList, value);
        }
        
        public MainViewModel(IMemory memory)
        {
            _calculator = new Calculator();
            _memory = memory;

            string numbers = "0123456789";

            MemoryList = new ObservableCollection<string>();



            AddPlusToMemoryCommand = new RelayCommand<string>(x =>
            {
                _memory.PutP(Result);
            });

            AddMinusToMemoryCommand = new RelayCommand<string>(x =>
            {
                _memory.PutM(Result);
            });

            ReadMemoryCommand = new RelayCommand<string>(x =>
            {
                Result = _memory.GetExpressions(Result);

                MemoryList.Clear();
                var expressions = _memory.GetAllExpressions(null);
                MemoryList = new ObservableCollection<string>(expressions.Split('\n').Select(expression => expression.Trim()));

            });

            CleanMemoryCommand = new RelayCommand<string>(x =>
            {
                _memory.Delete();
            });



            NumberButtonCommand = new RelayCommand<string>(x =>
            {
                int indexInf = Result.IndexOf("∞");
                int index = Result.IndexOf("=");
                if (index == -1 && indexInf == -1)
                {
                    Result += x;
                }
                else
                {
                    Result = "";
                    Result += x;
                }
            });


            OperatorButtonCommand = new RelayCommand<string>(x =>
            {
                int indexInf = Result.IndexOf("∞");
                int index = Result.IndexOf("=");
                if (index == -1 && indexInf == -1)
                {
                    Result += x;
                }
                else if (index != -1 && indexInf == -1)
                {
                    Result = Result.Substring(index + 1);
                    Result += x;
                }
                else
                {
                    Result = "";
                    Result += x;
                }
            });


            DeleteButtonCommand = new RelayCommand<string>(x =>
            {
                if (!string.IsNullOrEmpty(Result))
                {
                    Result = Result.Substring(0, Result.Length - 1);
                }
            }, x => string.IsNullOrWhiteSpace(Result) == false);


            DeleteAllButtonCommand = new RelayCommand<string>(x =>
            {
                Result = "";
            }, x => string.IsNullOrWhiteSpace(Result) == false);


            EqualsButtonCommand = new RelayCommand<string>(x =>
            {
                Result = Result + "=" + _calculator.Calculate(Result);
                _memory.Put(Result);
            }, x => (string.IsNullOrWhiteSpace(Result) || Result.IndexOf("=") != -1 || _errors.Count > 0) == false);


            SelectMemoryItemCommand = new RelayCommand<string>(x =>
            {
                Result = x;
            });
        }



        private string _result = "";
        public string Result
        {
            get => _result;
            set
            {
                if (_calculator.Calculate(value) == "Expression error")
                    _errors[nameof(Result)] = "ошибка";


                if (_errors.Count > 0 && _calculator.Calculate(value) != "Expression error")
                {
                    var keyToRemove = _errors.FirstOrDefault(x => x.Value == "ошибка").Key;
                    _errors.Remove(keyToRemove);
                }


                if (_result == value) return;
                _result = value;
                OnPropertyChanged(nameof(Result));

                EqualsButtonCommand.NotifyCanExecuteChanged();
                DeleteButtonCommand.NotifyCanExecuteChanged();
                DeleteAllButtonCommand.NotifyCanExecuteChanged();
                AddPlusToMemoryCommand.NotifyCanExecuteChanged();
                AddMinusToMemoryCommand.NotifyCanExecuteChanged();
            }
        }
        public RelayCommand<string> SelectMemoryItemCommand { get; }



        public RelayCommand<string> NumberButtonCommand { get; }
        public RelayCommand<string> OperatorButtonCommand { get; }
        public RelayCommand<string> EqualsButtonCommand { get; }
        public RelayCommand<string> DeleteButtonCommand { get; }
        public RelayCommand<string> DeleteAllButtonCommand { get; }
        public RelayCommand<string> AddPlusToMemoryCommand { get; }
        public RelayCommand<string> AddMinusToMemoryCommand { get; }
        public RelayCommand<string> ReadMemoryCommand { get; }
        public RelayCommand<string> CleanMemoryCommand { get; }



        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine, _errors.Values);
            }
        }

        public string this[string columnName]
        {
            get
            {
                return _errors.TryGetValue(columnName, out var value) ? value : string.Empty;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
