namespace DevilDaggersAssetCore.Data
{
	public struct VertexReference
	{
		public int PositionReference { get; set; }
		public int TexCoordReference { get; set; }
		public int NormalReference { get; set; }

		public VertexReference(int positionReference, int texCoordReference, int normalReference)
		{
			PositionReference = positionReference;
			TexCoordReference = texCoordReference;
			NormalReference = normalReference;
		}

		public VertexReference(int unifiedReference)
		{
			PositionReference = unifiedReference;
			TexCoordReference = unifiedReference;
			NormalReference = unifiedReference;
		}

		public override string ToString() => $"{PositionReference}/{TexCoordReference}/{NormalReference}";
	}
}