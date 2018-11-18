## Coding guidelines
### Tabs
- **Size**: 2
- **Type**: Space
## Brackets

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
    
## Properties
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
    
    