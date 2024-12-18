using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WordTrainerApp
{
    public partial class AddWordWindow : Window
    {
        private List<WordCategory> Categories;

        public AddWordWindow(List<WordCategory> categories)
        {
            InitializeComponent();
            Categories = categories;
            CategoryComboBox.ItemsSource = Categories.Select(c => c.CategoryName).ToList();
        }

        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            string foreignWord = ForeignWordTextBox.Text.Trim();
            string translation = TranslationTextBox.Text.Trim();
            string selectedCategory = CategoryComboBox.SelectedItem?.ToString();

            if (!string.IsNullOrWhiteSpace(foreignWord) &&
                !string.IsNullOrWhiteSpace(translation) &&
                !string.IsNullOrWhiteSpace(selectedCategory))
            {
                // Получаем категорию по выбранному имени (как это делается в вашем другом коде)
                WordCategory selectedCategoryObject = Categories.First(c => c.CategoryName == selectedCategory);

                // Добавляем новое слово с указанием имени категории
                selectedCategoryObject.Words.Add(new Word
                {
                    ForeignWord = foreignWord,
                    Translation = translation,
                    CategoryName = selectedCategoryObject.CategoryName // Присваиваем имя категории
                });

                MessageBox.Show("Слово добавлено!");
                Close();
            }
            else
            {
                MessageBox.Show("Заполните все поля!");
            }
        }


    }
}