using System.Collections.Generic;
using System.Windows;

namespace WordTrainerApp
{
    public partial class CategorySelectionWindow : Window
    {
        public List<WordCategory> SelectedCategories { get; private set; }

        public CategorySelectionWindow(List<WordCategory> categories)
        {
            InitializeComponent();
            CategoryListBox.ItemsSource = categories; // Заполняем список категорий
            CategoryListBox.DisplayMemberPath = "CategoryName"; // Отображаем только названия категорий
            SelectedCategories = new List<WordCategory>();
        }

        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in CategoryListBox.SelectedItems)
            {
                SelectedCategories.Add((WordCategory)item);
            }

            if (SelectedCategories.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну категорию.");
                return;
            }

            DialogResult = true; // Устанавливаем результат диалога
            Close();
        }
    }

}