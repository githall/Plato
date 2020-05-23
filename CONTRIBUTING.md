# Contributing Guidelines

We’d love you to help us improve Plato. To help us keep Plato
high quality, we request that contributions adhere to the following guidelines.

- Try to include descriptive commit messages, even if you squash them before sending a pull request. Good commit messages are important. They tell others why you did the changes you did, not just right here and now, but months or years from now.
- Commit messages should be clear, concise and provide a reasonable summary to give an indication of what was changed and why.
- Avoid committing several unrelated changes in one go. It makes merging difficult, and also makes it harder to determine which change is the culprit if a bug crops up.
- If you aren't sure how something works or want to solicit input from other Plato developers before making a change, you can create an issue with the discussion tag or post your quesitons to https://plato.instantasp.co.uk/questions.
- Please include relevant unit tests / specs along with your changes, if appropriate.

## Commit Messages

Please try to indicate the type of commit by using one of the following prefixes with your commit message...

- FIX: A code fix
- REF: A code refactor
- FEAT: A new feature
- SEC: A security update
- PERF: Performance related work
- UX: A user experience fix or improvement
- UI: A user interface fix or improvement
- DEV: Tooling, project or dev changes
- BUILD: Changes to build or publishing pipeline
- LEGAL: A legal change
- DOC: A documentation change
- TYPO: A text change / fix

## C# Coding Conventions

- Use the default Resharper guidelines for code styling
- Start private fields with _, i.e. _camelCased
- Use PascalCase for public and protected properties and methods
- Avoid using this when accessing class variables, e.g. this.fieldName
- Ensure your folder structure matches your .NET namespacing strcuture
- Do not use protected fields - create a private field and a protected property instead
- Use allman style brackets for C# & JavaScript code
- Use tabs not spaces if possible :)
- When documenting code, please use the standard .NET convention of XML documentation comments
- Line Length < 80 - For readability, avoid lines longer than 80 characters
- Always use tabs, not spaces for indentation of code blocks.

## JavaScript Coding Conventions
 
- Use camelCase for all property and method names
-  Always use tabs, not spaces for indentation of code blocks.
- Use anonymous functions to encapulate scope
- Always put spaces around operators ( = + - * / ), and after commas
- Line Length < 80 - For readability, avoid lines longer than 80 characters
- Always declare variables at the top of the current scope
- Hyphens are not allowed in JavaScript names.
- Never use document.write
- Avoid using window.alert, window.confifm
- Ensure console.log messages are removed

**Statement Rules**

- Always end a simple statement with a semicolon. 

For example

```
var values = ["Volvo", "Saab", "Fiat"];

var person = {
    firstName: "John",
    lastName: "Doe",
    age: 50,
    eyeColor: "blue"
};
```

General rules for complex (compound) statements:

- Put the opening bracket at the end of the first line.
- Use one space before the opening bracket.
- Put the closing bracket on a new line, without leading spaces.
- Do not end a complex statement with a semicolon.

The following examples demonstate this...

```
function toCelsius(fahrenheit) {
    return (5 / 9) * (fahrenheit - 32);
}

for (i = 0; i < 5; i++) {
    x += i;
}
```

## CSS Coding Conventions

- Always use lower case CSS class names seperated with a hypen. For example "header-right", "header-body-content" are good whilst "headerRight", "HeaderBodyContent" are bad. 
- Use vendor specific prefixes whenever possible
- Line Length < 80 - For readability, avoid lines longer than 80 characters.

**Shorthand**

Whenever possible avoid using short hand. Short hand can make it hard to reason about the CSS. 

**Good CSS**

```
.body {
    font-family: Arial;
    font-size: 100%;
    font-weight:bold
}
```

**Bad CSS**

```
.body {
    font: bold 100% Arial;
}
```

**Simple CSS**

For CSS fewer than 80 characters in length you should include this on a single line. 

**Good CSS**

```
.body { background-color:red; }
```

**Bad CSS**

```
.body {
    background-color:red; 
}
```

**Complex CSS-**

- Separate multiple classes that use the same CSS onto new lines
- Put the opening bracket at the end of the first line.
- Use one space before the opening bracket.
- Put the closing bracket on a new line, without leading spaces.

**Good CSS**

```
.header-left,
.header-center,
.header-right {
    display: inline-block;
}
```

**Bad CSS**

```
.header-left,.header-center,.header-right {display: inline-block;}
```

## HTML Coding Conventions

- Avoid include JavaScript and CSS
- Always use camelCase for HTML element ids
- Always ensure "data-" is prefixed for HTML 5 data attributes
- Avoid using the style attribute on HTML elements, CSS classes should be used to control elemrnt styling
- Avoid 2 or more elements on the same page the same id
- Avoid using CSS class names or HTML element ids to initialize client side JavaScript. 

## Naming Conventions

Always use the same naming convention for all your code. 

## Design Patterns

When developing functionality or refactoring code please always consider the SOLID principals and try to follow existing established design patterns. Code should be clean, easy to read by humans and easy to reason about with minimal side effects. 

The **SOLID** principals are:-

- **Single-responsibility** - A class should have one and only one reason to change, meaning that a class should have only one job.
- **Open-closed** - Objects should be open for extension but closed for modification.
- **Liskov substitution** - Every derived class should be substitutable for their base / parent class.
- **Interface segregation** - A client should never be forced to implement an interface that it doesn't use or clients shouldn't be forced to depend on methods they do not use.
- **Dependency Inversion** - Object should depend on abstractions (interfaces) and not concreate implementations.

## Common Design Patterns

**Adapter**

Convert the interface of a class into another interface clients expect. Adapter lets classes work together that could not otherwise because of incompatible interfaces.

**Abstract Factory**

Where the Factory pattern only affects one class, the Abstract Factory pattern affects a whole bunch of classes. 

**Builder**

Separate the construction of a complex object from its representation allowing the same construction process to create various representations.

**Bridge**

Decouple an abstraction from its implementation allowing the two to vary independently.

**Command**

Encapsulate a request as an object, thereby letting you parameterize clients with different requests, queue, or log requests, and support undoable operations. To understand the idea behind the Command pattern, consider the following restaurant example: A customer goes to a restaurant and orders some food. The waiter takes the order (command, in this case) and hands it to the cook in the kitchen. In the kitchen the command is executed, and depending on the command different food or drink is being prepared.

**Composite**

The Composite pattern is very widespread. Basically, it is a list that may contain objects, but also lists. A typical example is a file system, which may consist of directories and files. Here directories may contain files, but also may contain other directories. Other examples of the Composite pattern are menus that may contain other menus, or in user management one often has users and groups, where groups may contain users, but also other groups.

**Decorator** 

Attach additional responsibilities to an object dynamically keeping the same interface. Decorators provide a flexible alternative to subclassing for extending functionality.

**Facade**

Provide a unified interface to a set of interfaces in a subsystem. Facade defines a higher-level interface that makes the subsystem easier to use.

**Front Controller**

Provide a unified interface to a set of interfaces in a subsystem. Front Controller defines a higher-level interface that makes the subsystem easier to use.

**Factory Method**

The Factory pattern creates an object from a set of similar classes.

**Iterator**

Nowadays, the Iterator pattern is trivial: it allows you to go through a list of objects, starting at the beginning, iterating through the list one element after the other, until reaching the end.

**Mediator or Manager**

Define an object that encapsulates how a set of objects interact. Mediator promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently.

**Null object**

Avoid null references by providing a default object.

**Observer**

The Observer pattern is one of the most popular patterns, and it has many variants. Assume you have a table in a spreadsheet. That data can be displayed in table form, but also in form of some graph or histogram. If the underlying data changes, not only the table view has to change, but you also expect the histogram to change. To communicate these changes you can use the Observer pattern: the underlying data is the observable and the table view as well as the histogram view are observers that observe the observable. 

**Proxy**

The idea behind the Proxy pattern is that we have some complex object and we need to make it simpler. One typical application is an object that exists on another machine, but you want to give the impression as if the user is dealing with a local object. Another application is when an object would take a long time to create (like loading a large image/video), but the actual object may never be needed. In this case a proxy represents the object until it is needed.

**Singleton**

This is one of the most dangerous design patterns, when in doubt do not use it. Its main purpose is to guarantee that only one instance of a object exists. Possible applications are a printer manager or a database connection manager. It is useful when access to a limited resource needs to be controlled.

**State**

In the State pattern, an internal state of the object influences its behaviour. Assume you have some drawing program in which you want to be able to draw straight lines and dotted lines. Instead of creating different classes for lines, you have one Line class that has an internal state called 'dotted' or 'straight' and depending on this internal state either dotted lines or straight lines are drawn. 

**Strategy**

Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it.

**Template Method**

This pattern is rather simple: as soon as you define an abstract class, that forces its subclasses to implement some method, you are using a simple form of the Template pattern.

**Visitor** 

Represent an operation to be performed on the elements of an object structure. Visitor lets you define a new operation without changing the classes of the elements on which it operates. 

## Concurrency Patterns

**Asynchronous Code**

For unpredictable operations always use asynchronous coding patterns. For example when accessing the file system or connecting to the database. 

**Lock**

One thread puts a "lock" on a resource, preventing other threads from accessing or modifying it

**Double-checked locking**

Reduce the overhead of acquiring a lock by first testing the locking criterion (the 'lock hint') in an unsafe manner; only if that succeeds does the actual lock proceed. Can be unsafe when implemented in some language/hardware combinations. It can therefore sometimes be considered an anti-pattern.

**Read-write lock**

Allows concurrent read access to an object but requires exclusive access for write operations.
**Monitor object**

An object whose methods are subject to mutual exclusion, thus preventing multiple objects from erroneously trying to use it at the same time.

## Patterns in Practice

Design patterns can speed up the development process by providing tested, proven development paradigms. 

Effective software design requires considering issues that may not become visible until later in the implementation. 

Reusing design patterns helps to prevent subtle issues that can cause major problems, and it also improves code readability for those who are familiar with the patterns. 

In addition to this, patterns allow developers to communicate using well-known, well understood names for software interactions.

To achieve flexibility, design patterns usually introduce additional levels of indirection, which in some cases may complicate the resulting designs but ultimately lead to more flexible, maintainable code. 

## Unacceptable API Changes

The following types of API changes will generally not be accepted:

- Any modification to a commonly used public interface
- Changing any public method signature or removing any public members
- Renaming public classes or members
- Changing an access modifier from public to private / internal / protected