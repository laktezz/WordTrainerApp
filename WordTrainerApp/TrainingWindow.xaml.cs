using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WordTrainerApp
{
    public partial class TrainingWindow : Window
    {
        // Делегат для события
        public delegate void TrainingCompletedEventHandler(object sender, (int CorrectAnswers, int WrongAnswers) results);

        // Событие, которое будет вызываться при завершении тренировки
        public event TrainingCompletedEventHandler TrainingCompleted;
        private Language SelectedLanguage;
        private Word CurrentWord;
        private List<string> Options;
        private List<(string ForeignWord, string UserAnswer, string CorrectAnswer)> IncorrectAnswers = new();
        private Random Random = new();

        public int WrongAnswers => IncorrectAnswers.Count;
        private List<WordCategory> SelectedCategories;

        public int CorrectAnswers { get; private set; }

        public TrainingWindow(Language selectedLanguage, MainWindow mainWindow, List<WordCategory> selectedCategories)
        {
            InitializeComponent();
            SelectedLanguage = selectedLanguage;

            // Используем только выбранные категории
            SelectedCategories = selectedCategories;
            LoadNextWord();
        }

        private void LoadNextWord()
        {
            // Выбираем случайную категорию из выбранных пользователем
            var category = SelectedCategories[Random.Next(SelectedCategories.Count)];

            // Выбираем случайное слово из этой категории
            CurrentWord = category.Words[Random.Next(category.Words.Count)];

            Options = SelectedLanguage.Categories
                .Where(c => SelectedCategories.Contains(c)) // Отбираем только выбранные категории
                .SelectMany(c => c.Words)
                .Select(w => w.Translation)
                .OrderBy(x => Random.Next())
                .Take(4)
                .ToList();

            if (!Options.Contains(CurrentWord.Translation))
                Options[Random.Next(Options.Count)] = CurrentWord.Translation;

            WordTextBlock.Text = CurrentWord.ForeignWord;
            Option1Button.Content = Options[0];
            Option2Button.Content = Options[1];
            Option3Button.Content = Options[2];
            Option4Button.Content = Options[3];
        }


        private void CheckAnswer(string selectedOption)
        {
            if (selectedOption == CurrentWord.Translation)
            {
                CorrectAnswers++;
                MessageBox.Show("Правильно!");
            }
            else
            {
                // Добавляем в список неправильных ответов
                IncorrectAnswers.Add((CurrentWord.ForeignWord, selectedOption, CurrentWord.Translation));
                MessageBox.Show($"Неправильно! Правильный ответ: {CurrentWord.Translation}");
            }

            // Проверяем, достигли ли мы 5 правильных ответов
            if (CorrectAnswers >= 5)
            {
                // Вызываем событие с корректными параметрами
                TrainingCompleted?.Invoke(this, (CorrectAnswers, WrongAnswers));

                // Показываем неправильные ответы
                ShowIncorrectAnswers();
                Close(); // Закрываем окно
            }
            else
            {
                LoadNextWord(); // Загружаем следующее слово
            }
        }
        private void ShowIncorrectAnswers()
        {
            if (IncorrectAnswers.Count == 0)
            {
                MessageBox.Show("Вы ответили правильно на все вопросы!");
                return;
            }

            string message = $"Вы сделали {IncorrectAnswers.Count} ошибок.\n\nНеправильные ответы:\n\n";
            foreach (var answer in IncorrectAnswers)
            {
                message += $"Слово: {answer.ForeignWord}, Ваш ответ: {answer.UserAnswer}, Правильный ответ: {answer.CorrectAnswer}\n";
            }
            MessageBox.Show(message, "Результаты");

        }


        private void Option1Button_Click(object sender, RoutedEventArgs e) => CheckAnswer(Options[0]);
        private void Option2Button_Click(object sender, RoutedEventArgs e) => CheckAnswer(Options[1]);
        private void Option3Button_Click(object sender, RoutedEventArgs e) => CheckAnswer(Options[2]);
        private void Option4Button_Click(object sender, RoutedEventArgs e) => CheckAnswer(Options[3]);
    }
}
