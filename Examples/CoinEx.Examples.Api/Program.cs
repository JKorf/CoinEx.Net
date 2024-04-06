using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the CoinEx services
builder.Services.AddCoinEx();

// OR to provide API credentials for accessing private endpoints, or setting other options:
/*
builder.Services.AddCoinEx(restOptions =>
{
    restOptions.ApiCredentials = new ApiCredentials("<APIKEY>", "<APISECRET>");
    restOptions.RequestTimeout = TimeSpan.FromSeconds(5);
}, socketOptions =>
{
    socketOptions.ApiCredentials = new ApiCredentials("<APIKEY>", "<APISECRET>");
});
*/

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Map the endpoints and inject the CoinEx rest client
app.MapGet("/{Symbol}", async ([FromServices] ICoinExRestClient client, string symbol) =>
{
    var result = await client.SpotApiV2.ExchangeData.GetTickersAsync(new[] { symbol });
    return (object)(result.Success ? result.Data : result.Error!);
})
.WithOpenApi();

app.MapGet("/Balances", async ([FromServices] ICoinExRestClient client) =>
{
    var result = await client.SpotApiV2.Account.GetBalancesAsync();
    return (object)(result.Success ? result.Data : result.Error!);
})
.WithOpenApi();

app.Run();