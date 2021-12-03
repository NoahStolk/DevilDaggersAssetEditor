using System;

namespace DevilDaggersAssetEditor.Progress;

public class ProgressWrapper
{
	public ProgressWrapper(Progress<string> progressDescription, Progress<float> progressPercentage)
	{
		ProgressDescription = progressDescription;
		ProgressPercentage = progressPercentage;
	}

	public Progress<string> ProgressDescription { get; }
	public Progress<float> ProgressPercentage { get; }

	public void Report(string description, float? percentage = null)
	{
		((IProgress<string>)ProgressDescription).Report(description);
		if (percentage.HasValue)
			((IProgress<float>)ProgressPercentage).Report(percentage.Value);
	}
}