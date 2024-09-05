using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using File = Google.Apis.Drive.v3.Data.File;

//https://medium.com/@meghnav274/uploading-files-to-google-drive-using-net-console-app-f0aae69a3f0f

class Program
{
    static string[] Scopes = { DriveService.Scope.DriveReadonly };
    static string ApplicationName = "GoogleDriveFileCheck";
    static void Main(string[] args)
    {
        string credentialsPath = "credentials.json";
        string folderId = "12yrKyEds81lOeY-XUuOs01RUnZrydCN1";
        string specifiedDate = "2024-09-05"; // Ngày bạn muốn kiểm tra


        GoogleCredential credential;
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
            {
                    DriveService.ScopeConstants.DriveReadonly
            });

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveFileCheck"
            });

            var fileMetaData = new File()
            {
                Parents = new List<string> { folderId },
            };


            string query = $"'{folderId}' in parents and createdTime >= '{specifiedDate}T00:00:00' and createdTime < '{specifiedDate}T23:59:59'"; //
            var request = service.Files.List();
            request.Q = query;
            request.Fields = "files(id, name, createdTime)";

            var result = request.Execute();
            Console.WriteLine($"Số lượng file được tạo vào ngày {specifiedDate} trong thư mục {folderId}: {result.Files.Count}");
            foreach (var file in result.Files)
            {
                Console.WriteLine($"File ID: {file.Id}, Name: {file.Name}, Created Time: {file.CreatedTime}");

            }
        }
    }
}