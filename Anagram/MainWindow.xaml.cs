using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Anagram.Annotations;

namespace Anagram
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WORDLIST_FOLDER = "lists";
        private static readonly Regex numOnly = new Regex("[^0-9]+");
        private static readonly Regex lettersOnly = new Regex("[^a-zA-Z]");
        private readonly MyContext dataValues = new MyContext();
        private string currentlyLoadedList;
        private List<string> currentWordlist;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = dataValues;
            if (Directory.Exists("lists"))
            {
                foreach (string filename in Directory.EnumerateFiles("lists"))
                {
                    if (filename.EndsWith(".txt"))
                    {
                        dataValues.Wordlists.Add(filename.Substring(6));
                    }
                }
            }

            dataValues.InputText = "hotel kalifornien";
            dataValues.NumWords = 1;
            dataValues.MinLength = 1;
            dataValues.MaxLength = 12;
        }

        private static bool IsTextAllowed(string text) { return !numOnly.IsMatch(text); }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string) e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void NumbersOnly(object sender, TextCompositionEventArgs e) { e.Handled = !IsTextAllowed(e.Text); }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("wordlist:  ").AppendLine(dataValues.SelectedWordlist);
            buf.Append("inputtext: ").AppendLine(dataValues.InputText);
            buf.Append("numwords:  ").AppendLine(dataValues.NumWords.ToString());
            buf.Append("minlength: ").AppendLine(dataValues.MinLength.ToString());
            buf.Append("maxlength: ").AppendLine(dataValues.MaxLength.ToString());
            buf.AppendLine("---------------------------");

            if (string.IsNullOrWhiteSpace(dataValues.SelectedWordlist))
            {
                buf.AppendLine("ERROR - No Wordlist selected");
                return;
            }

            if (!dataValues.SelectedWordlist.Equals(currentlyLoadedList))
            {
                if (File.Exists(Path.Combine(WORDLIST_FOLDER, dataValues.SelectedWordlist)))
                {
                    currentWordlist = new List<string>();
                    currentWordlist.AddRange(File.ReadLines(Path.Combine(WORDLIST_FOLDER, dataValues.SelectedWordlist)));
                    currentlyLoadedList = dataValues.SelectedWordlist;
                }
                else
                {
                    buf.AppendLine("ERROR - Wordlist does not exist");
                    return;
                }
            }

            string inputCharacters = lettersOnly.Replace(dataValues.InputText,"");
            buf.Append("cleaned characters: ").AppendLine(inputCharacters);
            buf.Append("alphabetized: ").AppendLine(SortCharacters(inputCharacters));
            buf.AppendLine();
            buf.AppendLine("<generated> - <leftover characters>");
            buf.AppendLine("-----------------------------------");
            buf.AppendLine();

            List<string> foundWords = new List<string>();

            // TODO

            dataValues.Output = buf.ToString();
        }

        private static string SortCharacters(string input)
        {
            string inputCharacters = lettersOnly.Replace(input, "");
            char[] inputArray = inputCharacters.ToCharArray();
            Array.Sort(inputArray);
            return string.Join("", inputArray);
        }

        public class MyContext : INotifyPropertyChanged
        {
            public List<string> Wordlists
            {
                get { return wordlists; }
                set
                {
                    wordlists = value;
                    NotifyPropertyChanged(nameof(Wordlists));
                }
            }

            public string SelectedWordlist
            {
                get { return selectedWordlist; }
                set
                {
                    selectedWordlist = value;
                    NotifyPropertyChanged(nameof(SelectedWordlist));
                }
            }

            public string InputText
            {
                get { return inputText; }
                set
                {
                    inputText = value;
                    NotifyPropertyChanged(nameof(InputText));
                }
            }

            public int NumWords
            {
                get { return numWords; }
                set
                {
                    numWords = value;
                    NotifyPropertyChanged(nameof(NumWords));
                }
            }

            public int MinLength
            {
                get { return minLength; }
                set
                {
                    minLength = value;
                    NotifyPropertyChanged(nameof(MinLength));
                }
            }

            public int MaxLength
            {
                get { return maxLength; }
                set
                {
                    maxLength = value;
                    NotifyPropertyChanged(nameof(MaxLength));
                }
            }

            public string Output
            {
                get { return output; }
                set
                {
                    output = value;
                    NotifyPropertyChanged(nameof(Output));
                }
            }

            private string inputText;
            private int maxLength;
            private int minLength;
            private int numWords;
            private string output;

            private string selectedWordlist;
            private List<string> wordlists = new List<string>();
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}