using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using System;

namespace DevilDaggersAssetEditor.GUI.UserControls.TabControls
{
	public partial class AudioTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(AudioTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AudioTabControlHandler Handler { get; private set; }

		public AudioTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (AudioAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}