using DsCore.Infrastructure;
using DsCore.Api.Models;
using DsCryptoLib;

public class Generator
{
    static User GetUser(string alias, string email, string surname, string name, string profile)
    {
        return new()
        {
            Alias = alias,
            Email = email,
            Surname = surname,
            Name = name,
            ProfileImage = profile,
            LastOnline = DateTime.Now,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }

    public static void Main()
    {
        var db = new DsCoreContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        db.Currency.Add(new() { Name = "Ruble"});

        db.User.Add(GetUser("d", "d@d.d",  "d", "d", "wawrzyn1.png"));
        db.User.Add(GetUser("e", "e@e.e",  "e", "e", "npc.png"));
        db.User.Add(GetUser("suhy", "f@d.d",  "Suchojad", "Krzysztof", "suhojad.png"));
        db.User.Add(GetUser("ct", "g@d.d",  "Tokarczyk", "Cezary", "tokar.png"));
        db.User.Add(GetUser("wawrzyn", "h@d.d",  "Milkiewicz", "Wawrzyniec", "wawrzyn.jpg"));
        db.User.Add(GetUser("kierecki", "i@d.d",  "Kierecki", "Rafał", "kierecki.png"));
        db.User.Add(GetUser("koziel", "j@d.d",  "Kozieł", "Kacper", "koziel.png"));
        db.User.Add(GetUser("cadi", "k@d.d",  "Kadziewicz", "Mikołaj", "cadi.jpeg"));
        db.User.Add(GetUser("karton", "l@d.d",  "Pospolity", "Karton", "chad.png"));
        db.User.Add(GetUser("julzal", "m@d.d",  "Zalewski", "Julian", "angry.jpg"));
        db.User.Add(GetUser("kjub", "n@d.d",  "Głuszek", "Jakub", "en57.jpg"));
        db.User.Add(GetUser("biden", "o@d.d",  "Biden", "Joe", "biden.jpg"));
        db.User.Add(GetUser("trump", "p@d.d",  "Trump", "Donald J", "trump.jpg"));
        db.User.Add(GetUser("sussy", "q@d.d",  "Supińska", "Karolina", "suspinska.png"));
        db.SaveChanges();

        for (int i = 0; i < db.User.Count(); i++)
        {
            var salt = SecretsBuilder.GenerateSalt();
            var passwordNewHash = SecretsBuilder.CreatePasswordHash("ZA==", salt); //set pass to d
            var verificationCode = SecretsBuilder.GenerateVerificationCode();
            var credentials = new Credentials()
            {
                Password = passwordNewHash,
                Salt = salt,
                User = db.User.ElementAt(i),
                VerificationCode = SecretsBuilder.TextToBase64(verificationCode)
            };
            db.Credentials.Add(credentials);
        }
        db.SaveChanges();

        db.Transaction.Add(new()
        {
            Payment = new()
            {
                Currency = db.Currency.ElementAt(0),
                User = db.User.ElementAt(0),
                Value = 1000,
            }
        });
        db.SaveChanges();
    }
}