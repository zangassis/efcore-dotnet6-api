using BlogManager.Data;
using BlogManager.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

ConfigureApp(app);

void ConfigureApp(WebApplication app)
{
    var ctx = app.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();
    ctx.Database.EnsureCreated();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseCors(builder => builder.AllowAnyOrigin());
}

void RegisterServices(IServiceCollection services)
{
    // Add services to the container.
    services.AddDbContext<AppDbContext>();

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Blogs API",
            Description = "Blog administration",
            Version = "v1"
        });
    });
}

app.MapGet("/v1/posts", (AppDbContext context) =>
{
    var posts = context.Posts;

    if (!posts.Any())
        return Results.NotFound();

    var postsDto = posts.Select(b => new BlogPostDto(b.Id, b.Title, b.Content, b.Tags, b.PublishedDate, b.CoverImage, b.Author)).ToList();

    return Results.Ok(postsDto);

}).Produces<BlogPostDto>();

app.MapPost("/v1/posts", (BlogPostDto createBlogPost, AppDbContext context) =>
{
    try
    {
        var post = new BlogPost()
        {
            Id = Guid.NewGuid(),
            Title = createBlogPost.Title,
            Content = createBlogPost.Content,
            Tags = createBlogPost.Tags,
            PublishedDate = DateTime.UtcNow,
            CoverImage = createBlogPost.CoverImage,
            Author = createBlogPost.Author
        };

        context.Add(post);
        context.SaveChanges();

        return Results.Created($"v1/posts/{createBlogPost.Id}", createBlogPost);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
}).Produces<BlogPostDto>();

app.MapPut("/v1/posts", (Guid id, BlogPost updateBlogPost, AppDbContext context) =>
{
    try
    {
        var blogPost = context.Posts.Find(id);

        if (blogPost is null)
            return Results.NotFound();

        context.Entry(blogPost).CurrentValues.SetValues(updateBlogPost);
        context.SaveChanges();

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error ocurred while puting to Post: {ex.Message}");
    }
});

app.MapDelete("/v1/posts", (Guid id, AppDbContext context) =>
{
    try
    {
        var post = context.Posts.Where(p => p.Id == id).FirstOrDefault();

        if (post is null)
            return Results.BadRequest($"Post not found to Id = {id}");

        context.Remove(post);
        context.SaveChanges();

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.Run();