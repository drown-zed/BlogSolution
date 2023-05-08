using Microsoft.EntityFrameworkCore;
using Blog.Models;
using MySql.EntityFrameworkCore.Extensions;
using Blog.Services;
using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Blog.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
);

// Add services to the container.

builder.Services.AddControllers(option => {
    option.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});
builder.Services.AddScoped<IAIBlogPostGenerator, AIBlogPostGenerator>();
builder.Services.AddScoped<IJwtToken, JwtToken>();
builder.Services.AddScoped<BlogPostRepository>();
builder.Services.AddScoped<CommentRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PostEventProducer>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddEntityFrameworkMySQL().AddDbContext<DatabaseContext>(options =>
//{
//    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});




var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHangfireDashboard();

app.Run();
