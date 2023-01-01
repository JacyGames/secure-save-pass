cd ./secure-save-pass
dotnet ef migrations add FirstMigration
cd ..
docker-compose build
docker-compose up -d
timeout 180
cd ./secure-save-pass
dotnet ef database update