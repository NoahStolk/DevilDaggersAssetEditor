namespace DevilDaggersAssetEditor.Binaries.Chunks;

public struct VertexReference
{
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

	public uint PositionReference { get; set; }
	public uint TexCoordReference { get; set; }
	public uint NormalReference { get; set; }

	public override string ToString()
		=> $"{PositionReference}/{TexCoordReference}/{NormalReference}";
}
