namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Service for parsing czech birth numbers.
    /// </summary>
    public interface IBirthNumberParser
    {
        /// <summary>
        /// Parses given <paramref name="birthNumber"/> and returns new instance of <see cref="BirthNumber"/> with the values obtained from the birth number.
        /// </summary>
        /// <param name="birthNumber">Birth number to be parsed</param>
        /// <returns>
        /// Instance of <see cref="BirthNumber"/> containing data obtained from <paramref name="birthNumber"/>, if <paramref name="birthNumber"/> is in valid format; otherwise, retuns null
        /// </returns>
        BirthNumber GetBirthNumber(string birthNumber);
    }
}