# Contents
- [Issue](#issues)
    - [Reporting a bug](#reporting-a-bug)
        - [Issue description](#issue-description)
        - [Steps to reproduce](#str)
    - [Requesting an enhancement](#requesting-an-enhancement)
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

## Issues
- Guidelines for submitting issues

### Reporting a bug
> [Bug report template](https://gitlab.com/hailstorm75/Common/snippets/1781777)  

- A bug report must have at least two parts:
    - [Description](#issue-description)
    - [Steps to reproduce (STR)](#str)
- Additional information, screenshots are not mandatory but are very welcom

#### Issue description
- Describe the issue effects and conditions
    - Example: During the download operation the progress bar percentage value started decreasing and went under 0
- Suggest, if possible, what you think may be the cause if the issue
    - Example: Probably the percentage calculation function isn't working properly

#### STR
- Describe the steps required to reproduce the bug
    - Example:
        1. Open MyProgram
        2. Click Download
        3. The progress bar doesn't work as intended
- Include reproducibility
    - Examples: always, once, random

### Requesting an enhancement
- Provide a name of the libary for which the enhancement should be made
- If the given library doesn't yet exist, explain why it should be created and whether it should be merged with an existing library
- Describe the enhancement
- Provide use case example if possible

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
[Test]
[Category(Constants.METHOD)]
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
		- Class methods must be `static` and can either be `internal` or `public`
    - Class methods must return `IEnumerable<object[]>`
2. Add a DynamicData attribute to your test method and add arguments to your method which accept the retrieved data:
```csharp
[Test]
[Category(Constants.METHOD)]
[TestCaseSource(typeof(DataMyClass), nameof(DataMyClass.GetTestData))]
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
    return new object[]   // First dataset
    {
      123,    // Will be received by the first argument, inputValue
      321     // Will be received by the second argument, expected
    };
    yield return new object[]   // Second dataset
    {
      456,
      654
    };
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
    