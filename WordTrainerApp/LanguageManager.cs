using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WordTrainerApp;

public class LanguageManager
{
    public List<Language> Languages { get; set; } = new List<Language>();

    // Сохранение языков, категорий и слов
    public void SaveLanguages(string filePath)
    {
        var json = JsonConvert.SerializeObject(Languages, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    // Загрузка языков, категорий и слов
    public void LoadLanguages(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            Languages = JsonConvert.DeserializeObject<List<Language>>(json) ?? new List<Language>();
        }
        else
            Languages = new List<Language>();
    }

    // Сохранение и загрузка результатов
    public void SaveScores(string filePath, Dictionary<string, UserScore> userScores)
    {
        var json = JsonConvert.SerializeObject(userScores, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public Dictionary<string, UserScore> LoadScores(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, UserScore>>(json);
        }
        return new Dictionary<string, UserScore>();
    }
}