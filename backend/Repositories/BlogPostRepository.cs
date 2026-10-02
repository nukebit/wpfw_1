using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public class BlogPostRepository
{
    // temp data
    private static readonly BlogPost[] BlogPosts =
    {
        new BlogPost
        {
            Id = 1,
            Title = "Validate iedere API request",
            Category = "Laravel",
            Content = "Form requests houden validation rules buiten de controllers. Ongeldige data komt hierdoor niet in de application logic terecht.",
            PublishedOn = new DateOnly(2026, 9, 7)
        },
        new BlogPost
        {
            Id = 2,
            Title = "Bewaar state bij de juiste component",
            Category = "React",
            Content = "Local state is geschikt voor tijdelijke data. Gedeelde server data gaat via een duidelijke API layer.",
            PublishedOn = new DateOnly(2026, 9, 8)
        },
        new BlogPost
        {
            Id = 3,
            Title = "Maak API responses voorspelbaar",
            Category = "REST API",
            Content = "Een vaste JSON structure maakt succesvolle requests en errors eenvoudiger af te handelen in React.",
            PublishedOn = new DateOnly(2026, 9, 9)
        }
    };

    /**
     * Reads all blog posts from the temporary data. No parameters are required.
     * @returns {BlogPost[]} The complete blog post list.
     */
    public BlogPost[] GetAll()
    {
        return BlogPosts;
    }

    /**
     * Searches the temporary data for a blog post with the requested id.
     * @param {int} id - The id to search for.
     * @returns {BlogPost?} The matching blog post, or null if no match exists.
     */
    public BlogPost? GetById(int id)
    {
        return Array.Find(BlogPosts, blogPost => blogPost.Id == id);
    }
}
