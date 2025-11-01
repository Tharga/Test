namespace Tharga.Test.Toolkit
{
    public interface IAssignmentIssue
    {
        string Message { get; }
        string ObjectName { get; }
        int? Index { get; }
    }
}