using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using DevilDaggersAssetEditor.Code.TabControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls.TabControls
{
	public partial class AudioTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileNameProperty = DependencyProperty.Register
		(
			"BinaryFileName",
			typeof(BinaryFileName),
			typeof(AudioTabControl),
			new PropertyMetadata(BinaryFileName.Audio)
		);

		public BinaryFileName BinaryFileName
		{
			get => (BinaryFileName)GetValue(BinaryFileNameProperty);
			set => SetValue(BinaryFileNameProperty, value);
		}

		public AudioTabControlHandler Handler { get; private set; }

		public AudioTabControl()
		{
			InitializeComponent();

			Handler = new AudioTabControlHandler(BinaryFileName);

			foreach (AudioAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}