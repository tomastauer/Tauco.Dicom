using System.Collections.Generic;

namespace Tauco.Dicom.Models.InfoObjects
{
    /// <summary>
    /// Specifies that the <typeparamref name="TInfo"/> can be present in several instances, however all instances should be considered as the same object.
    /// </summary>
    /// <typeparam name="TInfo">Type of the object</typeparam>
    public interface IMultipleInstance<TInfo>
    {
        /// <summary>
        /// Gets hash code identifier of the <typeparamref name="TInfo"/> identifier.
        /// </summary>
        /// <returns>Hash code of the <typeparamref name="TInfo"/> identifier</returns>
        int GetIdentifierHashCode();
        
        /// <summary>
        /// Represents collection of another instances that should be considered the same as the original.
        /// </summary>
        IList<TInfo> AdditionalInstances
        {
            get;
        }
    }
}
