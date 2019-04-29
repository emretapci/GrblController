namespace GrblController
{
	internal class Vector3D
	{
		internal double X { get; set; }
		internal double Y { get; set; }
		internal double Z { get; set; }

		internal Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}
