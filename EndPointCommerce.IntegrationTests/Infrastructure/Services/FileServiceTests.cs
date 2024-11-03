using EndPointCommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EndPointCommerce.IntegrationTests.Infrastructure.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IFormFile> _mockFile;
        private readonly string _directory;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockFile = new Mock<IFormFile>();
            _mockFile.Setup(f => f.FileName).Returns("test_file.txt");

            _directory = Path.GetTempPath();

            _fileService = new FileService();
        }

        private static void Cleanup(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public async Task SaveFile_ShouldSaveAFileAndReturnTheFileName()
        {
            // Act
            var result = await _fileService.SaveFile(_mockFile.Object, _directory);

            // Assert
            Assert.NotNull(result);
            Assert.EndsWith(".txt", result);

            var filePath = Path.Combine(_directory, result);
            Assert.True(File.Exists(filePath));

            // Cleanup
            Cleanup(filePath);
        }


        [Fact]
        public async Task SaveFile_ShouldCopyTheContentsOfTheIncomingFormFileIntoTheFileItCreates()
        {
            // Act
            var result = await _fileService.SaveFile(_mockFile.Object, _directory);

            // Assert
            var filePath = Path.Combine(_directory, result);

            _mockFile.Verify(f => f.CopyToAsync(It.IsAny<FileStream>(), default), Times.Once);

            // Cleanup
            Cleanup(filePath);
        }

        [Fact]
        public void DeleteFile_FileExists_FileDeleted()
        {
            // Arrange
            var filePath = Path.Combine(_directory, Path.GetRandomFileName());
            File.WriteAllText(filePath, "test_contents");

            // Act
            _fileService.DeleteFile(filePath);

            // Assert
            Assert.False(File.Exists(filePath));

            // Cleanup
            Cleanup(filePath);
        }
    }
}
