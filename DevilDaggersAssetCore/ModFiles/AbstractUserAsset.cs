﻿namespace DevilDaggersAssetCore.ModFiles
{
	public abstract class AbstractUserAsset
	{
		public abstract string ChunkTypeName { get; }

		public string AssetName { get; }
		public string EditorPath { get; set; }

		public AbstractUserAsset(string assetName, string editorPath)
		{
			AssetName = assetName;
			EditorPath = editorPath;
		}
	}
}