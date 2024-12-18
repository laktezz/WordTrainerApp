using System.Windows;

namespace WordTrainerApp
{
    public partial class WordListWindow : Window
    {
        private WordCategory SelectedCategory;

        public WordListWindow(WordCategory category)
        {
            InitializeComponent();
            SelectedCategory = category;
            UpdateWordList();
        }

        private void UpdateWordList()
        {
            WordListBox.Items.Clear();
            foreach (var word in SelectedCategory.Words)
            {
                WordListBox.Items.Add($"{word.ForeignWord} - {word.Translation}");
            }
        }

        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            var foreignWord = Microsoft.VisualBasic.Interaction.InputBox("Введите иностранное слово:", "Добавить слово");
            if (string.IsNullOrWhiteSpace(foreignWord))
            {
                MessageBox.Show("Слово не может быть пустым.");
                return;
            }

            var translation = Microsoft.VisualBasic.Interaction.InputBox("Введите перевод слова:", "Добавить слово");
            if (string.IsNullOrWhiteSpace(translation))
            {
                MessageBox.Show("Перевод не может быть пустым.");
                return;
            }

            // Устанавливаем категорию для нового слова из выбранной категории
            SelectedCategory.Words.Add(new Word
            {
                ForeignWord = foreignWord,
                Translation = translation,
                CategoryName = SelectedCategory.CategoryName // Устанавливаем имя категории
            });

            UpdateWordList();
        }

        private void EditWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (WordListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите слово для редактирования.");
                return;
            }

            var selectedIndex = WordListBox.SelectedIndex;
            var selectedWord = SelectedCategory.Words[selectedIndex];

            var newForeignWord = Microsoft.VisualBasic.Interaction.InputBox("Редактировать иностранное слово:", "Редактировать слово", selectedWord.ForeignWord);
            if (!string.IsNullOrWhiteSpace(newForeignWord))
                selectedWord.ForeignWord = newForeignWord;

            var newTranslation = Microsoft.VisualBasic.Interaction.InputBox("Редактировать перевод:", "Редактировать слово", selectedWord.Translation);
            if (!string.IsNullOrWhiteSpace(newTranslation))
                selectedWord.Translation = newTranslation;
            UpdateWordList();
        }

        private void DeleteWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (WordListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите слово для удаления.");
                return;
            }

            var selectedIndex = WordListBox.SelectedIndex;
            SelectedCategory.Words.RemoveAt(selectedIndex);
            UpdateWordList();
        }
    }
}
