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

The first step is to annotate your model's properties with the `MementoProperty` attribute, so that the memento will know which properties it will need to watch.

```csharp
public class Article
{
   [MementoProperty]
   public string Title { get; set; }

   [MementoProperty]
   public DateTime? ReleaseDate { get; set; }

   [MementoProperty]
   public Person Author { get; set; }
}
```

In order to preserve encapsulation, these properties can have any access modifiers (private, public, protected) and even be static.
And if the class `Person` also happens to have annotated properties, those will be registered as well!

Alternatively, you may also use the `MementoClass` attribute on your classes or interfaces.
This way, all your object's properties will be recorded, provided they implement both get and set accessors.
```csharp
[MementoClass]
public class Magazine
{
   public string Title { get; set; }
   public Photo FrontPagePhoto { get; set; }
   //more properties...
}
```

The Memento object exposes a [fluent interface](http://www.martinfowler.com/bliki/FluentInterface.html) so that you can easily register all your objects with the container.

```csharp
var article = new Article("Draft", null, author);

var memento = Memento.Create()
                        .Register(article)
                        .Register(magazine)
                        .Register(publisher);
```

Now you can freely act upon your objects and if anything goes wrong, just rollback!

```csharp
try
{
   article.Name = "State of emergency declared";
   article.ReleaseDate = DateTime.UtcNow;
   db.Save(article);
}
catch(DBException)
{
   memento.Rollback();   //Name is now "Draft" and ReleaseDate is null
}
```

### Registering single properties

You can also register single properties, without the need to add attributes to your classes.

```csharp
var memento = Memento.Create()
                        .RegisterProperty(publisher, p => p.Name) //simple property
                        .RegisterProperty(publisher, p => p.ProfilePhoto.Description) //'deep' property
                        .RegisterProperty(() => ArticleFactory.LastReleaseDate) //static property
```



## NuGet
To install MementoContainer, run the following command in the Package Manager Console

```
PM> Install-Package MementoContainer
```

## Depency Injection and Mocking
The IMemento interface is available so that you can easily mock it with tools like [Moq](https://code.google.com/p/moq/) in your unit tests and inject it as a depency using your favourite IoC container (e.g., [Castle Windsor](http://docs.castleproject.org/Windsor.MainPage.ashx) or [Ninject](http://www.ninject.org/)).
