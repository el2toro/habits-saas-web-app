using GoodHabits.Database;
using GoodHabits.Database.Interfaces;
using GoodHabits.HabitService;
using GoodHabits.HabitService.Dtos;
using GoodHabits.HabitService.Extensions;
using GoodHabits.HabitService.Interfaces;
using GoodHabits.HabitService.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen(c =>
c.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "GoodHabits.HabitService",
    Version = "v1"
 }));

builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IHabitService, HabitService>();
builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection(nameof(TenantSettings)));
builder.Services.AddAndMigrateDatabases(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("GoodHabitsPolicy",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint(
        "/swagger/v1/swagger.json", "GoodHabits.HabitServicev1"));
}



app.UseHttpsRedirection();

using var scope = app.Services.CreateScope();
var habitService = scope.ServiceProvider.GetRequiredService<IHabitService>();

app.MapGet("/GetHabitById/{id}", async (int id) =>
{

    return Results.Ok(await habitService.GetById(id));
})
.WithName("GetHabitById")
.WithOpenApi();

app.MapGet("/GetHabits", async () =>
{
    var habits = await habitService.GetAll();
    return Results.Ok(habits);
})
.WithName("GetHabits")
.WithOpenApi();

app.MapPost("/CreateHabit", async (CreateHabitDto request) =>
{

    return Results.Ok(await habitService.Create(request.Name, request.Description));
})
.WithName("CreateHabit")
.WithOpenApi();

app.UseCors("GoodHabitsPolicy");

app.Run();

