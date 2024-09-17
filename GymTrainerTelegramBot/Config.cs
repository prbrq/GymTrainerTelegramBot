using Newtonsoft.Json;

namespace GymTrainerTelegramBot;

public class Config
{
    public required string Token { get; set; }

    /// <summary>
    /// Метод загружает конфигурацию из файла <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">Путь к файлу конфигурации.</param>
    /// <returns>Объект содержащий все конфигурационные.</returns>
    /// <exception cref="InvalidOperationException">Если файл конфигурации поврежден или не существует.</exception>
    public static Config LoadFromFile(string filePath)
    {
        var configFileContent = File.ReadAllText(filePath);
        var configuration = JsonConvert.DeserializeObject<Config>(configFileContent) 
            ?? throw new InvalidOperationException("Файл конфигурации поврежден или не существует");

        return configuration;
    }
}