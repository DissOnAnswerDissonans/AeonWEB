
using DB_Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

string connectionString = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("dbSettings.json")
	.Build()
	.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<AeonDBContext>()
	.UseSqlServer(connectionString).Options;

//using (var db = new AeonDBContext(options)) {
//	db.Players.Add(new Player { Nickname = "Test3" });
//	db.Players.Add(new Player { Nickname = "Test4" });
//	db.SaveChanges();
//}

using (var db = new AeonDBContext(options)) {
	var test = db.Players.ToList();
	foreach (var t in test) {
		Console.WriteLine($"ID: {t.Id} -> {t.Nickname}");
	}
}