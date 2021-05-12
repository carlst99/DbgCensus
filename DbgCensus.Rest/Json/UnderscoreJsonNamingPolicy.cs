using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DbgCensus.Rest.Json
{
    public class UnderscoreJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            StringBuilder sb = new StringBuilder();
            List<string> words = new();
            string currentWord = string.Empty;

            foreach (char letter in name)
            {
                if (char.IsUpper(letter) && !currentWord.Equals(string.Empty))
                {
                    words.Add(currentWord);
                    currentWord = string.Empty;
                }

                currentWord += letter;
            }

            return string.Join('_', words);
        }
    }
}
