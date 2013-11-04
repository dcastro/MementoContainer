# MementoContainer

MementoContainer is an alternative approach to the [Memento design pattern](http://paginas.fe.up.pt/~aaguiar/as/gof/hires/pat5ffso.htm).

It is a lightweight utility that keeps a record of your objects' properties so that you can easily rollback to a previous state when recovering from errors.

```csharp
var memento = Memento.Create()
                        .Register(article);

try
{
   ArticleFormatter.Format(article);
}
catch(FormattingException)
{
   memento.Rollback(); //Et voilà!
}

```

## Getting started

### Registering objects

The first step is to annotate your model's properties with the `MementoProperty` and/or `MementoCollection` attributes, so that the memento will know which properties it will need to watch.

```csharp
public class Magazine
{
   [MementoProperty]
   public string Title { get; set; }

   [MementoCollection]
   public IList<Article> Articles { get; set; }
}
```

In order to preserve encapsulation, these properties can have any access modifiers (private, public, protected) and even be static.
And if the class `Article` also happens to have annotated properties, those will be registered as well!

Alternatively, you may also use the `MementoClass` attribute on your classes.
This way, all your object's properties/collections will be recorded.
```csharp
[MementoClass]
public class Article
{
   public string Title { get; set; }
   public string Author { get; set; }
}
```

The Memento object exposes a [fluent interface](http://www.martinfowler.com/bliki/FluentInterface.html) so that you can easily register all your objects with the container.

```csharp
var magazine1 = new Magazine
    {
        Title = "Draft",
        Articles = new List<Article>
            {
                new Article("Draft", "DCastro")
            }
    };

var memento = Memento.Create()
                        .Register(magazine1)
                        .Register(magazine2)
                        .Register(publisher);
```

Now you can freely act upon your objects and if anything goes wrong, just rollback!

```csharp
try
{
   magazine1.Name = "State of emergency declared";
   magazine1.Articles.Clear();
   db.Save(article);
}
catch(DBException)
{
   //The magazine will be renamed back to "Draft"
   //and the article authored by "DCastro" will be re-inserted.
   memento.Rollback();
}
```

### Registering single properties/collections

You can also register single properties/collections, without the need to add attributes to your classes.

```csharp
var memento = Memento.Create()
                        .RegisterProperty(publisher, p => p.Name) //simple property
                        .RegisterProperty(publisher, p => p.ProfilePhoto.Description) //chain of properties
                        .RegisterProperty(() => ArticleFactory.LastReleaseDate) //static property
                        .RegisterCollection(articlesList);
```



## NuGet
To install MementoContainer, run the following command in the Package Manager Console

```
PM> Install-Package MementoContainer
```

## Depency Injection and Mocking
The IMemento interface is available so that you can easily mock it with tools like [Moq](https://code.google.com/p/moq/) in your unit tests and inject it as a depency using your favourite IoC container (e.g., [Castle Windsor](http://docs.castleproject.org/Windsor.MainPage.ashx) or [Ninject](http://www.ninject.org/)).

## Documentation
For more information and advanced usage, head on the [wiki].

[wiki]: https://github.com/dcastro/MementoContainer/wiki
