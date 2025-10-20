namespace HMS.Essentials.TestBase;

/// <summary>
/// Base class for all test classes in HMS.Essentials test projects.
/// Provides common test utilities and setup.
/// </summary>
public abstract class TestBaseClass
{
    /// <summary>
    /// Gets the current test name. Override in derived classes if needed.
    /// </summary>
    protected virtual string TestName => GetType().Name;
}
