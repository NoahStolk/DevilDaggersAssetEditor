using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using System;

namespace DevilDaggersAssetEditor.GUI.UserControls.TabControls
{
	public partial class ShadersTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileNameProperty = DependencyProperty.Register
		(
			nameof(BinaryFileName),
			typeof(string),
			typeof(ShadersTabControl)
		);

		public string BinaryFileName
		{
			get => (string)GetValue(BinaryFileNameProperty);
			set => SetValue(BinaryFileNameProperty, value);
		}

		public ShadersTabControlHandler Handler { get; private set; }

		public ShadersTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Handler = new ShadersTabControlHandler((BinaryFileName)Enum.Parse(typeof(BinaryFileName), BinaryFileName));

			foreach (ShaderAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}