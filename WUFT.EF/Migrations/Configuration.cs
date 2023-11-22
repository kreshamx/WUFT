namespace WUFT.EF.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WUFT.EF.WUFTDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WUFT.EF.WUFTDbContext context)
        {
            var seed = new WUFTDbContextSeedData();
            seed.SeedAll(context);
        }
    }
}
