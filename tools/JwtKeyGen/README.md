JwtKeyGen - small .NET 9 CLI to generate strong JWT secrets

Usage examples (from the repository root):

# build

dotnet build tools/JwtKeyGen/JwtKeyGen.csproj -c Release

# run

dotnet run --project tools/JwtKeyGen/JwtKeyGen.csproj -- --bytes 32 --format base64

# output is printed plus a PowerShell-friendly line to set an env var

Recommended: use at least 32 bytes (256 bits) for HS256 and prefer at least 64 bytes for HS512 or other uses.
