# Contributing guidelines

We’d love you to help us improve Plato. To help us keep this Plato
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

## Coding conventions

- Use the default Resharper guidelines for code styling
- Start private fields with _, i.e. _camelCased
- Use PascalCase for public and protected properties and methods
- Avoid using this when accessing class variables, e.g. this.fieldName
- Ensure your folder structure matches your .NET namespacing strcuture
- Do not use protected fields - create a private field and a protected property instead
- Use allman style brackets for C# & JavaScript code
- Use tabs not spaces if possible :)
- When documenting code, please use the standard .NET convention of XML documentation comments

## Unacceptable API Changes

The following types of API changes will generally not be accepted:

- Any modification to a commonly used public interface
- Changing any public method signature or removing any public members
- Renaming public classes or members
- Changing an access modifier from public to private / internal / protected