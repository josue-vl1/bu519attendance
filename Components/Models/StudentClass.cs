using System.Text.Json.Serialization;
using NCrontab;


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
        private Azure.Identity.DefaultAzureCredential? credential = null;

        private HttpClient httpClient = new HttpClient();

        public string error = "";
        public string status = "";

        private static Dictionary<string, ClassInfo> scheduleData = new Dictionary<string, ClassInfo>();

        public StudentClass(string className)
        {
            this.className = className;
            try
            {
                containerUri = new Uri($"https://studentinfo.blob.core.windows.net/{className}");
                blobUri = new Uri($"{containerUri}/students.json");

                credential = new Azure.Identity.DefaultAzureCredential();
                var containerClient = new Azure.Storage.Blobs.BlobContainerClient(containerUri, credential);

                // Check if the container exists, and create it if it does not
                if (!containerClient.Exists())
                {
                    containerClient.Create();
                    Console.WriteLine("Container created.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating container: " + e.Message);
            }

            scheduleData["519fall2024"] = new ClassInfo("0 0 * * 1", new DateTime(2024, 09, 03), new DateTime(2024, 12, 31),
                    new List<string> { "nagendra@bu.edu", "langd0n@bu.edu", "matzh@bu.edu" });
        }

        public static List<DateOnly> getSchedule(string classname)
        {
            List<DateOnly> schedule = new List<DateOnly>();

            if (scheduleData.ContainsKey(classname))
            {

                var info = scheduleData[classname];
                var cronSchedule = CrontabSchedule.Parse(info.Schedule);
                var occurrence = cronSchedule.GetNextOccurrences(info.StartDate, info.EndDate);

                //schedule.AddRange(occurrence);
                //convert to dateonly
                schedule = occurrence.Select(x => DateOnly.FromDateTime(x.Date)).ToList();
            }

            return schedule;
        }

        public static List<string> lookupClassName(string principalName)
        {

            List<string> classnames = new List<string>();

            // if (principalName == "nagendra@bu.edu" || principalName == "langd0n@bu.edu" || principalName == "matzh@bu.edu")
            // {
            //     classnames.Add("519fall2024");
            // }

            // if (principalName == "nagendra@bu.edu")
            // {
            //     classnames = new List<string>() {"488fall2024"};
            // }

            foreach (var item in scheduleData)
            {
                if (item.Value.Instructors.Contains(principalName))
                {
                    classnames.Add(item.Key);
                }
            }

            return classnames;
        }

        public async Task<List<Student>> LoadStudentsAsync()
        {
            try
            {
                var blobClient = new Azure.Storage.Blobs.BlobClient(blobUri, credential);
                var containerClient = new Azure.Storage.Blobs.BlobContainerClient(containerUri, credential);


                if (await blobClient.ExistsAsync())
                {
                    var response = await blobClient.DownloadAsync();
                    using (var streamReader = new StreamReader(response.Value.Content))
                    {
                        var jsonData = await streamReader.ReadToEndAsync();
                        students = System.Text.Json.JsonSerializer.Deserialize<List<Student>>(jsonData) ?? new List<Student>();
                        students = students.OrderBy(x => x.Name).ToList();

                    }
                }
                else
                {
                    students = new List<Student>();
                }

                //Console.WriteLine("Data loaded successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading student data: {e.Message}");
                error += "  - Error loading student data: " + e.Message + " - ";

            }

            return students;
        }

        public async Task<bool> SaveStudentsAsync()
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(students);
            var blobUri = new Uri($"{containerUri}/students.json");

            try
            {
                var blobClient = new Azure.Storage.Blobs.BlobClient(blobUri, credential);

                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData)))
                {
                    Console.WriteLine("Uploading data to blob storage");
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving student data: {e.Message}");
                error += "  - Error saving student data: " + e.Message + " - ";
            }

            return false;
        }

        public async Task<List<Student>> LoadStudentDataAsync(string _date)
        {
            var date = System.Text.Encodings.Web.UrlEncoder.Default.Encode(_date) + ".json";
            status = $"Loading : {date}";
            //Console.WriteLine($"Loading student data for {date}");
            try
            {
                var blobUri = new Uri($"{containerUri}/{date}");
                var blobClient = new Azure.Storage.Blobs.BlobClient(blobUri, credential);

                //reset the student attendance data
                foreach (var student in students)
                {
                    student.Present = false;
                    student.Exempt = false;
                }

                // Check if the blob exists
                if (await blobClient.ExistsAsync())
                {
                    var response = await blobClient.DownloadAsync();

                    using (var streamReader = new StreamReader(response.Value.Content))
                    {
                        var jsonData = await streamReader.ReadToEndAsync();
                        var attendance = System.Text.Json.JsonSerializer.Deserialize<List<StudentData>>(jsonData);
                        if (attendance != null)
                        {

                            status += $" - {attendance.Count} students found";
                            foreach (var student in attendance)
                            {
                                var index = students.FindIndex(x => x.BUID == student.BUID);
                                if (index != -1)
                                {
                                    students[index].Present = student.Present;
                                    students[index].Exempt = student.Exempt;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No attendance data found for this date");

                    status += " - No attendance data found";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading student data: {e.Message}");
                status += " - Error loading student data: " + e.Message;
            }

            return students;
        }

        public async Task<bool> SaveStudentDataAsync(string _date)
        {
            var date = System.Text.Encodings.Web.UrlEncoder.Default.Encode(_date) + ".json";
            var blobUri = new Uri($"{containerUri}/{date}");
            status = $"Saving : {date}";

            try
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(students.Select(x => new StudentData
                {
                    BUID = x.BUID,
                    Present = x.Present,
                    Exempt = x.Exempt
                }).ToList());
                error += " -- data:  " + jsonData + " --  ";

                //Console.WriteLine("Data to save: " + jsonData);
                var blobClient = new Azure.Storage.Blobs.BlobClient(blobUri, credential);

                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData)))
                {
                    //Console.WriteLine("Uploading data to blob storage");
                    error += " - uploading data- ";
                    await blobClient.UploadAsync(stream, overwrite: true);
                    //Console.WriteLine("Data saved successfully");
                }

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving student data: {e.Message}");
                error += e.Message ?? "Unknown error";
            }

            return false;

        }
    }
}