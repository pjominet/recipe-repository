﻿namespace RecipeRepository.Logic.Interfaces;

public interface IFileService
{
    public void CheckForAllowedSignature(Stream stream, string proposedExtension);
    public void DeleteExistingFile(string fileName);
    public Task SaveFileToDisk(Stream sourceStream, string physicalDestination, string trustedFileName);
}
