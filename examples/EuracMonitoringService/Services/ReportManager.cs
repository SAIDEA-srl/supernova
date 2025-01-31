namespace EuracMonitoringService.Services;

public class ReportManager
{
    const string REPORT_PATHS = @"wwwroot";

    public static string CreateFile(string filePath, string content)
    {
        if(!Directory.Exists(REPORT_PATHS))
        {
            Directory.CreateDirectory(REPORT_PATHS);
        }

        File.WriteAllText(filePath, content);

        return $"{filePath}";
    }

    public static void DeleteFile(string filename)
    {
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), REPORT_PATHS, filename.TrimStart('/'));

        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }

}
