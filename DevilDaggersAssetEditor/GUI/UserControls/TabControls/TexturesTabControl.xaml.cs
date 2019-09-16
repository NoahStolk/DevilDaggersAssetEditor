using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using System;

namespace DevilDaggersAssetEditor.GUI.UserControls.TabControls
{
	public partial class TexturesTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileNameProperty = DependencyProperty.Register
		(
			nameof(BinaryFileName),
			typeof(string),
			typeof(TexturesTabControl)
		);

		public string BinaryFileName
		{
			get => (string)GetValue(BinaryFileNameProperty);
			set => SetValue(BinaryFileNameProperty, value);
		}

		public TexturesTabControlHandler Handler { get; private set; }

		public TexturesTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Handler = new TexturesTabControlHandler((BinaryFileName)Enum.Parse(typeof(BinaryFileName), BinaryFileName));

			foreach (TextureAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}