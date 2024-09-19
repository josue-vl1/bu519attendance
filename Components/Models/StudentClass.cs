using System.Text.Json.Serialization;

namespace attendance.Components.Models
{
    public class StudentClass 
    {
        private string className { get; set; }
        private List<Student> students = new List<Student>();

        public string ClassName { get => className; set => className = value; }
        public List<Student> Students { get => students; set => students = value; }

        private Uri containerUri;
        private Uri blobUri;

        public StudentClass(string className)
        {
            this.className = className;
            containerUri = new Uri($"https://studentinfo.blob.core.windows.net/{className}");
            blobUri = new Uri($"{containerUri}/students.json");
        }

        public async Task<List<Student>> LoadStudentsAsync()
        {
            try
            {
                var credential = new Azure.Identity.DefaultAzureCredential();
                var blobClient = new Azure.Storage.Blobs.BlobClient(blobUri, credential);
                var containerClient = new Azure.Storage.Blobs.BlobContainerClient(containerUri, credential);

                // Check if the container exists, and create it if it does not
                if (!await containerClient.ExistsAsync())
                {
                    await containerClient.CreateAsync();
                    Console.WriteLine("Container created.");
                }

                if (await blobClient.ExistsAsync())
                {
                    var response = await blobClient.DownloadAsync();
                    using (var streamReader = new StreamReader(response.Value.Content))
                    {
                        var jsonData = await streamReader.ReadToEndAsync();
                        students = System.Text.Json.JsonSerializer.Deserialize<List<Student>>(jsonData) ?? new List<Student>();
                    }
                }
                else {
                    students = new List<Student>();
                }

                //Console.WriteLine("Data loaded successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading student data: {e.Message}");
            }

            return students;
        }

    }
}