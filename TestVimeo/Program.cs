using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VimeoDotNet.Net;
using VimeoDotNet.Tests;

namespace TestVimeo
{
    class Program
    {
        static void Main(string[] args)
        {
            VimeoClient_IntegrationTests obj = new VimeoClient_IntegrationTests();

            string filePath = @"Resources\test.mp4";
            IUploadRequest completedRequest = obj.Integration_VimeoClient_UploadEntireFile_UploadsFile(filePath);

            Console.WriteLine("Video uploaded successfully.");
            Console.WriteLine("ID:" + completedRequest.ClipId);
            Console.WriteLine("URI:" + completedRequest.ClipUri);
            Console.ReadLine();
        }
    }
}
