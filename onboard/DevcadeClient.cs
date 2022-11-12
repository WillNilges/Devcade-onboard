using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

// RadosGW
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using Microsoft.Xna.Framework; // FIXME: Is this necessary for the client code?

// For making requests to the API
using System.Net.Http;
using Newtonsoft.Json;

namespace onboard
{
    public class DevcadeGame
    {
        public string id { get; set; }
        public string author { get; set; }
        public DateTime uploadDate { get; set; }
        public string name { get; set; }
        public string hash { get; set; }
    }

    public class DevcadeClient
    {
        private string _accessKey;
        private string _secretKey;

        private string _bucketName;

        private static AmazonS3Config _config;
        private static AmazonS3Client _s3Client;

        private string _apiDomain;

        public DevcadeClient()
        {
            _accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            _secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            _bucketName = "devcade-games";

            _config = new AmazonS3Config();
            _config.ServiceURL = "https://s3.csh.rit.edu";
            _config.ForcePathStyle = true;

            _s3Client = new AmazonS3Client(
                    _accessKey,
                    _secretKey,
                    _config
            );

            _apiDomain = Environment.GetEnvironmentVariable("DEVCADE_API_DOMAIN");
        }
        
        public List<DevcadeGame> GetGames()
        {
            Task<List<DevcadeGame>> infoTask = asyncGetGames();
            return infoTask.Result;
        }

        async Task<List<DevcadeGame>> asyncGetGames()
        {
            HttpClient client = new HttpClient();
            List<DevcadeGame> games;
                // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string uri = $"https://{_apiDomain}/api/games/gamelist/"; // TODO: Env variable URI tld 
                string responseBody = await client.GetStringAsync(uri);

                games = JsonConvert.DeserializeObject<List<DevcadeGame>>(responseBody);
                return games;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return new List<DevcadeGame>();
        }

        // Returns true if success and false otherwise
        // permissions can be an int or a string. For example it can also be +x, -x etc..
        bool Chmod(string filePath, string permissions = "700", bool recursive = false)
        {
            string cmd;
            if (recursive)
                cmd = $"chmod -R {permissions} {filePath}";
            else
                cmd = $"chmod {permissions} {filePath}";

            try
            {
                using (Process proc = Process.Start("/bin/bash", $"-c \"{cmd}\""))
                {
                    proc.WaitForExit();
                    return proc.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public void runGame(DevcadeGame game)
        {
            string path = "/tmp/" + game + ".zip";

            Console.WriteLine("Getting " + game);

            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync($"https://{_apiDomain}/api/games/download/${game.id}"))
                {
                    using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        s.Result.CopyTo(fs);
                    }
                }
            }

            try
            {
                Console.WriteLine("Extracting " + path);
                // Extract the specified path (the zip file) to the specified directory (/tmp/, probably)
                System.IO.Directory.CreateDirectory("/tmp/" + game);
                ZipFile.ExtractToDirectory(path, "/tmp/" + game);
            } catch (System.IO.IOException e) {
                Console.WriteLine(e);
            }

            string execPath = "/tmp/" + game + "/publish/" + game;
            Console.WriteLine("Running " + execPath);
            Chmod(execPath,"+x",false);
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo(execPath) // chom
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Path.GetDirectoryName(execPath)
                }
            };

            process.Start();
        }

        public void listBuckets()
        {
            Task<ListBucketsResponse> response = ListBucketsAsync();
            
            foreach (S3Bucket b in response.Result.Buckets)
            {
                Console.WriteLine("{0}\t{1}", b.BucketName, b.CreationDate);
            }
        }

        // Async method to get a list of Amazon S3 buckets.
        private async Task<ListBucketsResponse> ListBucketsAsync()
        {
            var response = await _s3Client.ListBucketsAsync();
            return response;
        }
        
        public async Task<List<string>> ListBucketContentsAsync(string bucketName)
        {
            try
            {
                List<string> myGameTitles = new List<String>();
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 5,
                };
                var response = new ListObjectsV2Response();
                do
                {
                    response = await _s3Client.ListObjectsV2Async(request);
                    response.S3Objects.ForEach(obj => myGameTitles.Add(obj.Key));
                    // If the response is truncated, set the request ContinuationToken
                    // from the NextContinuationToken property of the response.
                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);
                return myGameTitles;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' getting list of objects.");
                return new List<String>();
            }
        }
    }
}
