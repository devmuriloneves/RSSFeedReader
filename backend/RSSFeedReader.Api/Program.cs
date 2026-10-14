using RSSFeedReader.Api.Subscriptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SubscriptionStore>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5213")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/subscriptions", (SubscriptionStore store) =>
    Results.Ok(store.GetAll()))
    .WithName("GetSubscriptions");

app.MapPost("/api/subscriptions", (CreateSubscriptionRequest? request, SubscriptionStore store) =>
{
    if (!store.TryAdd(request?.Address, out var subscription))
    {
        return Results.BadRequest();
    }

    return Results.Ok(subscription);
})
    .WithName("AddSubscription");

app.Run();

public partial class Program;

internal sealed record CreateSubscriptionRequest(string? Address);
