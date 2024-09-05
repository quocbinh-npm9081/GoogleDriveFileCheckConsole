using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

class Program
{
    static string[] Scopes = { DriveService.Scope.DriveReadonly };
    static string ApplicationName = "GoogleDriveFileCheck";

    static async Task Main(string[] args)
    {
        string credentialsPath = "credentials.json";
        string folderId = "12yrKyEds81lOeY-XUuOs01RUnZrydCN1";
        string specifiedDate = "2024-09-05"; // Ngày bạn muốn kiểm tra

        GoogleCredential credential;
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        int fileCount = await CountFilesInFolder(service, folderId, specifiedDate);
        Console.WriteLine($"Tổng số file được tạo vào ngày {specifiedDate} trong thư mục {folderId} và các thư mục con: {fileCount}");
    }

    static async Task<int> CountFilesInFolder(DriveService service, string folderId, string specifiedDate)
    {
        int fileCount = 0;
        string query = $"'{folderId}' in parents and createdTime >= '{specifiedDate}T00:00:00' and createdTime < '{specifiedDate}T23:59:59'";
        var request = service.Files.List();
        request.Q = query;
        request.Fields = "files(id, name, mimeType, createdTime)";

        var result = await request.ExecuteAsync();
        foreach (var file in result.Files)
        {
            if (file.MimeType == "application/vnd.google-apps.folder")
            {
                fileCount += await CountFilesInFolder(service, file.Id, specifiedDate);
            }
            else
            {
                fileCount++;
                Console.WriteLine($"File ID: {file.Id}, Name: {file.Name}, Created Time: {file.CreatedTime}");
            }
        }

        return fileCount;
    }
}