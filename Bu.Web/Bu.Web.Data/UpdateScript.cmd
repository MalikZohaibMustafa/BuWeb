dotnet tool update --global dotnet-ef
dotnet ef migrations remove
dotnet ef migrations add init
dotnet ef migrations script -o ../Database/Script.sql
dotnet ef migrations remove
