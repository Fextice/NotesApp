using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Сервис для загрузки фотографий. Сохраняет файлы в папке Workspace/pictures.
/// Допустимые форматы: JPEG, SVG, GIF, PNG :)
/// </summary>
public class PhotoService
{
    private readonly string _workspacePicturesPath;
    private readonly string[] _allowedExtensions = { ".jpeg", ".jpg", ".svg", ".gif", ".png" };

    public PhotoService()
    {
        var workspacePath = Path.Combine(Directory.GetCurrentDirectory(), "Workspace");
        _workspacePicturesPath = Path.Combine(workspacePath, "pictures");
        if (!Directory.Exists(_workspacePicturesPath))
        {
            Directory.CreateDirectory(_workspacePicturesPath);
        }
    }

    public async Task<string> UploadPhotoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (Array.IndexOf(_allowedExtensions, extension) < 0)
        {
            throw new ArgumentException("Неверный формат файла. Допустимые форматы: JPEG, SVG, GIF, PNG.");
        }

        // Генерация уникального имени файла
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_workspacePicturesPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Возвращаем относительный путь (например, "/Workspace/pictures/имяфайла")
        return $"/Workspace/pictures/{fileName}";
    }
}