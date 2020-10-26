using System;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public class ProgressWrapper
	{
		public ProgressWrapper(Progress<float> progressPercentage, Progress<string> progressDescription)
		{
			ProgressPercentage = progressPercentage;
			ProgressDescription = progressDescription;
		}

		public Progress<float> ProgressPercentage { get; }
		public Progress<string> ProgressDescription { get; }

		public void Report(string description, float? percentage = null)
		{
			((IProgress<string>)ProgressDescription).Report(description);
			if (percentage.HasValue)
				((IProgress<float>)ProgressPercentage).Report(percentage.Value);
		}
	}
}