using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace projektWisielec
{

    /*
    TODO:
    - Change image of hangman depending on lives
    - Add images of hangman
    - "GuessesTextBlock" show all guesses (add each guess to textBlock)

    DONE: 
    - "Bot" btn: random word from preset
    - "2 Players" btn: new dialog with prompt to enter word
      - Input dialog
    - "reset" btn: reset game
    - Add more words to preset
    
    SUGGESTIONS:
    - add simple setings like:
      - *number* of total lives (1-12)
      - random letter from word each *number* guesses 
      
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
        private List<string> guesses = new List<string>();
        public int lives { get; private set; } = 11;

        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;


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
            WordTextBlock.Text = "";
            int count = 0;
            foreach (char letter in currentWord) {
                WordTextBlock.Text += ((letter == ' ') ? " " : "_") + " ";
                if (letter != ' ') count++;
            }
            WordTextBlock.Text += $" ({count})";

            correctLetters.Clear();
            lives = 10;
            UpdateResults();
        }

        private void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text.Trim().ToLower();

            guesses.Add(input);
            GuessesTextBlock.Text = string.Join(", \n", guesses);

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
                    UpdateResults();
                    if (lives == 0)
                    {
                        ShowMessage("You lost!\nWord: " + currentWord);
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
                    UpdateResults();
                    if (lives == 0)
                    {
                        ShowMessage("You lost!\nWord: " + currentWord);
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
                if (correctLetters.Contains(currentWord[i]) || currentWord[i] == ' ')
                {
                    word[i] = currentWord[i];
                }
                else
                {
                    word[i] = '_';
                }
            }

            WordTextBlock.Text = "";
            int count = 0;
            foreach (char letter in word) {
                WordTextBlock.Text += ((letter == ' ') ? ' ' : letter) + " ";
                if (letter == '_') count++;
            }
            WordTextBlock.Text += $" ({count})";

        }

        private bool IsGameWon()
        {
            foreach (char c in currentWord)
            {
                if (!correctLetters.Contains(c) && c != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateResults()
        {
            LivesTextBlock.Content = $"Lives: {lives}";
            HangmanImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri($"pack://application:,,,/assets/lives{lives}.png"));
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
            StartGame(words[new Random().Next(words.Count)]);
        }
        
        private void InputWord(object sender, RoutedEventArgs e) {
            InputDialog dialog = new InputDialog("Enter word to guess:");
            if (dialog.ShowDialog() == true) {
                string word = dialog.Result.Trim().ToLower();
                StartGame(word);
            }
            else {
                MessageBox.Show("You didn't enter any text!\nGuess: random word", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                RandomWord(sender, e);
            }
        }

        private void RandomWord(object sender, RoutedEventArgs e) {
            StartGame(words[new Random().Next(words.Count)]);
        }
    }
}
