using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WordTrainerApp
{
    public partial class MainWindow : Window
    {
        private LanguageManager LanguageManager;
        private UserScore CurrentUserScore;
        private Dictionary<string, UserScore> UserScores = new Dictionary<string, UserScore>();
        public MainWindow()
        {
            InitializeComponent();
            LanguageManager = new LanguageManager();
            UpdateLanguageListBox();
        }

        // Обновление списка языков
        private void UpdateLanguageListBox()
        {
            LanguageListBox.Items.Clear();
            foreach (var language in LanguageManager.Languages)
            {
                LanguageListBox.Items.Add(language.Name);
            }
        }

        // Обновление списка категорий
        private void UpdateCategoryListBox(Language selectedLanguage)
        {
            CategoryListBox.Items.Clear();
            foreach (var category in selectedLanguage.Categories)
            {
                CategoryListBox.Items.Add(category.CategoryName);
            }
        }

        // Добавить язык
        private void AddLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            var languageName = Microsoft.VisualBasic.Interaction.InputBox("Введите название языка:", "Добавить Язык");
            if (!string.IsNullOrWhiteSpace(languageName))
            {
                LanguageManager.Languages.Add(new Language { Name = languageName });
                UpdateLanguageListBox();
            }
        }

        // Добавить категорию
        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите язык для добавления категории.");
                return;
            }

            var categoryName = Microsoft.VisualBasic.Interaction.InputBox("Введите название категории:", "Добавить Категорию");
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var selectedLanguage = LanguageManager.Languages[LanguageListBox.SelectedIndex];
                selectedLanguage.Categories.Add(new WordCategory { CategoryName = categoryName });
                UpdateCategoryListBox(selectedLanguage);
            }
        }

        // Обработчик выбора языка
        private void LanguageListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageListBox.SelectedItem != null)
            {
                var selectedLanguage = LanguageManager.Languages[LanguageListBox.SelectedIndex];
                UpdateCategoryListBox(selectedLanguage);
            }
        }

        // Сохранение языков
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить данные",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                DefaultExt = ".json",
                FileName = "languages.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Сохраняем языки
                LanguageManager.SaveLanguages(saveFileDialog.FileName);

                // Сохраняем результаты в отдельный файл
                var scoreFileName = saveFileDialog.FileName.Replace("languages.json", "scores.json");
                LanguageManager.SaveScores(scoreFileName, UserScores);

                MessageBox.Show("Данные успешно сохранены!");
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Загрузить данные",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Проверяем, является ли загружаемый файл языковым или рекордов
                if (openFileDialog.FileName.EndsWith("languages.json"))
                {
                    // Загружаем языки
                    LanguageManager.LoadLanguages(openFileDialog.FileName);
                    MessageBox.Show("Языки успешно загружены!");
                    UpdateLanguageListBox();
                }
                else if (openFileDialog.FileName.EndsWith("scores.json"))
                {
                    // Загружаем результаты
                    UserScores = LanguageManager.LoadScores(openFileDialog.FileName);
                    MessageBox.Show("Рекорды успешно загружены!");

                    // Очищаем ListBox перед добавлением новых результатов
                    ResultsListBox.Items.Clear();

                    // Сортируем загруженные результаты по убыванию
                    var sortedScores = UserScores.Values
                        .OrderByDescending(score => score.BestScore)
                        .ToList();

                    // Добавляем отсортированные результаты в ListBox
                    foreach (var score in sortedScores)
                    {
                        var resultEntry = $"Имя: {score.UserName}, Правильных ответов: {score.BestScore}, Неправильных ответов: {score.WrongAnswers}";
                        ResultsListBox.Items.Add(resultEntry);
                    }
                }
                else
                    MessageBox.Show("Выберите файл с языками или рекордами.");
            }
        }

        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите язык для тренировки.");
                return;
            }

            string userName = Microsoft.VisualBasic.Interaction.InputBox("Введите ваше имя:", "Имя пользователя");
            if (string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show("Имя не может быть пустым.");
                return;
            }

            if (!UserScores.TryGetValue(userName, out CurrentUserScore))
            {
                CurrentUserScore = new UserScore { UserName = userName, BestScore = 0 };
                UserScores[userName] = CurrentUserScore;
            }

            var selectedLanguage = LanguageManager.Languages[LanguageListBox.SelectedIndex];

            // Открываем окно выбора категорий
            var categorySelectionWindow = new CategorySelectionWindow(selectedLanguage.Categories);
            if (categorySelectionWindow.ShowDialog() == true)
            {
                var selectedCategories = categorySelectionWindow.SelectedCategories;

                // Проверяем, были ли выбраны категории
                if (selectedCategories == null || selectedCategories.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы одну категорию для тренировки.");
                    return;
                }

                // Запускаем тренировку с выбранными категориями
                var trainingWindow = new TrainingWindow(selectedLanguage, this, selectedCategories);
                trainingWindow.TrainingCompleted += TrainingWindow_TrainingCompleted; // Подписка на событие
                trainingWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Тренировка отменена.");
            }
        }


        // Обработчик события завершения тренировки
        private void TrainingWindow_TrainingCompleted(object sender, (int CorrectAnswers, int WrongAnswers) results)
        {
            // Обновляем лучший результат
            if (results.CorrectAnswers > CurrentUserScore.BestScore)
                CurrentUserScore.BestScore = results.CorrectAnswers;

            // Сохраняем количество неправильных ответов
            CurrentUserScore.WrongAnswers = results.WrongAnswers;

            // Обновляем отображение результатов
            UpdateBestResultDisplay();
        }


        private void UpdateBestResultDisplay()
        {
            // Очищаем ListBox перед добавлением новых результатов
            ResultsListBox.Items.Clear();

            // Сортируем результаты по убыванию количества правильных ответов
            var sortedScores = UserScores.Values
                .OrderByDescending(score => score.BestScore)
                .ToList();

            // Добавляем отсортированные результаты в ListBox
            foreach (var score in sortedScores)
            {
                var resultEntry = $"Имя: {score.UserName}, Правильных ответов: {score.BestScore}, Неправильных ответов: {score.WrongAnswers}";
                ResultsListBox.Items.Add(resultEntry);
            }
        }


        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите язык.");
                return;
            }

            if (CategoryListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            // Получаем выбранный язык и категорию
            var selectedLanguage = LanguageManager.Languages[LanguageListBox.SelectedIndex];
            var selectedCategory = selectedLanguage.Categories[CategoryListBox.SelectedIndex];

            // Ввод иностранного слова
            var foreignWord = Microsoft.VisualBasic.Interaction.InputBox("Введите иностранное слово:", "Добавить Слово");
            if (string.IsNullOrWhiteSpace(foreignWord)) return;

            // Ввод перевода
            var translation = Microsoft.VisualBasic.Interaction.InputBox("Введите перевод слова:", "Добавить Слово");
            if (string.IsNullOrWhiteSpace(translation)) return;

            // Создание нового объекта Word
            var newWord = new Word
            {
                ForeignWord = foreignWord,
                Translation = translation,
                CategoryName = selectedCategory.CategoryName // Устанавливаем правильное значение категории
            };

            // Добавление слова в категорию
            selectedCategory.Words.Add(newWord);
            MessageBox.Show($"Слово '{foreignWord}' добавлено в категорию '{selectedCategory.CategoryName}'.");
        }

        private void OpenWordListButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите язык.");
                return;
            }

            if (CategoryListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            var selectedLanguage = LanguageManager.Languages[LanguageListBox.SelectedIndex];
            var selectedCategory = selectedLanguage.Categories[CategoryListBox.SelectedIndex];

            var wordListWindow = new WordListWindow(selectedCategory);
            wordListWindow.ShowDialog();
        }
        private void ClearResultsButton_Click(object sender, RoutedEventArgs e) => ResultsListBox.Items.Clear();
        
    }
}