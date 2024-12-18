using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.BLL.Services.Interfaces;
using ToDoList.BLL.Services.Realizations;
using ToDoList.DAL.Persistence;
using ToDoList.DAL.Repositories.Interfaces.Base;
using ToDoList.DAL.Repositories.Interfaces.Statuses;
using ToDoList.DAL.Repositories.Interfaces.ToDos;
using ToDoList.DAL.Repositories.Realizations.Base;
using ToDoList.DAL.Repositories.Realizations.Statuses;
using ToDoList.DAL.Repositories.Realizations.ToDos;
using Serilog;
using ToDoList.BLL.DTO.ToDo;
using ToDoList.BLL.Validations.ToDo;
using ToDoList.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(currentAssemblies);
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblies(currentAssemblies);

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IToDoRepository, ToDoRepository>();

builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IToDoService, ToDoService>();

builder.Services.AddTransient<ErrorHandlerMiddleware>();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    loggerConfiguration.ReadFrom.Services(services);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
