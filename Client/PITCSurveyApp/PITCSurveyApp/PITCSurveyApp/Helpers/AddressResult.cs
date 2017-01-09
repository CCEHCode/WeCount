namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// An address result from the Bing geocoder service.
    /// </summary>
    public class AddressResult
    {
        /// <summary>
        /// The address line.
        /// </summary>
        public string AddressLine { get; set; }

        /// <summary>
        /// The locality.
        /// </summary>
        /// <remarks>
        /// Roughly a city or town.
        /// </remarks>
        public string Locality { get; set; }

        /// <summary>
        /// The admin district.
        /// </summary>
        /// <remarks>
        /// Roughly a state, province, or other similar area.
        /// </remarks>
        public string AdminDistrict { get; set; }

        /// <summary>
        /// The postal code.
        /// </summary>
        public string PostalCode { get; set; }
    }
}
