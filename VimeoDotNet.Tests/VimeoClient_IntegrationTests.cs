using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace VimeoDotNet.Tests
{
    [TestClass]
    [Ignore] // Comment this line to run integration tests.
    public class VimeoClient_IntegrationTests
    {
        private const string CLIENTID = "702bfebbc185383e2087b73ff82d42a99d85482b";
        private const string CLIENTSECRET = "r2OWqG9eoSq9BOXUDRXUcceg1MP9VZTLwxLP7HwZnIp/yaOanCzpbEpL/LaFPOZBioGySRl51sHIjvZF/8G/3r//H85nzCwaVMQrUweom78B1dcCYTDZKhSpmvaB2ZaX";
        private const string ACCESSTOKEN = "b13da420693d22a6e1129e51d7840136";

        private const string TESTFILEPATH = @"Resources\test.mp4";
            // http://download.wavetlan.com/SVV/Media/HTTP/http-mp4.htm

        [TestMethod]
        public void Integration_VimeoClient_GetUploadTicket_CanGenerateStreamingTicket()
        {
            // arrange
            VimeoClient client = CreateAuthenticatedClient();

            // act
            UploadTicket ticket = client.GetUploadTicket();

            // assert
            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public IUploadRequest Integration_VimeoClient_UploadEntireFile_UploadsFile(string filePath)
        {
            // arrange
            long length;
            IUploadRequest completedRequest;
            using (var file = new BinaryContent(GetFullPath(filePath)))
            {
                length = file.Data.Length;
                VimeoClient client = CreateAuthenticatedClient();

                // act
                completedRequest = client.UploadEntireFile(file);
            }

            // assert
            Assert.IsNotNull(completedRequest);
            Assert.IsTrue(completedRequest.AllBytesWritten);
            Assert.IsTrue(completedRequest.IsVerifiedComplete);
            Assert.AreEqual(length, completedRequest.BytesWritten);
            Assert.IsNotNull(completedRequest.ClipUri);
            Assert.IsTrue(completedRequest.ClipId > 0);
            return completedRequest;
        }

        [TestMethod]
        public void Integration_VimeoClient_DeleteVideo_DeletesVideo()
        {
            // arrange
            long length;
            IUploadRequest completedRequest;
            using (var file = new BinaryContent(GetFullPath(TESTFILEPATH)))
            {
                length = file.Data.Length;
                VimeoClient client = CreateAuthenticatedClient();
                // act
                completedRequest = client.UploadEntireFile(file);
                Assert.IsTrue(completedRequest.AllBytesWritten);
                Assert.IsNotNull(completedRequest);
                Assert.IsTrue(completedRequest.IsVerifiedComplete);
                Assert.AreEqual(length, completedRequest.BytesWritten);
                Assert.IsNotNull(completedRequest.ClipUri);
                Assert.IsTrue(completedRequest.ClipId.HasValue);                
                client.DeleteVideo(completedRequest.ClipId.Value);
                Assert.IsNull(client.GetVideo(completedRequest.ClipId.Value));
            }
            // assert            
        }

        [TestMethod]
        public void Integration_VimeoClient_GetAccountInformation_RetrievesCurrentAccountInfo()
        {
            // arrange
            VimeoClient client = CreateAuthenticatedClient();

            // act
            User account = client.GetAccountInformation();

            // assert
            Assert.IsNotNull(account);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetUserInformation_RetrievesUserInfo()
        {
            // arrange
            long userId = 8128214;
            VimeoClient client = CreateAuthenticatedClient();

            // act
            User user = client.GetUserInformation(userId);

            // assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.id.Value);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetAccountVideos_RetrievesCurrentAccountVideos()
        {
            // arrange
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Paginated<Video> videos = client.GetVideos();

            // assert
            Assert.IsNotNull(videos);
        }

        [TestMethod]
        public async Task Integration_VimeoClient_GetAccountVideos_SecondPage()
        {
            // arrange
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Paginated<Video> videos = await client.GetVideosAsync(page: 2, perPage: 5);

            // assert
            Assert.IsNotNull(videos);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetAccountVideo_RetrievesVideo()
        {
            // arrange
            long clipId = 103374506; // Your video ID here
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Video video = client.GetVideo(clipId);

            // assert
            Assert.IsNotNull(video);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetAccountAlbumVideos_RetrievesCurrentAccountAlbumVideos()
        {
            // arrange
            const long albumId = 2993579; // Your album ID here
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Paginated<Video> videos = client.GetAlbumVideos(albumId);

            // assert
            Assert.IsNotNull(videos);
            Assert.AreNotEqual(videos.data.Count, 0);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetAccountAlbumVideo_RetrievesVideo()
        {
            // arrange
            const long albumId = 2993579; // Your album ID here
            const long clipId = 103374506; // Your video ID here
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Video video = client.GetAlbumVideo(albumId, clipId);

            // assert
            Assert.IsNotNull(video);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetUserAlbumVideos_RetrievesUserAlbumVideos()
        {
            // arrange
            const long userId = 6029930; // Your user ID here
            const long albumId = 2993579; // Your album ID here
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Paginated<Video> videos = client.GetUserAlbumVideos(userId, albumId);

            // assert
            Assert.IsNotNull(videos);
            Assert.AreNotEqual(videos.data.Count, 0);
        }

        [TestMethod]
        public void Integration_VimeoClient_GetUserAlbumVideo_RetrievesVideo()
        {
            // arrange
            const long userId = 6029930; // Your user ID here
            const long albumId = 2993579; // Your album ID here
            const long clipId = 103374506; // Your video ID here
            VimeoClient client = CreateAuthenticatedClient();

            // act
            Video video = client.GetUserAlbumVideo(userId, albumId, clipId);

            // assert
            Assert.IsNotNull(video);
        }

        private VimeoClient CreateUnauthenticatedClient()
        {
            return new VimeoClient(CLIENTID, CLIENTSECRET);
        }

        private VimeoClient CreateAuthenticatedClient()
        {
            return new VimeoClient(ACCESSTOKEN);
        }

        private string GetFullPath(string relativePath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); // /bin/debug
            return Path.Combine(dir.Parent.Parent.FullName, relativePath);
        }
    }
}