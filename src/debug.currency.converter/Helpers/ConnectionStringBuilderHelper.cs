namespace debug.currency.converter.Helpers;

public class ConnectionStringBuilderHelper
{
    private readonly IConfiguration _configuration;

    public ConnectionStringBuilderHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    

    public string GetConnectionString(string connectionName)
    {
        return connectionName switch
        {
            "DefaultConnection" => GetOdbcConnectionString(),
            _ => null!
        };
    }

    private string GetOdbcConnectionString()
    {
        var connectionSettings = _configuration.GetSection("ConnectionStrings:DefaultConnection");
        
        var server = connectionSettings["Server"];
        var port = connectionSettings["Port"];
        var database = connectionSettings["Database"];
        var uid = connectionSettings["Uid"];
        var pwd = connectionSettings["Pwd"];

        return $"Server={server},{port};Database={database};Uid={uid};Pwd={pwd};Encrypt=no;";
    }
}