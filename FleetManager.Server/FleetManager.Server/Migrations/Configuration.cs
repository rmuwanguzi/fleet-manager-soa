namespace MutticoFleet.Server.Migrations
{
    using MutticoFleet.Service;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MutFleet.Server.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MutticoFleet.Server.DbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "A P" },
            //      new Person { FullName = "Br Lm" },
            //      new Person { FullName = "Rn Mm" }
            //    );
            //
            fnn.DB_SCHEMA = "my_fleeter_dev";
            fnn.SeedAdmin("dan@gmail.com", "frnaluk@gmail.com", "1234", context);
        }
    }
}
