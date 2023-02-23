using System;
using System.Collections.Generic;
using System.Windows;

namespace projektWisielec
{
    public partial class MainWindow : Window
    {
        private List<string> words = new List<string>()
        {
            "abstrakcja",
            "dyskrecja",
            "fotografia",
            "programowanie",
            "komputer",
            "klawiatura",
            "mikrofon",
            "telewizja"
        };

        private string currentWord;
        private List<char> correctLetters = new List<char>();
        private int lives = 6;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void StartGame(string word)
        {
            Random random = new Random();
            currentWord = words[random.Next(words.Count)];
            WordTextBlock.Text = new string('_', currentWord.Length);
            correctLetters.Clear();
            lives = 6;
            UpdateResultTextBlock();
        }

        private void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            if (input.Length == 1)
            {
                if (currentWord.Contains(input[0]))
                {
                    correctLetters.Add(input[0]);
                    UpdateWordTextBlock();
                    if (IsGameWon())
                    {
                        ShowMessage("You won!");
                    }
                }
                else
                {
                    lives--;
                    UpdateResultTextBlock();
                    if (lives == 0)
                    {
                        ShowMessage("You lost!");
                    }
                }
            }
            else
            {
                if (currentWord == input)
                {
                    WordTextBlock.Text = currentWord;
                    ShowMessage("You won!");
                }
                else
                {
                    lives--;
                    UpdateResultTextBlock();
                    if (lives == 0)
                    {
                        ShowMessage("You lost!");
                    }
                }
            }

            InputTextBox.Clear();
        }

        private void UpdateWordTextBlock()
        {
            char[] word = new char[currentWord.Length];
            for (int i = 0; i < currentWord.Length; i++)
            {
                if (correctLetters.Contains(currentWord[i]))
                {
                    word[i] = currentWord[i];
                }
                else
                {
                    word[i] = '_';
                }
            }
            WordTextBlock.Text = new string(word);
        }

        private bool IsGameWon()
        {
            foreach (char c in currentWord)
            {
                if (!correctLetters.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateResultTextBlock()
        {
            LivesTextBlock.Content = $"Lives: {lives}";
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
