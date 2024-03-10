using Business.Models.Exceptions;
using Common.Constants.Messages;
using System.Text.Json;

namespace SuperBarber.Extensions.DataLoaders
{
    public static class DataLoader
    {
        private const string DataLoaderPath = @"Extensions\DataLoaders";

        public static IReadOnlyList<string> LoadCitiesFromJson()
        {
            string path = Path.Combine(DataLoaderPath, "cities.json");
            
            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();

            IReadOnlyList<string> cities = JsonSerializer.Deserialize<IReadOnlyList<string>>(json) ??
                throw new NotConfiguredException(Messages.MissingCityJsonFile);

            return cities;
        }
        
        public static Dictionary<string,IReadOnlyList<string>> LoadNeighborhoodsFromJson()
        {
            string path = Path.Combine(DataLoaderPath, "neighborhoods.json");

            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();

            Dictionary<string, IReadOnlyList<string>> neighborhoods = JsonSerializer.Deserialize<Dictionary<string, IReadOnlyList<string>>>(json) ??
                throw new NotConfiguredException(Messages.MissingNeighborhoodJsonFile);

            return neighborhoods;
        }
    }
}
