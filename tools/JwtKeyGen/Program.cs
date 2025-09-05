using System;
using System.Security.Cryptography;
using System.Text;

namespace JwtKeyGen;

public static class Program
{
    // Simple CLI: jwtkeygen [--bytes N] [--format base64|hex] [--count N]
    public static int Main(string[] args)
    {
        int bytes = 32; // default 256-bit key
        string format = "base64";
        int count = 1;

        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a == "--bytes" && i + 1 < args.Length && int.TryParse(args[i + 1], out var b)) { bytes = b; i++; }
            else if (a == "--format" && i + 1 < args.Length) { format = args[i + 1].ToLowerInvariant(); i++; }
            else if (a == "--count" && i + 1 < args.Length && int.TryParse(args[i + 1], out var c)) { count = c; i++; }
            else if (a == "-h" || a == "--help") { PrintHelp(); return 0; }
            else { Console.Error.WriteLine($"Unknown arg: {a}"); PrintHelp(); return 2; }
        }

        if (bytes <= 0) { Console.Error.WriteLine("--bytes must be > 0"); return 2; }
        if (count <= 0) { Console.Error.WriteLine("--count must be > 0"); return 2; }

        for (int i = 0; i < count; i++)
        {
            var key = RandomNumberGenerator.GetBytes(bytes);
            string outStr = format switch
            {
                "base64" => Convert.ToBase64String(key),
                "hex" => BitConverter.ToString(key).Replace("-", "").ToLowerInvariant(),
                _ => throw new InvalidOperationException("Unknown format")
            };

            Console.WriteLine(outStr);
            // Print a PowerShell-friendly set command
            Console.WriteLine($"$env:JWT_Key = '{outStr}'");
        }

        return 0;
    }

    static void PrintHelp()
    {
        Console.WriteLine("JwtKeyGen: generate strong random keys for JWT signing\n");
        Console.WriteLine("Usage: jwtkeygen [--bytes N] [--format base64|hex] [--count N]\n");
        Console.WriteLine("Options:");
        Console.WriteLine("  --bytes N      Number of random bytes (default 32 = 256 bits)");
        Console.WriteLine("  --format F     Output format: base64 (default) or hex");
        Console.WriteLine("  --count N      How many keys to generate (default 1)");
        Console.WriteLine("  -h, --help     Show this help");
    }
}
