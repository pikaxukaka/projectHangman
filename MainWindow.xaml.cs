using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace projektWisielec
{

    /*
    TODO:
    - "Bot" btn: random word from preset
    - "2 Players" btn: new dialog with prompt to enter word
    - "reset" btn: reset game - DONE
    - "GuessesTextBlock" - show all guesses
    - Change image of hangman depending on lives
    - Add more words to preset - DONE
    - Add images of hangman

    */
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
        private int lives = 10;

        public MainWindow()
        {
            InitializeComponent();
            StartGame("abstrakcja");

            try
            {
                string fileName = "words.txt";
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        words.Add(line.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File read error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void StartGame(string word)
        {
            currentWord = word;
            WordTextBlock.Text = new string('_', currentWord.Length);    
            correctLetters.Clear();
            lives = 10;
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
                        Restart();
                    }
                }
                else
                {
                    lives--;
                    UpdateResultTextBlock();
                    if (lives == 0)
                    {
                        ShowMessage("You lost!");
                        Restart();
                    }
                }
            }
            else
            {
                if (currentWord == input)
                {
                    WordTextBlock.Text = currentWord;
                    ShowMessage("You won!");
                    Restart();
                }
                else
                {
                    lives--;
                    UpdateResultTextBlock();
                    if (lives == 0)
                    {
                        ShowMessage("You lost!");
                        Restart();
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

        private void RestartClick(object sender, RoutedEventArgs e)
        {
            Restart();
        }

        private void Restart()
        {
            Random random = new Random();
            currentWord = words[random.Next(words.Count)];
            WordTextBlock.Text = new string('_', currentWord.Length);
            correctLetters.Clear();
            lives = 10;
            UpdateResultTextBlock();
            InputTextBox.Clear();
        }
    }
}
