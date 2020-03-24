namespace DevilDaggersAssetCore.Data
{
	public struct Vector3
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(float xyz)
		{
			X = xyz;
			Y = xyz;
			Z = xyz;
		}

		public override string ToString() => $"X: {X}, Y: {Y}, Z: {Z}";
	}
}