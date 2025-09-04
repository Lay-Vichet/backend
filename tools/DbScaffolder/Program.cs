// Accept connection string either as first arg or via DB_CONNECTION environment variable.
// Allow output directory as second arg; default to the domain entities folder if not provided.
var connectionString = string.Empty;
var outputDir = string.Empty;

if (args.Length >= 1)
    connectionString = args[0];
else
    connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? string.Empty;

if (args.Length >= 2)
    outputDir = args[1];
else
    outputDir = Path.Combine("..", "..", "src", "Domain", "Entities");

// Basic sanitization: trim whitespace and stray surrounding quotes or leading slashes/backslashes
connectionString = connectionString?.Trim().Trim('"', '\'') ?? string.Empty;
connectionString = connectionString.TrimStart('\\', '/');

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Error: connection string is empty. Provide it as the first argument or in DB_CONNECTION env var.");
    Console.WriteLine("Usage: DbScaffolder <connectionString> <outputDir>");
    return 1;
}

// Redact sensitive values for the diagnostic output
string RedactConnectionString(string cs)
{
    try
    {
        var pairs = cs.Split(';', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < pairs.Length; i++)
        {
            var kv = pairs[i].Split('=', 2);
            if (kv.Length == 2 && kv[0].Trim().Equals("Password", StringComparison.OrdinalIgnoreCase))
                pairs[i] = kv[0] + "=****";
        }
        return string.Join(';', pairs);
    }
    catch
    {
        return "(unable to redact)";
    }
}

Console.WriteLine($"Using connection: {RedactConnectionString(connectionString)}");
Console.WriteLine($"Output directory: {outputDir}");

await using var conn = new NpgsqlConnection(connectionString);
try
{
    await conn.OpenAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Failed to open a connection to the database.");
    Console.WriteLine("Common causes:\n - malformed connection string (e.g. a leading backslash like \"\\host=...\")\n - wrong quoting/escaping when invoking from PowerShell\n - network or auth issues");
    Console.WriteLine($"Exception: {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine("Tip: In PowerShell wrap the connection string in single quotes, e.g.:\n  dotnet run -- 'Host=localhost;Port=5432;Username=me;Password=secret;Database=db' ..\\..\\src\\Domain\\Entities");
    return 1;
}

var tables = (await conn.QueryAsync<string>(@"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type='BASE TABLE';")).ToList();

Directory.CreateDirectory(outputDir);

foreach (var table in tables)
{
    var cols = (await conn.QueryAsync(@"
		SELECT column_name, data_type, is_nullable
		FROM information_schema.columns
		WHERE table_schema = 'public' AND table_name = @Table
		ORDER BY ordinal_position;", new { Table = table })).ToList();

    var className = ToPascalCase(table);
    var sb = new StringBuilder();
    sb.AppendLine("using System;");
    sb.AppendLine();
    sb.AppendLine("namespace SubscriptionTracker.Domain.Entities");
    sb.AppendLine("{");
    sb.AppendLine($"    public sealed class {className}");
    sb.AppendLine("    {");

    foreach (var c in cols)
    {
        var col = (IDictionary<string, object>)c;
        var name = (string)col["column_name"];
        var dataType = (string)col["data_type"];
        var isNullable = ((string)col["is_nullable"]).Equals("YES", StringComparison.OrdinalIgnoreCase);
        var propType = MapPostgresTypeToCSharp(dataType, isNullable);
        sb.AppendLine($"        public {propType} {ToPascalCase(name)} {{ get; set; }}");
    }

    sb.AppendLine("    }");
    sb.AppendLine("}");

    var filePath = Path.Combine(outputDir, className + ".cs");
    await File.WriteAllTextAsync(filePath, sb.ToString());
    Console.WriteLine($"Generated {filePath}");
}

return 0;

static string ToPascalCase(string input)
{
    var parts = input.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
    return string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1)));
}

static string MapPostgresTypeToCSharp(string pgType, bool isNullable)
{
    // Minimal type map, expand as needed
    string t = pgType switch
    {
        "integer" => "int",
        "bigint" => "long",
        "smallint" => "short",
        "boolean" => "bool",
        "text" => "string",
        "character varying" => "string",
        "timestamp without time zone" => "DateTime",
        "timestamp with time zone" => "DateTimeOffset",
        "date" => "DateTime",
        "numeric" => "decimal",
        "real" => "float",
        "double precision" => "double",
        "uuid" => "Guid",
        _ => "string",
    };

    if (t != "string" && isNullable)
        return t + "?";
    return t;
}
