﻿using System;
using System.Globalization;

namespace Wikiled.Twitter.Persistency.Data
{
    /// <summary>
    ///     Represents a geographical location that is determined by latitude and longitude
    ///     coordinates. May also include altitude, accuracy, speed, and course information.
    /// </summary>
    public class GeoCoordinate : IEquatable<GeoCoordinate>
    {
        /// <summary>
        ///     Represents a <see cref="GeoCoordinate" /> object that has unknown latitude and longitude fields.
        /// </summary>
        public static readonly GeoCoordinate Unknown = new GeoCoordinate();

        private double course;

        private double horizontalAccuracy;

        private double latitude;

        private double longitude;

        private double speed;

        private double verticalAccuracy;

        /// <summary>
        ///     Initializes a new instance of GeoCoordinate that has no data fields set.
        /// </summary>
        public GeoCoordinate()
            : this(double.NaN, double.NaN)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the GeoCoordinate class from latitude and longitude data.
        /// </summary>
        /// <param name="latitude">The latitude of the location. May range from -90.0 to 90.0. </param>
        /// <param name="longitude">The longitude of the location. May range from -180.0 to 180.0.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Latitude or longitude is out of range.</exception>
        public GeoCoordinate(double latitude, double longitude)
            : this(latitude, longitude, double.NaN)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the GeoCoordinate class from latitude, longitude, and altitude data.
        /// </summary>
        /// <param name="latitude">Latitude. May range from -90.0 to 90.0.</param>
        /// <param name="longitude">Longitude. May range from -180.0 to 180.0</param>
        /// <param name="altitude">The altitude in meters. May be negative, 0, positive, or Double.NaN, if unknown.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     latitude, longitude or altitude is out of range.
        /// </exception>
        public GeoCoordinate(double latitude, double longitude, double altitude)
            : this(latitude, longitude, altitude, double.NaN, double.NaN, double.NaN, double.NaN)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the GeoCoordinate class from latitude, longitude, altitude, horizontal accuracy,
        ///     vertical accuracy, speed, and course.
        /// </summary>
        /// <param name="latitude">The latitude of the location. May range from -90.0 to 90.0.</param>
        /// <param name="longitude">The longitude of the location. May range from -180.0 to 180.0.</param>
        /// <param name="altitude">The altitude in meters. May be negative, 0, positive, or Double.NaN, if unknown.</param>
        /// <param name="horizontalAccuracy">
        ///     The accuracy of the latitude and longitude coordinates, in meters. Must be greater
        ///     than or equal to 0. If a value of 0 is supplied to this constructor, the HorizontalAccuracy property will be set to
        ///     Double.NaN.
        /// </param>
        /// <param name="verticalAccuracy">
        ///     The accuracy of the altitude, in meters. Must be greater than or equal to 0. If a value
        ///     of 0 is supplied to this constructor, the VerticalAccuracy property will be set to Double.NaN.
        /// </param>
        /// <param name="speed">
        ///     The speed measured in meters per second. May be negative, 0, positive, or Double.NaN, if unknown.
        ///     A negative speed can indicate moving in reverse.
        /// </param>
        /// <param name="course">
        ///     The direction of travel, rather than orientation. This parameter is measured in degrees relative
        ///     to true north. Must range from 0 to 360.0, or be Double.NaN.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     If latitude, longitude, horizontalAccuracy, verticalAccuracy, course is out of range.
        /// </exception>
        public GeoCoordinate(
            double latitude,
            double longitude,
            double altitude,
            double horizontalAccuracy,
            double verticalAccuracy,
            double speed,
            double course)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            HorizontalAccuracy = horizontalAccuracy;
            VerticalAccuracy = verticalAccuracy;
            Speed = speed;
            Course = course;
        }

        /// <summary>
        ///     Gets the altitude of the GeoCoordinate, in meters.
        /// </summary>
        /// <returns>
        ///     The altitude, in meters.
        /// </returns>
        public double Altitude { get; set; }

        /// <summary>
        ///     Gets or sets the heading in degrees, relative to true north.
        /// </summary>
        /// <returns>
        ///     The heading in degrees, relative to true north.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Course is set outside the valid range.</exception>
        public double Course
        {
            get => course;
            set
            {
                if (!double.IsNaN(value) && (value < 0.0 || value > 360.0))
                {
                    throw new ArgumentOutOfRangeException("course", "Argument must be in range 0 to 360");
                }

                course = value;
            }
        }

        /// <summary>
        ///     Gets or sets the accuracy of the latitude and longitude that is given by the GeoCoordinate, in meters.
        /// </summary>
        /// <returns>
        ///     The accuracy of the latitude and longitude, in meters.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">HorizontalAccuracy is set outside the valid range.</exception>
        public double HorizontalAccuracy
        {
            get => horizontalAccuracy;
            set
            {
                if (!double.IsNaN(value) && value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("HorizontalAccuracy", "Argument must be non negative");
                }

                horizontalAccuracy = value == 0.0 ? double.NaN : value;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the GeoCoordinate does not contain latitude or longitude data.
        /// </summary>
        /// <returns>
        ///     true if the GeoCoordinate does not contain latitude or longitude data; otherwise, false.
        /// </returns>
        public bool IsUnknown => Equals(Unknown);

        /// <summary>
        ///     Gets or sets the latitude of the GeoCoordinate.
        /// </summary>
        /// <returns>
        ///     Latitude of the location.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Latitude is set outside the valid range.</exception>
        public double Latitude
        {
            get => latitude;
            set
            {
                if (!double.IsNaN(value) && (value > 90.0 || value < -90.0))
                {
                    throw new ArgumentOutOfRangeException("Latitude", "Argument must be in range of -90 to 90");
                }

                latitude = value;
            }
        }

        /// <summary>
        ///     Gets or sets the longitude of the GeoCoordinate.
        /// </summary>
        /// <returns>
        ///     The longitude.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Longitude is set outside the valid range.</exception>
        public double Longitude
        {
            get => longitude;
            set
            {
                if (!double.IsNaN(value) && (value > 180.0 || value < -180.0))
                {
                    throw new ArgumentOutOfRangeException("Longitude", "Argument must be in range of -180 to 180");
                }

                longitude = value;
            }
        }

        /// <summary>
        ///     Gets or sets the speed in meters per second.
        /// </summary>
        /// <returns>
        ///     The speed in meters per second. The speed must be greater than or equal to zero, or Double.NaN.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Speed is set outside the valid range.</exception>
        public double Speed
        {
            get => speed;
            set
            {
                if (!double.IsNaN(value) && value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("speed", "Argument must be non negative");
                }

                speed = value;
            }
        }

        /// <summary>
        ///     Gets or sets the accuracy of the altitude given by the GeoCoordinate, in meters.
        /// </summary>
        /// <returns>
        ///     The accuracy of the altitude, in meters.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">VerticalAccuracy is set outside the valid range.</exception>
        public double VerticalAccuracy
        {
            get => verticalAccuracy;
            set
            {
                if (!double.IsNaN(value) && value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("VerticalAccuracy", "Argument must be non negative");
                }

                verticalAccuracy = value == 0.0 ? double.NaN : value;
            }
        }

        /// <summary>
        ///     Determines whether two GeoCoordinate objects refer to the same location.
        /// </summary>
        /// <returns>
        ///     true, if the GeoCoordinate objects are determined to be equivalent; otherwise, false.
        /// </returns>
        /// <param name="left">The first GeoCoordinate to compare.</param>
        /// <param name="right">The second GeoCoordinate to compare.</param>
        public static bool operator ==(GeoCoordinate left, GeoCoordinate right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two GeoCoordinate objects correspond to different locations.
        /// </summary>
        /// <returns>
        ///     true, if the GeoCoordinate objects are determined to be different; otherwise, false.
        /// </returns>
        /// <param name="left">The first GeoCoordinate to compare.</param>
        /// <param name="right">The second GeoCoordinate to compare.</param>
        public static bool operator !=(GeoCoordinate left, GeoCoordinate right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Determines if a specified GeoCoordinate is equal to the current GeoCoordinate, based solely on latitude and
        ///     longitude.
        /// </summary>
        /// <returns>
        ///     true, if the GeoCoordinate objects are equal; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare the GeoCoordinate to.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as GeoCoordinate);
        }

        /// <summary>
        ///     Serves as a hash function for the GeoCoordinate.
        /// </summary>
        /// <returns>
        ///     A hash code for the current GeoCoordinate.
        /// </returns>
        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode();
        }

        /// <summary>
        ///     Returns a string that contains the latitude and longitude.
        /// </summary>
        /// <returns>
        ///     A string that contains the latitude and longitude, separated by a comma.
        /// </returns>
        public override string ToString()
        {
            if (this == Unknown)
            {
                return "Unknown";
            }

            return
                $"{Latitude.ToString("G", CultureInfo.InvariantCulture)}, {Longitude.ToString("G", CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        ///     Determines if the GeoCoordinate object is equivalent to the parameter, based solely on latitude and longitude.
        /// </summary>
        /// <returns>
        ///     true if the GeoCoordinate objects are equal; otherwise, false.
        /// </returns>
        /// <param name="other">The GeoCoordinate object to compare to the calling object.</param>
        public bool Equals(GeoCoordinate other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var num = Latitude;

            if (!num.Equals(other.Latitude))
            {
                return false;
            }

            num = Longitude;

            return num.Equals(other.Longitude);
        }

        /// <summary>
        ///     Returns the distance between the latitude and longitude coordinates that are specified by this GeoCoordinate and
        ///     another specified GeoCoordinate.
        /// </summary>
        /// <returns>
        ///     The distance between the two coordinates, in meters.
        /// </returns>
        /// <param name="other">The GeoCoordinate for the location to calculate the distance to.</param>
        public double GetDistanceTo(GeoCoordinate other)
        {
            if (double.IsNaN(Latitude) ||
                double.IsNaN(Longitude) ||
                double.IsNaN(other.Latitude) ||
                double.IsNaN(other.Longitude))
            {
                throw new ArgumentException("Argument latitude or longitude is not a number");
            }

            var d1 = Latitude * (Math.PI / 180.0);
            var num1 = Longitude * (Math.PI / 180.0);
            var d2 = other.Latitude * (Math.PI / 180.0);
            var num2 = other.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}
