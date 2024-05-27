using DsIdentity.Infrastructure;
using DsIdentity.Models;
using DsIdentity.Api.Helpers;

public class Generator
{
    public static void Main()
    {
        var db = new DsIdentityContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        db.User.Add(new () { Alias = "d", Email = "d@d.d", Surname = "d", Name = "d", ProfileImage="wawrzyn1.png"});
        db.User.Add(new () { Alias = "e", Email = "e@e.e", Surname = "e", Name = "e", ProfileImage="npc.png"});
        db.User.Add(new () { Alias = "suhy", Email = "f@d.d", Surname = "Suchojad", Name = "Krzysztof", ProfileImage="suhojad.png"});
        db.User.Add(new () { Alias = "ct", Email = "g@d.d", Surname = "Tokarczyk", Name = "Cezary", ProfileImage="tokar.png"});
        db.User.Add(new () { Alias = "wawrzyn", Email = "h@d.d", Surname = "Milkiewicz", Name = "Wawrzyniec", ProfileImage="wawrzyn.jpg"});
        db.User.Add(new () { Alias = "kierecki", Email = "i@d.d", Surname = "Kierecki", Name = "Rafał", ProfileImage="kierecki.png"});
        db.User.Add(new () { Alias = "koziel", Email = "j@d.d", Surname = "Kozieł", Name = "Kacper", ProfileImage="koziel.png"});
        db.User.Add(new () { Alias = "cadi", Email = "k@d.d", Surname = "Kadziewicz", Name = "Mikołaj", ProfileImage="cadi.jpeg"});
        db.User.Add(new () { Alias = "karton", Email = "l@d.d", Surname = "Pospolity", Name = "Karton", ProfileImage="chad.png"});
        db.User.Add(new () { Alias = "julzal", Email = "m@d.d", Surname = "Zalewski", Name = "Julian", ProfileImage="angry.jpg"});
        db.User.Add(new () { Alias = "kjub", Email = "n@d.d", Surname = "Głuszek", Name = "Jakub", ProfileImage="en57.jpg"});
        db.User.Add(new () { Alias = "biden", Email = "o@d.d", Surname = "d", Name = "Joe", ProfileImage="biden.jpg"});
        db.User.Add(new () { Alias = "trump", Email = "p@d.d", Surname = "Trump", Name = "Donald J", ProfileImage="trump.jpg"});
        db.User.Add(new () { Alias = "sussy", Email = "q@d.d", Surname = "Supińska", Name = "Karolina", ProfileImage="suspinska.jpg"});
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
                User = db.User.ElementAt(0),
                VerificationCode = SecretsBuilder.TextToBase64(verificationCode)
            };
        }
    }
}