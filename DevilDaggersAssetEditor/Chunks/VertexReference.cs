namespace DevilDaggersAssetEditor.Chunks
{
	public struct VertexReference
	{
		public uint PositionReference { get; set; }
		public uint TexCoordReference { get; set; }
		public uint NormalReference { get; set; }

		public VertexReference(uint positionReference, uint texCoordReference, uint normalReference)
		{
			PositionReference = positionReference;
			TexCoordReference = texCoordReference;
			NormalReference = normalReference;
		}

		public VertexReference(uint unifiedReference)
		{
			PositionReference = unifiedReference;
			TexCoordReference = unifiedReference;
			NormalReference = unifiedReference;
		}

		public override string ToString()
			=> $"{PositionReference}/{TexCoordReference}/{NormalReference}";
	}
}