namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Provides method for selecting proper implementation of <see cref="ITypeHandler"/> based on given type.
    /// </summary>
    internal interface ITypeHandlerSelector
    {
        /// <summary>
        /// Selects correct implementation of <see cref="ITypeHandler"/> for given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of handler to be selected</param>
        /// <returns>Implementation of <see cref="ITypeHandler"/> for given <paramref name="type"/></returns>
        ITypeHandler SelectTypeHandler(string type);
    }
}