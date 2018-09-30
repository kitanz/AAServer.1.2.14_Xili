﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Globalization;

namespace LocalCommons.World
{
	public struct Location
	{
		/// <summary>
		/// Map id.
		/// </summary>
		public readonly int MapId;

		/// <summary>
		/// X coordinate (left/right).
		/// </summary>
		public readonly float X;

		/// <summary>
		/// Y coordinate (up/down).
		/// </summary>
		public readonly float Y;

		/// <summary>
		/// Z coordinate (depth).
		/// </summary>
		public readonly float Z;

		/// <summary>
		/// Creates new location from given data.
		/// </summary>
		/// <param name="mapId"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Location(int mapId, float x, float y, float z)
		{
			this.MapId = mapId;
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Creates new location from map id and position.
		/// </summary>
		/// <param name="mapId"></param>
		/// <param name="pos"></param>
		public Location(int mapId, Position pos)
		{
			this.MapId = mapId;
			this.X = pos.X;
			this.Y = pos.Y;
			this.Z = pos.Z;
		}

		/// <summary>
		/// Creates new location from location.
		/// </summary>
		/// <param name="loc"></param>
		public Location(Location loc)
		{
			this.MapId = loc.MapId;
			this.X = loc.X;
			this.Y = loc.Y;
			this.Z = loc.Z;
		}

		/// <summary>
		/// Returns true if both locations represent the same location.
		/// </summary>
		/// <param name="loc1"></param>
		/// <param name="loc2"></param>
		/// <returns></returns>
		public static bool operator ==(Location loc1, Location loc2)
		{
		    // Define the tolerance for variation in their values
		    double differenceXAbs = Math.Abs(loc1.X * .00001);
		    double differenceYAbs= Math.Abs(loc1.X * .00001);
		    double differenceZAbs = Math.Abs(loc1.X * .00001);

            return Math.Abs(loc1.X - loc2.X) < differenceXAbs && Math.Abs(loc1.Y - loc2.Y) < differenceYAbs && Math.Abs(loc1.Z - loc2.Z) < differenceZAbs;
		}

        /// <summary>
		/// Returns true if locations aren't representing the same location.
		/// </summary>
		/// <param name="loc1"></param>
		/// <param name="loc2"></param>
		/// <returns></returns>
		public static bool operator !=(Location loc1, Location loc2)
		{
			return !(loc1 == loc2);
		}

		/// <summary>
		/// Returns hash code for this location, calculated out of the coorindates.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.MapId.GetHashCode() ^ this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
		}

		/// <summary>
		/// Returns true if the given object is the same instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return obj is Location location && this == location;
		}

		/// <summary>
		/// Returns string representation of location.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", MapId, X, Y, Z);
		}
	}
}
