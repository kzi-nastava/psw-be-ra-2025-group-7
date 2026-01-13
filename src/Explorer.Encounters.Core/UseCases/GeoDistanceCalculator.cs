using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public static class GeoDistanceCalculator
    {
        private const double EarthRadiusMeters = 6371000;

        public static bool IsWithinRadius(
            double centerLat,
            double centerLon,
            double radiusMeters,
            double userLat,
            double userLon)
        {
            double dLat = ToRadians(userLat - centerLat);
            double dLon = ToRadians(userLon - centerLon);

            double lat1 = ToRadians(centerLat);
            double lat2 = ToRadians(userLat);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = EarthRadiusMeters * c;

            return distance <= radiusMeters;
        }

        private static double ToRadians(double degrees)
            => degrees * Math.PI / 180;
    }
}
