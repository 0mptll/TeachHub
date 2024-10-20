﻿using Firebase.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TeachHub.Services { 
public class FirebaseService
{
    private readonly string _bucket;
    private readonly ILogger<FirebaseService> _logger;

    public FirebaseService(ILogger<FirebaseService> logger, string firebaseConfig)
    {
        _logger = logger;
        _bucket = ExtractBucketName(firebaseConfig);
    }

    private string ExtractBucketName(string firebaseConfig)
    {
        // Assuming firebaseConfig is the full gs:// URL
        if (firebaseConfig.StartsWith("gs://"))
        {
            return firebaseConfig.Substring(5);
        }
        return firebaseConfig;
    }

    public async Task<string> UploadToFirebase(Stream fileStream, string fileName)
    {
        var firebaseStorage = new FirebaseStorage(_bucket);

        var cancellation = new CancellationTokenSource();
        var task = firebaseStorage
            .Child("videos")
            .Child(fileName)
            .PutAsync(fileStream, cancellation.Token);

        task.Progress.ProgressChanged += (s, e) => _logger.LogInformation($"Upload progress: {e.Percentage} %");

        try
        {
            var downloadUrl = await task;
            _logger.LogInformation($"File '{fileName}' uploaded successfully. Download URL: {downloadUrl}");
            return downloadUrl;
        }
        catch (FirebaseStorageException ex)
        {
            _logger.LogError(ex, $"FirebaseStorageException occurred while uploading '{fileName}': ");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error occurred while uploading '{fileName}': {ex.Message}");
            throw;
        }
    }
}
}