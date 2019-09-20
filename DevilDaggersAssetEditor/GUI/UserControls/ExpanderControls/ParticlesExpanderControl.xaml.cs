using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System;
using DevilDaggersAssetEditor.Code.ExpanderControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls.ExpanderControls
{
	public partial class ParticlesExpanderControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ParticlesExpanderControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ParticlesExpanderControlHandler Handler { get; private set; }

		public ParticlesExpanderControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ParticlesExpanderControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (ParticleAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}