namespace Common.Data
{
  /// <summary>
  /// Interface for all graphs
  /// </summary>
  public interface IGraph
  {
    /// <summary>
    /// Number of transitions between nodes
    /// </summary>
    int TransitionCount { get; }

  }

  /// <summary>
  /// Graph node
  /// </summary>
  public interface INode
  {
    /// <summary>
    /// Node Id
    /// </summary>
    int Id { get; }
  }
}
