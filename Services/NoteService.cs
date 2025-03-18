using System;
using System.Collections.Generic;
using System.IO;
using NotesApp.Models;

namespace NotesApp.Services;

 /// <summary>
    /// Сервис для работы с заметками.
    /// Реализует операции загрузки, создания, обновления и удаления заметок.
    /// Заметки сохраняются в виде Markdown-файлов в папке Workspace.
    /// </summary>
    public class NoteService
    {
        private readonly string _workspacePath;
        private List<Note> _notesCache;

        public NoteService()
        {
            // Путь к рабочей директории (Workspace)
            _workspacePath = Path.Combine(Directory.GetCurrentDirectory(), "Workspace");
            if (!Directory.Exists(_workspacePath))
            {
                Directory.CreateDirectory(_workspacePath);
            }
            LoadNotes();
        }

        /// <summary>
        /// Загружает все заметки из Workspace.
        /// Предполагается, что каждая заметка хранится в файле с расширением .md
        /// и именем в формате "id_title.md".
        /// </summary>
        private void LoadNotes()
        {
            _notesCache = new List<Note>();
            var noteFiles = Directory.GetFiles(_workspacePath, "*.md", SearchOption.TopDirectoryOnly);
            foreach (var file in noteFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                // Предполагается, что имя файла содержит id и заголовок, разделённые символом '_'
                var parts = fileName.Split('_', 2);
                if (parts.Length == 2 && int.TryParse(parts[0], out int id))
                {
                    var title = parts[1];
                    var content = File.ReadAllText(file);
                    var createdDate = File.GetCreationTime(file);
                    _notesCache.Add(new Note 
                    { 
                        Id = id, 
                        Title = title, 
                        Content = content, 
                        CreatedDate = createdDate 
                    });
                }
            }
        }

        /// <summary>
        /// Возвращает список всех заметок.
        /// </summary>
        public IEnumerable<Note> GetAllNotes()
        {
            return _notesCache;
        }

        /// <summary>
        /// Возвращает заметку по идентификатору.
        /// </summary>
        public Note GetNoteById(int id)
        {
            return _notesCache.Find(n => n.Id == id);
        }

        /// <summary>
        /// Создаёт новую заметку, присваивая ей новый идентификатор,
        /// сохраняет заметку в файловой системе и добавляет в кэш.
        /// </summary>
        public void CreateNote(Note note)
        {
            int newId = _notesCache.Count > 0 ? _notesCache[^1].Id + 1 : 1;
            note.Id = newId;
            note.CreatedDate = DateTime.Now;
            _notesCache.Add(note);
            SaveNoteToFile(note);
        }

        /// <summary>
        /// Обновляет существующую заметку и сохраняет изменения в файловой системе.
        /// </summary>
        public void UpdateNote(Note note)
        {
            var existingNote = GetNoteById(note.Id);
            if (existingNote == null)
                throw new Exception("Заметка не найдена");

            existingNote.Title = note.Title;
            existingNote.Content = note.Content;
            SaveNoteToFile(existingNote);
        }

        /// <summary>
        /// Удаляет заметку по идентификатору и удаляет соответствующий файл.
        /// </summary>
        public void DeleteNote(int id)
        {
            var note = GetNoteById(id);
            if (note == null)
                return;
            _notesCache.Remove(note);
            var filePath = GetNoteFilePath(note);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// Сохраняет заметку в виде Markdown-файла.
        /// </summary>
        private void SaveNoteToFile(Note note)
        {
            var filePath = GetNoteFilePath(note);
            File.WriteAllText(filePath, note.Content);
        }

        /// <summary>
        /// Генерирует путь к файлу заметки по соглашению: "id_title.md".
        /// Имя заголовка очищается от недопустимых символов.
        /// </summary>
        private string GetNoteFilePath(Note note)
        {
            var sanitizedTitle = string.Concat(note.Title.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_workspacePath, $"{note.Id}_{sanitizedTitle}.md");
        }
    }