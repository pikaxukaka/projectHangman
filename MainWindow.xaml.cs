using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace projektWisielec {

    public partial class MainWindow : Window {
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

        private int totalLives = 11;
        private int guessesCount = 0;
        public int randomLetter { get; private set; } = 0;
        public int lives { get; private set; } = 11;

        private string lang { get; set; } = "pl";

        public MainWindow() {
            InitializeComponent();
            this.DataContext = this;

            LoadWords();
        }

        private void StartGame(string word) {
            GuessButton.IsEnabled = true;
            InputTextBox.IsEnabled = true;
            guesses.Clear();
            GuessesTextBlock.Text = "";
            lives = totalLives;

            currentWord = word.ToUpper();
            WordTextBlock.Text = "";
            int count = 0;
            foreach (char letter in currentWord) {
                WordTextBlock.Text += ((letter == ' ') ? " " : "_") + " ";
                if (letter != ' ') count++;
            }
            WordTextBlock.Text += $" ({count})";

            correctLetters.Clear();
            UpdateResults();
        }
        
        private void LoadWords() {
            try {
                string fileName = "words-" + lang + ".txt";
                using (StreamReader sr = new StreamReader(fileName)) {
                    words.Clear();
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        words.Add(line.Trim());
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"File read error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RandomLetter(string input) {

            List<char> letters = new List<char>();
            foreach (char letter in currentWord) {
                if (!letters.Contains(letter) && letter != ' ') {
                    letters.Add(letter);
                }
            }

            if (randomLetter != 0 && correctLetters.Count < letters.Count) {
                if (guessesCount++ % randomLetter == 0) {
                    Random random = new Random();
                    int index = random.Next(0, currentWord.Length);

                    while (correctLetters.Contains(currentWord[index]) || currentWord[index] == ' ' || currentWord[index] == input[0]) {
                        index = random.Next(0, currentWord.Length);
                    }

                    correctLetters.Add(currentWord[index]);
                    if (IsGameWon()) {
                        correctLetters.Remove(currentWord[index]);
                    }
                    else {
                        UpdateGuessesTextBlock(currentWord[index].ToString(), Brushes.Cyan);
                        UpdateWordTextBlock();
                    }
                }
            }
        }

        private void GuessButton_Click(object sender, RoutedEventArgs e) {

            string input = InputTextBox.Text.Trim().ToUpper();
            if (guesses.Contains(input) || string.IsNullOrEmpty(input)) {
                return;
            }

            if (input.Length == 1) {
                if (currentWord.Contains(input[0])) {
                    correctLetters.Add(input[0]);
                    UpdateGuessesTextBlock(input, Brushes.Green);
                    UpdateWordTextBlock();

                    if (IsGameWon()) {
                        ShowMessage("You won!");
                        Restart();
                    }
                    else
                        RandomLetter(input);
                }
                else {
                    lives--;
                    UpdateGuessesTextBlock(input, Brushes.Red);
                    UpdateResults();

                    if (lives == 0) {
                        ShowMessage("You lost!\nWord: " + currentWord);
                        Restart();
                    }
                    else
                        RandomLetter(input);
                }
            }
            else {
                if (currentWord == input) {
                    UpdateGuessesTextBlock(input, Brushes.Green);
                    WordTextBlock.Text = currentWord;
                    ShowMessage("You won!");
                    Restart();
                }
                else {
                    lives--;
                    UpdateGuessesTextBlock(input, Brushes.Red);
                    UpdateResults();
                    if (lives == 0) {
                        ShowMessage("You lost!\nWord: " + currentWord);
                        Restart();
                    }
                    else
                        RandomLetter(input);
                }
            }

            InputTextBox.Clear();
        }

        private void UpdateGuessesTextBlock(string text, Brush color) {
            guesses.Add(text);
            Run run = new Run() { Foreground = color };
            run.SetValue(Run.TextProperty, text);
            GuessesTextBlock.Inlines.Add(run);
            GuessesTextBlock.Inlines.Add(new LineBreak());
        }

        private void UpdateWordTextBlock() {
            char[] word = new char[currentWord.Length];
            for (int i = 0; i < currentWord.Length; i++) {
                if (correctLetters.Contains(currentWord[i]) || currentWord[i] == ' ') {
                    word[i] = currentWord[i];
                }
                else {
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

        private bool IsGameWon() {
            foreach (char c in currentWord) {
                if (!correctLetters.Contains(c) && c != ' ') {
                    return false;
                }
            }
            return true;
        }

        private void UpdateResults() {
            LivesTextBlock.Content = $"Lives: {lives}";
            HangmanImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri($"pack://application:,,,/assets/lives{lives}.png"));
        }

        private void ShowMessage(string message) {
            MessageBox.Show(message, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RestartClick(object sender, RoutedEventArgs e) {
            Restart();
        }

        private void Restart() {
            StartGame(words[new Random().Next(words.Count)]);

        }

        private void InputWord(object sender, RoutedEventArgs e) {
            InputDialog dialog = new InputDialog("Enter word to guess:");

            if (dialog.ShowDialog() == true) {
                string word = dialog.Result.Trim().ToUpper();

                if (word.Length > 0) {
                    StartGame(word);
                    return;
                }
            }

            MessageBox.Show("You didn't enter any text!\nGuess: random word", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            RandomWord(sender, e);
        }

        private void RandomWord(object sender, RoutedEventArgs e) {
            StartGame(words[new Random().Next(words.Count)]);
        }

        private void LivesUpdate(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (int.TryParse(TotalLives.Text, out int totalLivesInput)) {
                if (totalLivesInput >= 1 && totalLivesInput <= 10) {
                    totalLives = totalLivesInput;
                }
                else {
                    totalLives = 10;
                }
            }
        }

        private void RandomUpdate(object sender, System.Windows.Controls.TextChangedEventArgs e) {

            if (int.TryParse(RandomLetters.Text, out int RandomLettersInput)) {
                if (RandomLettersInput >= 0 && RandomLettersInput <= 10) {
                    randomLetter = RandomLettersInput;
                }
                else {
                    randomLetter = 0;
                }
            }
        }

        private void Credits(object sender, RoutedEventArgs e) {
            MessageBox.Show("Authors: \n\nPatryk Kozaczkiewicz\nMateusz Kowalski", "Credits", MessageBoxButton.OK);
        }

        private void LangBtnClick(object sender, RoutedEventArgs e) {
            LangPlSelected.Visibility = Visibility.Hidden;
            LangEnSelected.Visibility = Visibility.Hidden;
            LangEsSelected.Visibility = Visibility.Hidden;
            LangGrSelected.Visibility = Visibility.Hidden;

            LangPl.Opacity = 1;
            LangEn.Opacity = 1;
            LangEs.Opacity = 1;
            LangGr.Opacity = 1;

            if (LangPl.IsChecked == true) {
                lang = "pl";
                LangPlSelected.Visibility = Visibility.Visible;
                LangPl.Opacity = 0.75;
            }
            else if (LangEn.IsChecked == true) {
                lang = "en";
                LangEnSelected.Visibility = Visibility.Visible;
                LangPl.Opacity = 0.75;
            }
            else if (LangEs.IsChecked == true) {
                lang = "es";
                LangEsSelected.Visibility = Visibility.Visible;
                LangPl.Opacity = 0.75;
            }
            else if (LangGr.IsChecked == true) {
                lang = "gr";
                LangGrSelected.Visibility = Visibility.Visible;
                LangPl.Opacity = 0.75;
            }

            LoadWords();
        }
    }
}
