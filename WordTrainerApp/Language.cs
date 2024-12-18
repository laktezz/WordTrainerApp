using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTrainerApp
{
    public class Language
    {
        public string Name { get; set; } // Название языка
        public List<WordCategory> Categories { get; set; }

        public Language() => Categories = new List<WordCategory>();
        
    }

}
