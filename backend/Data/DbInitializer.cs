using PortfolioApi.Models;

namespace PortfolioApi.Data;

public static class DbInitializer
{
    /**
     * Creates the database if needed and fills empty tables with example data.
     * @param {PortfolioDbContext} context - Database context configured for the local SQL Server.
     * @returns {void} No return value, saves the example data in the database.
     */
    public static void Initialize(PortfolioDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Projects.Any())
        {
            // SQL Server generates the ids when these projects are saved.
            context.Projects.AddRange(
                new Project
                {
                    Title = "Task management platform",
                    Type = "applicatie",
                    Description = "Een applicatie voor het maken van projecten, verdelen van taken en volgen van de voortgang."
                },
                new Project
                {
                    Title = "Event booking application",
                    Type = "applicatie",
                    Description = "Een applicatie waarin gebruikers evenementen bekijken, plaatsen reserveren en boekingen beheren."
                },
                new Project
                {
                    Title = "Inventory dashboard",
                    Type = "dashboard",
                    Description = "Een dashboard voor productbeheer, voorraadupdates en meldingen bij lage voorraad."
                });
        }

        if (!context.BlogPosts.Any())
        {
            context.BlogPosts.AddRange(
                new BlogPost
                {
                    Title = "Validate iedere API request",
                    Category = "Laravel",
                    Content = "Form requests houden validation rules buiten de controllers. Ongeldige data komt hierdoor niet in de application logic terecht.",
                    PublishedOn = new DateOnly(2026, 9, 7)
                },
                new BlogPost
                {
                    Title = "Bewaar state bij de juiste component",
                    Category = "React",
                    Content = "Local state is geschikt voor tijdelijke data. Gedeelde server data gaat via een duidelijke API layer.",
                    PublishedOn = new DateOnly(2026, 9, 8)
                },
                new BlogPost
                {
                    Title = "Maak API responses voorspelbaar",
                    Category = "REST API",
                    Content = "Een vaste JSON structure maakt succesvolle requests en errors eenvoudiger af te handelen in React.",
                    PublishedOn = new DateOnly(2026, 9, 9)
                });
        }

        context.SaveChanges();
    }
}
