namespace Tharga.Test.Toolkit
{
    /// <summary>
    /// A field or property that holds its default value, as reported by <see cref="AssignmentExtension.AssignmentIssues"/>.
    /// </summary>
    public interface IAssignmentIssue
    {
        /// <summary>
        /// A description that names the full path of the member, for example <c>"The field 'Customer.Name' is not assigned."</c>
        /// for a <c>null</c> member, or <c>"Value of type Int32 in 'Customer.Id' is not assigned."</c> for a member that holds
        /// its default value.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Where the issue was found. For a <c>null</c> member it is the path of the object that holds the member
        /// (<c>Customer</c> for a null <c>Customer.Name</c>). For a member that holds its default value it is the path of the
        /// member itself (<c>Customer.Id</c>).
        /// </summary>
        string ObjectName { get; }

        /// <summary>
        /// The position of the collection element the issue was found in, or <c>null</c> when it is not inside a collection.
        /// </summary>
        int? Index { get; }
    }
}