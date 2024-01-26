using Web.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();
var app = builder.ConfigureApp();
app.Run();

// Needed to make the `Program` class available to the test projects.
public partial class Program { }
