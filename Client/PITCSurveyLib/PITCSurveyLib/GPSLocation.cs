namespace PITCSurveyLib
{
    public class GPSLocation
    {
        public double? Lat { get; set; }
        public double? Lon { get; set; }

        /// <summary>
        /// Identifies the estimated accuracy of the GPS location info. Represented as a "circle of confidence".
        /// </summary>
        /// <remarks>
        /// For Android:
        ///        We define accuracy as the radius of 68% confidence. In other words, if you draw a circle centered
        ///        at this location's latitude and longitude, and with a radius equal to the accuracy, then there is
        ///        a 68% probability that the true location is inside the circle.
        ///        
        ///        In statistical terms, it is assumed that location errors are random with a normal distribution,
        ///        so the 68% confidence circle represents one standard deviation.Note that in practice, location
        ///        errors do not always follow such a simple distribution.
        ///        
        ///        This accuracy estimation is only concerned with horizontal accuracy, and does not indicate the
        ///        accuracy of bearing, velocity or altitude if those are included in this Location.
        ///        
        ///     If this location does not have an accuracy, then 0.0 is returned.
        /// </remarks>
        public float Accuracy { get; set; }
    }
}
