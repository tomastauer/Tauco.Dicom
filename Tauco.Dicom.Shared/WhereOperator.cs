namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Specifies which operator will be used for constraint record.
    /// </summary>
    public enum WhereOperator
    {
        /// <summary>
        /// Item must equal exactly the value to satisfy the constraint.
        /// </summary>
        Equals,

        /// <summary>
        /// Items must contain the value to satisfy the constraint.
        /// </summary>
        Like
    }
}
