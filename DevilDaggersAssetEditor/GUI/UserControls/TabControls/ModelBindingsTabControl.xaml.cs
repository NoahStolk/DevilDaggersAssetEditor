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
		public static readonly DependencyProperty BinaryFileNameProperty = DependencyProperty.Register
		(
			nameof(BinaryFileName),
			typeof(string),
			typeof(ModelBindingsTabControl)
		);

		public string BinaryFileName
		{
			get => (string)GetValue(BinaryFileNameProperty);
			set => SetValue(BinaryFileNameProperty, value);
		}

		public ModelBindingsTabControlHandler Handler { get; private set; }

		public ModelBindingsTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ModelBindingsTabControlHandler((BinaryFileName)Enum.Parse(typeof(BinaryFileName), BinaryFileName));

			foreach (ModelBindingAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}