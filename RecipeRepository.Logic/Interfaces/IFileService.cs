﻿namespace RecipeRepository.Logic.Interfaces;

public interface IFileService
{
    public void CheckForAllowedSignature(Stream stream, string proposedExtension);
    public void DeleteExistingFile(string fileName);
    public Task SaveFileToDiskAsync(Stream sourceStream, string physicalDestination, string trustedFileName);
}
