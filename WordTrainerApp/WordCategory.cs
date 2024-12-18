using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTrainerApp
{
    public class WordCategory
    {
        public string CategoryName { get; set; }
        public List<Word> Words { get; set; }

        public WordCategory() => Words = new List<Word>();
        

        // Переопределяем метод ToString
        public override string ToString() => CategoryName; // Возвращаем название категории
        
    }
}
