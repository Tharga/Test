namespace Tharga.Test.Toolkit
{
    public class AssignmentIssue : IAssignmentIssue
    {
        public AssignmentIssue(string objectName, string message, int? index)
        {
            ObjectName = objectName ?? "N/A";
            Message = message;
            Index = index;

            if (ObjectName.Contains("[]") && index != null)
            {
                ObjectName = ObjectName.Replace("[]", $"[{index}]");
            }
        }

        public string ObjectName { get; }
        public int? Index { get; }
        public string Message { get; }
    }
}