namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents single constraint item within the request constraint collection.
    /// </summary>
    public class WhereItem
    {
        /// <summary>
        /// Specifies which DICOM tag is used in the constraint.
        /// </summary>
        public DicomTags DicomTag
        {
            get;
            set;
        }


        /// <summary>
        /// Specifies relevant value of the constraint. Depending on the <see cref="Operator"/> the value has to exact match, ar be contained within the requested value.
        /// </summary>
        public object Value
        {
            get;
            set;
        }


        /// <summary>
        /// Specifies binary operation performed between the <see cref="Value"/> and requested value.
        /// </summary>
        public WhereOperator Operator
        {
            get;
            set;
        }

    }
}
