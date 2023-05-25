using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using OperationStackedAuth.Data;
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
    return new CognitoUserPool(userPoolId, userPoolClientId, x.GetRequiredService<IAmazonCognitoIdentityProvider>());
});

// Add the DbContext to the service collection
builder.Services.AddDbContext<OperationStackedAuthDbContext>();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Operation Stacked Auth V1"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
