namespace DevilDaggersAssetCore.Data
{
	public struct Vector2
	{
		public float X { get; set; }
		public float Y { get; set; }

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public Vector2(float xy)
		{
			X = xy;
			Y = xy;
		}

		public override string ToString() => $"X: {X}, Y: {Y}";
	}
}