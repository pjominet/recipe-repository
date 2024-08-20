using RecipeRepository.Logic.Interfaces;

namespace RecipeRepository.Logic.Services;

public class FileService : IFileService
{
    private class AllowedFileExtension
    {
        public required IList<string> ExtensionAliases { get; init; }
        public required IList<byte[]> Signatures { get; init; }
    }

    // https://www.filesignatures.net/
    private readonly IList<AllowedFileExtension> _allowedFileExtensions = new List<AllowedFileExtension>
    {
        new()
        {
            ExtensionAliases = new List<string> {".jpeg", ".jpg", ".jpe"},
            Signatures = new List<byte[]>
            {
                new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE1},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE2},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE3},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE8},
            }
        },
        new()
        {
            ExtensionAliases = new List<string> {".png"},
            Signatures = new List<byte[]>
            {
                new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
            }
        }
    };

    public void CheckForAllowedSignature(Stream stream, string proposedExtension)
    {
        var reader = new BinaryReader(stream); // do not dispose the stream, parent caller needs to do that!
        var allowedFileExtension = _allowedFileExtensions.FirstOrDefault(x => x.ExtensionAliases.Contains(proposedExtension));

        if (allowedFileExtension is null)
            throw new IOException($"{proposedExtension} is no allowed file extension!");

        var headerBytes = reader.ReadBytes(allowedFileExtension.Signatures.Max(m => m.Length));
        if (!allowedFileExtension.Signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature)))
            throw new IOException($"{proposedExtension} does not match its binary signature!");
        stream.Position = 0;
    }

    public void DeleteExistingFile(string fileName)
    {
        if (File.Exists(fileName))
            File.Delete(fileName);
    }

    public async Task SaveFileToDisk(Stream sourceStream, string physicalDestination, string trustedFileName)
    {
        if (!Directory.Exists(physicalDestination))
            Directory.CreateDirectory(physicalDestination);

        var destinationStream = File.Create(Path.Combine(physicalDestination, trustedFileName));
        await sourceStream.CopyToAsync(destinationStream);
        destinationStream.Close();
    }
}
