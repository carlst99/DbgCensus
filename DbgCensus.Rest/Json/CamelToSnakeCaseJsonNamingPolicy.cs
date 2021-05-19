using System.Text;
using System.Text.Json;

namespace DbgCensus.Rest.Json
{
    public class CamelToSnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            StringBuilder sb = new();

            foreach (char letter in name)
            {
                if (char.IsUpper(letter) && sb.Length != 0)
                    sb.Append('_');

                sb.Append(char.ToLowerInvariant(letter));
            }

            return sb.ToString();
        }
    }
}
