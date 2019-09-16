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
		public static readonly DependencyProperty BinaryFileNameProperty = DependencyProperty.Register
		(
			nameof(BinaryFileName),
			typeof(string),
			typeof(AudioTabControl)
		);

		public string BinaryFileName
		{
			get => (string)GetValue(BinaryFileNameProperty);
			set => SetValue(BinaryFileNameProperty, value);
		}

		public AudioTabControlHandler Handler { get; private set; }

		public AudioTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioTabControlHandler((BinaryFileName)Enum.Parse(typeof(BinaryFileName), BinaryFileName));

			foreach (AudioAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}