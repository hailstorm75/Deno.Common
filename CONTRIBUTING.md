# Contents
- [Coding guidelines](#coding-guidelines)
    - [Tabs](#tabs)
    - [Brackets](#brackets)
    - [Banned keywords](#banned-keywords)
    - [Properties](#properties)
    - [Fields](#fields)
- [Unit Tests](#unit-tests)
    - [How to write UTs](#how-to-write-unit-tests)
        - [Data-driven UTs](#data-driven-unit-tests)
- [Commits](#commits)
    - [Before working](#before-making-any-changes)
    - [Commiting a change](#how-to-commit-a-change)

## Coding guidelines
### Tabs
- **Size**: 2
- **Type**: Space
### Brackets

**Acceptable**:

```csharp
public void Foo()
{

}
```

**Unacceptable**:

```csharp
public void Foo() {

}
```

### Banned keywords
- `goto`
    
### Properties
- Name with camel case
    - Example: `MyProperty`
- Must not execute large amounts of code or query data
    - If such a need is necessary replace the property with a method: `MyProperty` => `GetMyProperty()`

### Fields
- Name with camel case and 'm_' prefix
    - Example: `m_myField`
- If the field is a constant name with all caps and pascal case
    - Example: `MY_CONSTANT'
- Properties must be **private**

## Unit Tests
- Every class should have its own dedicated unit test suit
- Tests for a class in a library called 'A' must be in a test library called 'UnitTestA' and located in the Tests folder of the solution

### How to write unit tests
- A test class for a class called 'MyClass' must be called 'UtMyClass'
- Every test method must be categorized
    - Use the constants provided in UnitTestConstants library

**Example**:
```csharp
[TestMethod, TestCategory(Constants.METHOD)]
public void TestMyMethod()
{
    // TODO
}
```
- If something needs to be tested for multiple values utilize [data-driven testing](#data-driven-unit-tests)

#### Data-driven Unit Tests
1. Create a new Data class for your tested class if it doesn't exist
    - It must be located in the Data folder of the test library
    - It must be called Data + the name of the class you are testing
        - Example: DataMyClass
    - It must be `static` and `internal`
2. Add a DynamicData attribute to your test method and add arguments to your method which accept the retrieved data:
```csharp
[TestMethod, TestCategory(Constants.METHOD)]
[DyanmicData(nameof(DataMyClass.GetTestData), typeof(DataMyClass), DyanamicDataSourceType.Method)]
public void TestMyMethod(int inputValue, int expected)
{
    // TODO
}
```
3. Add a method to your Data class which will provide the test data:
```csharp
internal static class DataMyClass
{
    internal static IEnumerable<object[]> GetTestData()
    {
        yield return new object[]   // First dataset
        {
            123,    // Will be received by the first argument, inputValue
            321     // Will be received by the second argument, expected
        },
        yield return new object[]   // Second dataset
        {
            456,
            654
        }
    }
}
```

## Commits
### Before making any changes
- Sync your branch
- Build
- Run tests
- Report issues if any were found

### How to commit a change
1. Build
2. Fix build errors AND warnings if any are present
3. Run tests
4. Fix commit to satisfy tests if any failed
5. Stage your changes
6. Write the commit message
    - If you are solving an issue, write the issue number followed by a dash and your commit message.
        - Example "#0 - Refactor MyClass"
    - Keep the message short and descriptive
    - Write the message in imperative mood
        - Examples: "Add file", "Fix issue", "Resolve conflicts"
    
    