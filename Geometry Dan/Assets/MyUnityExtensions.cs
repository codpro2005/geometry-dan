using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MyUnityExtensions
{
	public static class MyUnityExtensions
	{
		#region Vector3

		public static bool BiggerThan(this Vector3 main, Vector3 toCompare)
		{
			return main.x > toCompare.x && main.y > toCompare.y && main.z > toCompare.z;
		}

		public static bool BiggerOrEqualThan(this Vector3 main, Vector3 toCompare)
		{
			return main.x >= toCompare.x && main.y >= toCompare.y && main.z >= toCompare.z;
		}

		public static Vector3Int ToVector3Int(this Vector3 vector3)
		{
			return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
		}

		#endregion

		#region Vector2

		public static bool BiggerThan(this Vector2 main, Vector2 toCompare)
		{
			return main.x > toCompare.x && main.y > toCompare.y;
		}

		public static bool BiggerOrEqualThan(this Vector2 main, Vector2 toCompare)
		{
			return main.x >= toCompare.x && main.y >= toCompare.y;
		}

		public static Vector2Int ToVector2Int(this Vector2 vector2)
		{
			return new Vector2Int((int)vector2.x, (int)vector2.y);
		}

		#endregion
	}
}
