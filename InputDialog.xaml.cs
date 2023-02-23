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
using System.Windows.Shapes;

namespace projektWisielec {
    public partial class InputDialog : Window {
        public InputDialog(string prompt) {
            InitializeComponent();
            PromptTextBlock.Text = prompt;
        }

        public string Result { get; private set; }

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            Result = InputTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
