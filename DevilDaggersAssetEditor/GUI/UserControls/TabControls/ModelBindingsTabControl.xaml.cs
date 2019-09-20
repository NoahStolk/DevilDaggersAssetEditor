using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using System;

namespace DevilDaggersAssetEditor.GUI.UserControls.TabControls
{
	public partial class ModelBindingsTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ModelBindingsTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ModelBindingsTabControlHandler Handler { get; private set; }

		public ModelBindingsTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ModelBindingsTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (ModelBindingAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}