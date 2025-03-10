using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Assessments.Web.Infrastructure.AlienSpecies
{
    public class AttachmentRepository
    {
        private readonly IConfiguration _configuration;
        private BlobContainerClient _containerClient;
        private readonly ILogger<AttachmentRepository> _logger;

        private BlobContainerClient ContainerClient
        {
            get
            {
                if (_containerClient != null) return _containerClient;
                
                var connectionString = _configuration["ConnectionStrings:AzureBlobStorage"];
                if (string.IsNullOrWhiteSpace(connectionString)) throw new System.Exception("Missing required config for azure blog storage: ConnectionStrings:AzureBlobStorage");
                _containerClient = new BlobContainerClient(connectionString, "assessments");
                return _containerClient;
            }
        }

        public AttachmentRepository(IConfiguration configuration, ILogger<AttachmentRepository> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        public async Task<Stream> GetFileStream(string path)
        {
            var blobClient = ContainerClient.GetBlobClient(path);

            try
            {
                var stream = await blobClient.OpenReadAsync();

                return stream;
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogWarning("{Message}", ex.Message);
               
                return null;
            }
        }
    }
}
