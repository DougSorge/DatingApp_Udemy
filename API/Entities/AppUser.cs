//User Entity
namespace API.Entities
{
    public class AppUser
    {
        //Entity framework uses naming conventions to recognize certain fields. This property needs to be named Id so that it will be recognized as a primary key. It will be set to auto increment by default.
        public int ID { get; set; }

        //This name also follows Entity conventions and needs the ? after the type in case the value in the database is null. It means the value is nullable.
        public string UserName { get; set; }

        public byte[] PasswordHash  { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
