# Contributing Guidelines

We’d love you to help us improve Plato. To help us keep Plato
high quality, we request that contributions adhere to the following guidelines.

- Try to include descriptive commit messages, even if you squash them before sending a pull request. Good commit messages are important. They tell others why you did the changes you did, not just right here and now, but months or years from now.
- Commit messages should be clear, concise and provide a reasonable summary to give an indication of what was changed and why.
- Avoid committing several unrelated changes in one go. It makes merging difficult, and also makes it harder to determine which change is the culprit if a bug crops up.
- If you aren't sure how something works or want to solicit input from other developers before making a change, you can create an issue with the discussion tag or post your quesitons to https://plato.instantasp.co.uk/questions.
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

## General Guidance

Code should be clean, easy to read by humans and easy to reason about, improve & maintain now and in the future. Code should be loosely coupled and have clear separation of concerns.  

## Minimize Dependencies

Whenever possible build on top of established existing primitives & avoid dependencies on 3rd party libraries or frameworks that are likely to evolve quickly or become obsolete overtime. 

We don’t want to be fighting a framework to bend to our will nor do we want to be forced to make sweeping changes to the code to support an updated or obsolete dependency. 

## C# Coding Conventions

- Use the default Resharper guidelines for code styling
- Start private fields with _, i.e. _camelCased
- Use PascalCase for public and protected properties and methods
- Avoid using this when accessing class variables, e.g. this.fieldName
- Ensure your folder structure matches your .NET namespacing strcuture
- Do not use protected fields - create a private field and a protected property instead
- Use allman style brackets
- Use tabs not spaces
- When documenting code, please use the standard .NET convention of XML documentation comments
- Line Length < 80 - For readability, avoid lines longer than 80 characters
- Always use tabs, not spaces for indentation of code blocks.

## JavaScript Coding Conventions
 
- Ensure ReSharper is enabled to catch obvious JavaScript issues
- Use camelCase for all property and method names
-  Always use tabs, not spaces for indentation of code blocks.
- Use anonymous functions to encapulate scope
- Always put spaces around operators ( = + - * / ), and after commas
- Line Length < 80 - For readability, avoid lines longer than 80 characters
- Use "the one true brace style" (1TBS) for JavaScript code
- Due to hoisting always declare variables at the top of the current scope
- Ensure variables are not declared multiple times in the same scope, this is common if you have multiple for loops and the interator variable is declared multiple times i.e. for (var i = 0; ... )
- Hyphens are not allowed in JavaScript names.
- Production JavaScript should always be minified
- Avoid using window.alert, window.confifm
- Ensure console.log messages are removed from production code
- Avoid using class names or element ids within HTML mark-up to initialize client side JavaScript, instead use data attributes that are less likely to change. 
- Never use document.write and avoid building the DOM via JavaScript, HTML should always be constructed on the server and sent to the client for display

**Statement Rules**

- Always end a simple JavaScript statement with a semicolon. 

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

General rules for complex (compound) JavaScript statements:

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

- Always use lower case CSS class names seperated with a hypen. For example "header-right", "header-body-content" are good whilst "headerRight", "HeaderBodyContent" are bad - above all be consistent. 
- Keep all CSS organized, ensure related CSS is grouped together and named in a similar fashion.
- Use vendor specific prefixes whenever possible
- Production CSS should always be minified
- Avoid using CSS pre-processors (Sass, Scss, LESS)
- Avoid using !important to override CSS properties and instead use a more specific CSS selector
- Line Length < 80 - For readability, avoid lines longer than 80 characters.

**Reuse**

Avoid adding CSS just to style a single element and instead consider if this could be broken into smaller more generic CSS classes that could be reused throughout the HTML in other areas.

For example consider the following HTML & CSS...

```
<div class="myelement"></div>
```

```
.myelement { 
    background:red; 
    color: white;
    overflow: hidden; 
    max-height: 600px;
} 
```

This could be made much more generic & reuable like so...

```
<div class="bg-red text-white overflow-hidden max-h-600"></div>
```

```
.bg-red { background:red; }
.text-white { color: white; }
.overflow-hidden { overflow: hidden; }
.max-h-600 { max-height: 600px; }
```

Whilst this may result in more verbose HTML class attributes this approach promotes reuse, speeds up development and keep the CSS size down to a minimum. This is a similar pattern to that used by Bootstrap & other popular front-end CSS frameworks.

**Combine CSS class names**

Whenever possible combine class names that share common styles. 

For example...

**Good CSS**

```
.el1,
.el2 { margin-top: 12px; } 
```

**Bad CSS**

```
.el1 { margin-top: 12px; } 
.el2 { margin-top: 12px; } 
```

**CSS Shorthand**

One feature of CSS is the ability to use shorthand properties and values. Most properties and values have acceptable shorthand alternatives. To keep CSS files small whenever possible you should use CSS shorthand to reduce the overall size of the final CSS. 

For example...

**Good CSS**

```
body { font: bold 100% Arial; }
.element { margin: 12px 24px; }
```

**Bad CSS**

```
body {
    font-family: Arial;
    font-size: 100%;
    font-weight:bold
}

.element {
    margin-top: 12px;
    margin-bottom: 12px;
    margin-left: 24px;
    margin-right: 24px;
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

- Combine CSS class names & separate multiple classes that share the same CSS onto new lines
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

All HTML should be well-formed and W3C validated HTML 5.

- Use proper document structure
- Try to minimize complex nesting 
- Declare the correct HTML 5 doctype
- Always close tags
- Always use lower case for HTML elements
- Use alt attribute with images
- Use title attribute whenever possible
- Place external style sheets within the &lt;head&gt; tag
- Place external JavaScript just above the closing &lt;/body&gt; tag
- Avoid adding external JavaScript in the  &lt;head&gt; tag
- Avoid inline &lt;script&gt; tags - alays use external JS
- Avoid inline &lt;style&gt; tags - always use external CSS
- Avoid inline styling. Avoid using the style attribute on HTML elements, external CSS classes should be used to control elemrnt styling
- Always use camelCase for HTML element ids
- Always ensure the "data" prefix is present for all HTML 5 data attributes, data-provide="hello" is good whilst provide="hello" is bad
- Avoid 2 or more elements on the same page with the same id
- Validate all HTML mark-up against the W3C Markup Validation Service (https://validator.w3.org/)
- Whenever possible add support for Accessible Rich Internet Applications or ARIA attributes (https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA)

**HTML Semantics**

Semantic elements = elements with a meaning.

Use semantically correct HTML to represent elements. A semantic element clearly describes its meaning to both the browser and the developer. 

Examples of non-semantic elements include &lt;div&gt; and &lt;span&gt; - these elements tell us nothing about the content.

Examples of semantic elements include &lt;h1&gt;, &lt;p&gt;, &lt;form&gt;, &lt;table&gt;,, &lt;header&gt;, &lt;footer&gt;, &lt;main&gt;, &lt;article&gt; etc - these elements more clearly describe the content contained within them.

For further information please refer to https://www.w3schools.com/html/html5_semantic_elements.asp

## Naming Conventions

Always use a consistent naming convention for all your code. 

## Automated Testing

There are numerous benefits to writing unit tests; they help with regression, provide documentation, and facilitate good design. 

Whenever possible code should be accompanied by separate unit tests using a standardized unit testing framework. 

Characteristics of a good unit test...

- **Fast.** It is not uncommon for mature projects to have thousands of unit tests. Unit tests should take very little time to run. Milliseconds.
- **Isolated.** Unit tests are standalone, can be run in isolation, and have no dependencies on any outside factors such as a file system or database.
- **Repeatable.** Running a unit test should be consistent with its results, that is, it always returns the same result if you do not change anything in between runs.
- **Self-Checking.** The test should be able to automatically detect if it passed or failed without any human interaction.
- **Timely.** A unit test should not take a disproportionately long time to write compared to the code being tested. If you find testing the code taking a large amount of time compared to writing the code, consider a design that is more testable.

**Naming your tests**

The name of your test should consist of three parts:

- The name of the method being tested.
- The scenario under which it's being tested.
- The expected behavior when the scenario is invoked.

**Bad**

```
[Fact]
public void Test_Single()
{
    var stringCalculator = new StringCalculator();

    var actual = stringCalculator.Add("0");

    Assert.Equal(0, actual);
}
```

**Better (but not perfect)**

```
[Fact]
public void Add_SingleNumber_ReturnsSameNumber()
{
    var stringCalculator = new StringCalculator();

    var actual = stringCalculator.Add("0");

    Assert.Equal(0, actual);
}
```

For further guidance on writting good unit tests please see https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices

##  SOLID principals

When developing functionality or refactoring code please always consider the SOLID principals and try to follow these principals and existing well known design patterns. 

The SOLID principals are:-

- **Single-responsibility** - A class should have one and only one reason to change, meaning that a class should have only one job.
- **Open-closed** - Objects should be open for extension but closed for modification.
- **Liskov substitution** - Every derived class should be substitutable for their base / parent class.
- **Interface segregation** - A client should never be forced to implement an interface that it doesn't use or clients shouldn't be forced to depend on methods they do not use.
- **Dependency Inversion** - Object should depend on abstractions (interfaces) and not concreate implementations.

## Common Design Patterns

Whenever possible attempt to leverage an existing known design pattern. Common design patterns are detailed below.

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

**Provider**

The provider model is a design pattern formulated by Microsoft to allow an application to choose from one of multiple implementations or "condiments" in the application configuration, for example, to provide access to different data stores to retrieve login information, or to use different storage methodologies such as a database, binary to disk, XML, etc. The implementations to use are provided at run-time.

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

For unpredictable operations always use asynchronous coding patterns. For example when accessing the file system,  connecting to a database or making a HTTP request. 

**Lock**

One thread puts a "lock" on a resource, preventing other threads from accessing or modifying it

**Double-check locking**

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

To achieve flexibility, design patterns usually introduce additional levels of indirection, which in some cases may complicate the resulting designs but ultimately lead to more malleable & maintainable code. 

## Interfaces vs Abstract Base Classes

If an interface is likely to evolve overtime and is used in many areas of the code, consider providing an abstract base class that implements the interface that other classes can derive from to also implement the interface. This makes the code less brittle and allows us to evolve interfaces overtime with minimal impact. 

## General unacceptable changes

The following types of changes will generally not be accepted or will requir more review:-

- Any modification to a commonly used public interface
- Any modification to a commonly used base class
- Changing any public method signature or removing any public members
- Renaming public classes or members
- Changing an access modifier from public to private / internal / protected


