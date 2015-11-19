namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Provides method for obtaining single instance of every implementation of <see cref="IDicomInfo"/>.
    /// </summary>
    public interface IGeneralizedInfoProvider
    {
        /// <summary>
        /// Returns instance of <typeparamref name="TInfo"/>.
        /// </summary>
        /// <typeparam name="TInfo">Object info to be obtained</typeparam>
        /// <returns>Instance of <typeparamref name="TInfo"/></returns>
        TInfo GetGeneralizedInfo<TInfo>() where TInfo : IDicomInfo;
    }
}