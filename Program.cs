using Microsoft.Extensions.FileProviders;
using NotesApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем MVC и наши сервисы
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<NoteService>();
builder.Services.AddSingleton<PhotoService>();

var app = builder.Build();

// Обработка ошибок и HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Статические файлы из wwwroot
app.UseStaticFiles();
// Статические файлы из Workspace (например, для фотографий)
// Файлы из папки Workspace будут доступны по URL, начинающемуся с /Workspace
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Workspace")),
    RequestPath = "/Workspace"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Notes}/{action=Index}/{id?}");

app.Run();