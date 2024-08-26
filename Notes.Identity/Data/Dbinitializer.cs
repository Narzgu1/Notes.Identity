namespace Notes.Identity.Data
{
    public class Dbinitializer
    {
        public static void Initialize(AuthDbContext context)
        { 
            context.Database.EnsureCreated();
        }
    }
}
