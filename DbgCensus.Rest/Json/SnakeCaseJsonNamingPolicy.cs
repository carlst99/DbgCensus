using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DbgCensus.Rest.Json
{
    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            StringBuilder sb = new();
            Queue<int> wordBoundaries = new();

            char? previousLetter = null;
            for (var i = 0; i < name.Length; i++)
            {
                char letter = name[i];

                if (previousLetter is not null && char.IsUpper(previousLetter.Value) && char.IsLower(letter))
                    wordBoundaries.Enqueue(i - 1);

                if (previousLetter is not null && char.IsLower(previousLetter.Value) && char.IsUpper(letter))
                    wordBoundaries.Enqueue(i);

                previousLetter = letter;
            }

            wordBoundaries.TryDequeue(out int nextWordBoundary);
            for (int i = 0; i < name.Length; i++)
            {
                char letter = name[i];

                if (i == nextWordBoundary)
                {
                    wordBoundaries.TryDequeue(out nextWordBoundary);

                    if (i != 0)
                        sb.Append('_');
                }

                sb.Append(char.ToLowerInvariant(letter));
            }

            return sb.ToString();
        }
    }
}
