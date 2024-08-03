using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Util
{
    public class UploadFile : IUploadFile
    {
       public async Task<string> UploadFileToFireBase(IFormFile file,string folderName)
        {
            string fileName =Guid.NewGuid().ToString()+ file.FileName;
            if (file.Length == 0)
            {
                throw new Exception("File is empty");
            }
            var storage = new FirebaseStorage("firestorage-4ee45.appspot.com")
                               .Child(folderName)
                               .Child(fileName);
            await storage.PutAsync(file.OpenReadStream());
            var dowloadUrl = await storage.GetDownloadUrlAsync();
            return dowloadUrl;
        }
    }
    public interface IUploadFile
    {
        Task<string> UploadFileToFireBase(IFormFile file,string folderName);
    }
}
