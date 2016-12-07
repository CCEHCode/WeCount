namespace PITCSurveyEntities.Migrations
{
    using System;
	using System.Collections.Generic;
	using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
	using PITCSurveyEntities.Entities;

	internal sealed class Configuration : DbMigrationsConfiguration<PITCSurveyEntities.Entities.PITCSurveyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
		
        protected override void Seed(PITCSurveyEntities.Entities.PITCSurveyContext context)
        {
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
        }

    }

	internal static class Extensions
	{
		internal static void AddRange<T>(this IList<T> Collection, IEnumerable<T> NewItems)
		{
			foreach (T Item in NewItems)
			{
				Collection.Add(Item);
			}
		}

		internal static void AddRange<T>(this IList<T> Collection, params T[] NewItems)
		{
			foreach (T Item in NewItems)
			{
				Collection.Add(Item);
			}
		}
	}
}
