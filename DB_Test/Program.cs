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

using (var db = new AeonDBContext(options)) {

	db.Players.AddRange(new Player[] {
		new Player { Nickname = "Foo" },
		new Player { Nickname = "Bar" },
		new Player { Nickname = "Baz" },
		new Player { Nickname = "Qux" },
	});
	db.SaveChanges();

	db.Heroes.AddRange(new Hero[] {
		new Hero { AsmName = "Aeon.Heroes.Banker" },
		new Hero { AsmName = "Aeon.Heroes.Master" },
		new Hero { AsmName = "Aeon.Heroes.Thief" },
		new Hero { AsmName = "Aeon.Heroes.Fatty" },
	});
	db.SaveChanges(); 

	db.Games.AddRange(new Game[] {
		new Game { Player1Id = 1, Player2Id = 2, Hero1Id = 3, Hero2Id = 4, Winner = 1 },
		new Game { Player1Id = 1, Player2Id = 3, Hero1Id = 2, Hero2Id = 2, Winner = 2 },
		new Game { Player1Id = 1, Player2Id = 4, Hero1Id = 1, Hero2Id = 3, Winner = 2 },
		new Game { Player1Id = 2, Player2Id = 3, Hero1Id = 4, Hero2Id = 2, Winner = 2 },
		new Game { Player1Id = 2, Player2Id = 4, Hero1Id = 3, Hero2Id = 1, Winner = 1 },
		new Game { Player1Id = 3, Player2Id = 4, Hero1Id = 2, Hero2Id = 3, Winner = 1 },
	});
	db.SaveChanges();


	Console.WriteLine("Players");
	foreach (var t in db.Players) {
		Console.WriteLine($"#{t.Id} -> {t.Nickname}");
	};
	Console.WriteLine();
	Console.WriteLine("Heroes");
	foreach (var h in db.Heroes) {
		Console.WriteLine($"{h.Id}. {h.AsmName}");
	}
	Console.WriteLine();
	Console.WriteLine("===== GAMES =====");
	foreach (var g in db.Games) {
		Console.WriteLine();
		Console.WriteLine($"{g.Player1?.Nickname} (#{g.Player1Id}) vs {g.Player2?.Nickname} (#{g.Player2Id})");
		Console.WriteLine($"{g.Player1?.Nickname} picked {g.Hero1?.AsmName}");
		Console.WriteLine($"{g.Player2?.Nickname} picked {g.Hero2?.AsmName}");
		Console.WriteLine($"Winner: P{g.Winner} ({(g.Winner == 1 ? g.Player1?.Nickname : g.Player2?.Nickname)})");
	}
}