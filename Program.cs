using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationStackedAuth.Data;
using OperationStackedAuth.Options;
await ConfigureSecret();

async Task ConfigureSecret()
{
    var clientsecret = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.EUWest2);

    var request1 = new GetSecretValueRequest
    {
        SecretId = "AWS_UserPoolId"
    };

    var response1 = await clientsecret.GetSecretValueAsync(request1);

    var secretJson = response1.SecretString;

    var secret1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretJson);

    var request2 = new GetSecretValueRequest
    {
        SecretId = "AWS_UserPoolClientId"
    };

    var response2 = await clientsecret.GetSecretValueAsync(request2);

    var secretJson2 = response2.SecretString;

    var secret2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretJson2);


    Environment.SetEnvironmentVariable("AWS_UserPoolId", secret1.FirstOrDefault().Value);
    Environment.SetEnvironmentVariable("AWS_UserPoolClientId", secret2.FirstOrDefault().Value);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AWS services
builder.Services.AddAWSService<IAmazonCognitoIdentityProvider>(new Amazon.Extensions.NETCore.Setup.AWSOptions
{
    Region = RegionEndpoint.EUWest2
});

// Configure CognitoUserPool
builder.Services.AddSingleton(x =>
{
    var userPoolId = Environment.GetEnvironmentVariable("AWS_UserPoolId");
    var userPoolClientId = Environment.GetEnvironmentVariable("AWS_UserPoolClientId");
    var t = Environment.GetEnvironmentVariables();
    return new CognitoUserPool(userPoolId, userPoolClientId, x.GetRequiredService<IAmazonCognitoIdentityProvider>());
});

// Add the DbContext to the service collection
builder.Services.AddDbContext<OperationStackedAuthDbContext>();

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
